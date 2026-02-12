using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;
using static MOSTComputers.UI.Web.Blazor.Components._Tests.CommonLogic;

namespace MOSTComputers.UI.Web.Blazor.Components._Tests;

public class SaveImageFilesFromImagesAll
{
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileService _productImageFileService;

    public SaveImageFilesFromImagesAll(
        IProductImageService productImageService,
        IProductImageFileService productImageFileService)
    {
        _productImageService = productImageService;
        _productImageFileService = productImageFileService;
    }

    public async Task SaveFilesFromImagesAllAsync(
        List<MOSTComputers.Models.Product.Models.Product> products,
        string currentUserName,
        Func<OneOf<ProductTestMetrics, ValidationResult, List<string>>, Task>? onChanged = null)
    {
        ProductTestMetrics productTestMetrics = new();

        List<IGrouping<int, ProductImageData>> imageDatas = await _productImageService.GetAllWithoutFileDataAsync();
        List<IGrouping<int, ProductImageFileData>> imageFiles = await _productImageFileService.GetAllAsync()
            .ContinueWith(task => task.Result.GroupBy(x => x.ProductId)
            .ToList());

        List<MOSTComputers.Models.Product.Models.Product> productsThatNeedFileUpdates = GetProductsWithFileDifferences(products, imageDatas, imageFiles);

        if (productsThatNeedFileUpdates.Count == 0) return;

        List<int> productIds = productsThatNeedFileUpdates.Select(x => x.Id).ToList();

        List<IGrouping<int, ProductImage>> images = await _productImageService.GetAllInProductsAsync(productIds);

        foreach (MOSTComputers.Models.Product.Models.Product productToUpdate in productsThatNeedFileUpdates)
        {
            IEnumerable<ProductImage>? imagesForProduct = images.FirstOrDefault(x => x.Key == productToUpdate.Id);

            imagesForProduct ??= Array.Empty<ProductImage>();

            IGrouping<int, ProductImageFileData>? imageFilesForProduct = imageFiles.FirstOrDefault(x => x.Key == productToUpdate.Id);

            OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateResult
                = await SaveImgAllFilesForProductAsync(
                    currentUserName, productToUpdate.Id, imagesForProduct, imageFilesForProduct, productTestMetrics, onChanged);
        }
    }

    public static List<MOSTComputers.Models.Product.Models.Product> GetProductsWithFileDifferences(
        List<MOSTComputers.Models.Product.Models.Product> products,
        List<IGrouping<int, ProductImageData>> imageDatas,
        List<IGrouping<int, ProductImageFileData>> imageFiles,
        Func<string, Task>? onChanged = null)
    {
        List<MOSTComputers.Models.Product.Models.Product> productsThatNeedFileUpdates = new();

        foreach (MOSTComputers.Models.Product.Models.Product product in products)
        {
            IGrouping<int, ProductImageData>? imagesForProduct = imageDatas.FirstOrDefault(x => x.Key == product.Id);
            IGrouping<int, ProductImageFileData>? imageFilesForProduct = imageFiles.FirstOrDefault(x => x.Key == product.Id);

            int imagesCount = imagesForProduct?.Count() ?? 0;
            int imageFilesCount = imageFilesForProduct?.Count() ?? 0;

            if (imagesCount != imageFilesCount)
            {
                productsThatNeedFileUpdates.Add(product);

                if (onChanged is not null)
                {
                    onChanged($"CSTID: {product.Id}, ImagesAll: {imagesCount}, Files: {imageFilesCount}");
                }

                continue;
            }

            if (imagesCount == 0) continue;

            bool needsFileUpdate = false;

            foreach (ProductImageData image in imagesForProduct!)
            {
                if (!imageFilesForProduct!.Any(imageFile => imageFile.ImageId == image.Id))
                {
                    needsFileUpdate = true;
                    break;
                }
            }

            if (needsFileUpdate)
            {
                productsThatNeedFileUpdates.Add(product);

                if (onChanged is not null)
                {
                    onChanged($"CSTID: {product.Id}, ImagesAll: {imagesCount}, Files: {imageFilesCount}");
                }
            }
        }

        return productsThatNeedFileUpdates;
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> SaveImgAllFilesForProductAsync(
            string upsertUserName,
            int productId,
            IEnumerable<ProductImage> originalImagesByProduct,
            IEnumerable<ProductImageFileData>? existingImageFilesByProduct = null,
            ProductTestMetrics? productTestMetrics = null,
            Func<OneOf<ProductTestMetrics, ValidationResult, List<string>>, Task>? onChanged = null)
    {
        foreach (ProductImage originalImage in originalImagesByProduct)
        {
            if (originalImage.ImageData is null
                || originalImage.ImageData.Length <= 0
                || originalImage.ImageContentType is null)
            {
                continue;
            }

            string contentType = originalImage.ImageContentType.Trim();

            if (contentType == "-?-")
            {
                contentType = "image/jpeg";
            }

            MemoryStream imageDataStream = new(originalImage.ImageData);

            if (!IsImageDataValid(imageDataStream))
            {
                continue;
            }

            OneOf<string, ValidationResult> getFileExtensionResult = GetFileExtensionFromImageContentType(contentType);

            if (!getFileExtensionResult.IsT0)
            {
                return getFileExtensionResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    fileExtension => new UnexpectedFailureResult(),
                    validationResult => validationResult);
            }

            string fileName = originalImage.Id.ToString();
            string fullFileName = fileName + getFileExtensionResult.AsT0;

            ProductImageFileData? existingFile = existingImageFilesByProduct?
                .FirstOrDefault(file => Path.GetFileNameWithoutExtension(file.FileName)?.Equals(fileName, StringComparison.OrdinalIgnoreCase) ?? false);

            if (existingFile is not null)
            {
                ProductImageFileUpdateRequest productImageFileUpdateRequest = new()
                {
                    Id = existingFile.Id,
                    UpdateFileDataRequest = new()
                    {
                        FileData = new FileData()
                        {
                            FileName = fullFileName,
                            Data = imageDataStream.ToArray(),
                        }
                    },
                    Active = true,
                    UpdateImageIdRequest = originalImage.Id,
                    UpdateUserName = upsertUserName,
                };

                OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
                    = await _productImageFileService.UpdateFileAsync(productImageFileUpdateRequest);

                if (!updateFileResult.IsT0) return updateFileResult;

                continue;
            }

            ProductImageFileCreateRequest productImageFileCreateRequest = new()
            {
                ProductId = productId,
                ImageId = originalImage.Id,
                FileData = new()
                {
                    FileName = fullFileName,
                    Data = imageDataStream.ToArray(),
                },
                Active = true,
                CreateUserName = upsertUserName,
            };

            OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createFileResult
                = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

            if (!createFileResult.IsT0)
            {
                return createFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    newFileId => new UnexpectedFailureResult(),
                    validationResult => validationResult,
                    fileSaveFailureResult => fileSaveFailureResult,
                    fileAlreadyExistsResult => fileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }
        }

        if (productTestMetrics is not null)
        {
            productTestMetrics.AddProcessedProductId(productId);

            foreach (ProductImage newProductImage in originalImagesByProduct)
            {
                productTestMetrics.AddProcessedImagesAllId(newProductImage.Id);
            }

            if (onChanged is not null)
            {
                await onChanged(productTestMetrics);
            }
        }

        return new Success();
    }
}