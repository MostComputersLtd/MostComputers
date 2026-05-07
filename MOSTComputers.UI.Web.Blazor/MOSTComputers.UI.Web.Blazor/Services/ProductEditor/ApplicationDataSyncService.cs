using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using OneOf;
using OneOf.Types;
using SixLabors.ImageSharp;

using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Services.ProductEditor;

public class ApplicationDataSyncService
{
    public ApplicationDataSyncService(
        IProductService productService,
        IProductSearchService productSearchService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        IGroupPromotionImagesRepository groupPromotionImagesRepository,
        IGroupPromotionImageFileService groupPromotionImageFileService,
        IProductWorkStatusesService productWorkStatusesService)
    {
        _productService = productService;
        _productSearchService = productSearchService;
        _productImageService = productImageService;
        _productImageFileService = productImageFileService;
        _groupPromotionImagesRepository = groupPromotionImagesRepository;
        _groupPromotionImageFileService = groupPromotionImageFileService;
        _productWorkStatusesService = productWorkStatusesService;
    }

    private readonly IProductService _productService;
    private readonly IProductSearchService _productSearchService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly IGroupPromotionImagesRepository _groupPromotionImagesRepository;
    private readonly IGroupPromotionImageFileService _groupPromotionImageFileService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;

    public async Task SyncApplicationAsync(string currentUserName)
    {
        await AddFilesToBrandPromotionsAsync();
        await AddStatusesToAllProductsWithoutStatusesAsync(currentUserName);
        await AddFilesForProductImagesInImagesAllAsync(currentUserName);
    }

    private async Task<List<int>> AddFilesToBrandPromotionsAsync()
    {
		List<int> imageIdsThatFailedSave = new();

		List<GroupPromotionImageWithoutFile> images = await _groupPromotionImagesRepository.GetAllWithoutFilesAsync();

		List<GroupPromotionImageFileData> imageFiles = await _groupPromotionImageFileService.GetAllAsync();

		foreach (GroupPromotionImageWithoutFile image in images)
		{
			GroupPromotionImageFileData? matchingFile = imageFiles.FirstOrDefault(x => x.ImageId == image.Id);

			if (matchingFile != null) continue;

			if (image.ContentType is null) continue;

			int indexOfDotInBadFormattedContentType = image.ContentType.IndexOf('.');

			string fileExtension = indexOfDotInBadFormattedContentType > 0
                ? image.ContentType[indexOfDotInBadFormattedContentType..]
				: string.Empty;

			if (string.IsNullOrEmpty(fileExtension)) continue;

			string fileName = image.Id.ToString() + fileExtension;

			GroupPromotionImageFileDataCreateRequest createRequest = new()
			{
				PromotionId = image.PromotionId,
				ImageId = image.Id,
				FileName = fileName,
			};

			OneOf<int, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult> result
				= await _groupPromotionImageFileService.InsertAsync(createRequest);

			if (!result.IsT0)
			{
				imageIdsThatFailedSave.Add(image.Id);
			}
		}

		return imageIdsThatFailedSave;
    }

    private async Task AddStatusesToAllProductsWithoutStatusesAsync(string currentUserName)
    {
        if (currentUserName is null) return;

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = false,
            ProductNewStatuses = [ProductNewStatusSearchOptions.None],
        };

        List<Product> productsWithoutStatuses = await _productSearchService.SearchProductsAsync(productSearchRequest);

        if (productsWithoutStatuses.Count == 0) return;

        ServiceProductWorkStatusesCreateManyWithSameDataRequest createAllRequest = new()
        {
            ProductIds = productsWithoutStatuses.Select(x => x.Id).ToList(),
            ProductNewStatus = ProductNewStatus.New,
            ProductXmlStatus = ProductXmlStatus.NotReady,
            ReadyForImageInsert = false,
            CreateUserName = currentUserName,
        };

        await _productWorkStatusesService.InsertAllIfTheyDontExistAsync(createAllRequest);
    }

    private async Task<Dictionary<Product, OneOf<ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>>
        AddFilesForProductImagesInImagesAllAsync(string currentUserName)
    {
        List<Product> products = await _productService.GetAllAsync();

        List<IGrouping<int, ProductImageData>> imageDatasList = await _productImageService.GetAllWithoutFileDataAsync();
        List<ProductImageFileData> imageFilesList = await _productImageFileService.GetAllAsync();

        Dictionary<int, List<ProductImageData>> imageDatas =
            imageDatasList.ToDictionary(x => x.Key, x => x.ToList());

        Dictionary<int, List<ProductImageFileData>> imageFiles =
            imageFilesList.GroupBy(x => x.ProductId).ToDictionary(x => x.Key, x => x.ToList());

        Dictionary<Product, OneOf<ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> output
            = new();

        List<Product> productsThatNeedFileUpdates
            = GetProductsWithFileDifferences(products, imageDatas, imageFiles);

        if (productsThatNeedFileUpdates.Count == 0) return output;

        List<int> productIds = productsThatNeedFileUpdates.Select(x => x.Id).ToList();

        List<IGrouping<int, ProductImage>> images = await _productImageService.GetAllInProductsAsync(productIds);

        foreach (Product productToUpdate in productsThatNeedFileUpdates)
        {
            IEnumerable<ProductImage>? imagesForProduct = images.FirstOrDefault(x => x.Key == productToUpdate.Id);

            imagesForProduct ??= Array.Empty<ProductImage>();

            IEnumerable<ProductImageFileData>? imageFilesForProduct = imageFiles[productToUpdate.Id];

            OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateResult
                = await SaveFilesFromImagesAsync(currentUserName, productToUpdate.Id, imagesForProduct, imageFilesForProduct);

            if (!updateResult.IsT0)
            {
                output.Add(productToUpdate, updateResult.Match<OneOf<ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    x => throw new InvalidOperationException("Cannot execute this block, because of the preceding if statement"),
                    validationResult => validationResult,
                    fileSaveFailureResult => fileSaveFailureResult,
                    fileDoesntExistResult => fileDoesntExistResult,
                    fileAlreadyExistsResult => fileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult));
            }
        }

        return output;
    }

	private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveFilesFromImagesAsync(
        string upsertUserName,
        int productId,
        IEnumerable<ProductImage> originalImagesByProduct,
        IEnumerable<ProductImageFileData>? existingImageFilesByProduct = null)
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

        return new Success();
    }

    private static List<Product> GetProductsWithFileDifferences(
        List<Product> products,
        Dictionary<int, List<ProductImageData>> imageDatas,
        Dictionary<int, List<ProductImageFileData>> imageFiles)
    {
        List<Product> productsThatNeedFileUpdates = new();

        foreach (Product product in products)
        {
            bool hasImages = imageDatas.TryGetValue(product.Id, out List<ProductImageData>? imagesForProduct);
            bool hasFiles = imageFiles.TryGetValue(product.Id, out List<ProductImageFileData>? imageFilesForProduct);

            int imagesCount = hasImages ? imagesForProduct!.Count : 0;
            int imageFilesCount = hasFiles ? imageFilesForProduct!.Count : 0;

            if (imagesCount != imageFilesCount)
            {
                productsThatNeedFileUpdates.Add(product);

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
            }
        }

        return productsThatNeedFileUpdates;
    }

    public static OneOf<string, ValidationResult> GetFileExtensionFromImageContentType(string? imageContentType)
    {
        if (!IsImageContentType(imageContentType))
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "Image content type is invalid");

            return new ValidationResult([validationFailure]);
        }

        string? fileName = TEMP__GetImageFileExtensionFromImageData(imageContentType);

        if (fileName is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "File name is invalid");

            return new ValidationResult([validationFailure]);
        }

        string fileExtension = Path.GetExtension(fileName);

        if (fileExtension is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImage.ImageContentType), "File name is invalid");

            return new ValidationResult([validationFailure]);
        }

        return fileExtension;
    }

    public static string? TEMP__GetImageFileExtensionFromImageData(string imageContentType)
    {
        if (imageContentType == "image/jpeg") return ".jpeg";

        List<string> possibleFileExtensions = GetPossibleExtensionsFromContentType(imageContentType);

        string? extensionToUse = possibleFileExtensions.FirstOrDefault();

        return extensionToUse;
    }


    private static bool IsImageDataValid(Stream imageDataStream)
    {
        try
        {
            Image.Load(imageDataStream);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
