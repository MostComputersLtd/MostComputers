using System.Transactions;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductDocuments;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductHtml;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Utils.ImageUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.Services.ProductRegister.Services.NewSaveService;

internal sealed record ProductDataSaveValidationData(
    Product Product,
    List<ProductCharacteristic> ProductCharacteristics,
    List<ProductImageData> Images,
    List<ProductImageFileData> AllImageFiles,
    List<ProductImageFileData> ImageFiles,
    List<PromotionProductFileInfo> PromotionProductFiles,
    List<PromotionFileInfo> PromotionFiles,
    List<ProductDocument> ProductDocuments);

public interface IProductDataSaveService
{
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> SaveAsync(ProductRelatedItemsFullSaveRequest request);
}

internal sealed class ProductDataSaveService(
    IProductRepository productRepository,
    IProductCharacteristicService productCharacteristicService,
    IProductPropertyCrudService productPropertyCrudService,
    IProductImageCrudService productImageCrudService,
    IProductImageFileService productImageFileService,
    IPromotionProductFileInfoService promotionProductFileInfoService,
    IPromotionFileService promotionFileService,
    IProductDocumentFileService productDocumentFileService,
    IProductToHtmlProductService productToHtmlProductService,
    IProductHtmlService productHtmlService,
    IProductWorkStatusesService productWorkStatusesService,
    ILogger<ProductDataSaveService>? logger) : IProductDataSaveService
{
    private sealed class ImageUpsertDataInUpsert
    {
        public required ProductImageAndPromotionFileUpsertRequest UpsertRequest { get; init; }
        public required OneOf<ServiceProductImageCreateRequest, ProductImageUpsertRequest> ImageCrudOptions { get; set; }
        public required bool ReplaceRequestHtmlWithPropertyHtml { get; set; }
    }

    private readonly IProductRepository _productRepository = productRepository;
    private readonly IProductCharacteristicService _productCharacteristicService = productCharacteristicService;
    private readonly IProductPropertyCrudService _productPropertyCrudService = productPropertyCrudService;
    private readonly IProductImageCrudService _productImageCrudService = productImageCrudService;
    private readonly IProductImageFileService _productImageFileService = productImageFileService;
    private readonly IPromotionProductFileInfoService _promotionProductFileInfoService = promotionProductFileInfoService;
    private readonly IPromotionFileService _promotionFileService = promotionFileService;
    private readonly IProductDocumentFileService _productDocumentFileService = productDocumentFileService;
    private readonly IProductToHtmlProductService _productToHtmlProductService = productToHtmlProductService;
    private readonly IProductHtmlService _productHtmlService = productHtmlService;
    private readonly IProductWorkStatusesService _productWorkStatusesService = productWorkStatusesService;
    private readonly ILogger<ProductDataSaveService>? _logger = logger;

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> SaveAsync(
        ProductRelatedItemsFullSaveRequest request)
    {
        int productId = request.ProductId;

        using TransactionScope transactionScope = new(
            TransactionScopeOption.Required,
            new TransactionOptions() { IsolationLevel = IsolationLevel.Serializable },
            TransactionScopeAsyncFlowOption.Enabled);

        Product? product = await _productRepository.GetByIdAsync(productId);

        ProductDataSaveValidationDataProductIdValidator productIdValidator = new();

        ValidationResult productIdValidationResult = productIdValidator.Validate((request, product));

        if (!productIdValidationResult.IsValid || product == null)
        {
            return productIdValidationResult;
        }

        ProductDataSaveValidationData validationData = await GetValidationDataAsync(request, product);

        ProductDataSaveValidationDataValidator validator = new(validationData);

        ValidationResult validationResult = validator.Validate(request);

        if (!validationResult.IsValid) return validationResult;

        List<ProductProperty> existingProductProperties = await _productPropertyCrudService.GetAllInProductAsync(productId);
        List<ProductImage> allExistingProductImages = await _productImageCrudService.GetAllInProductAsync(productId);
        ProductWorkStatuses? existingWorkStatuses = await _productWorkStatusesService.GetByProductIdAsync(productId);

        List<ProductImage> productImagesToDelete = new(allExistingProductImages);

        List<ProductImageFileData> oldProductImageFiles = validationData.ImageFiles;
        List<PromotionProductFileInfo> oldPromotionProductFileInfos = validationData.PromotionProductFiles;

        List<ProductImageAndPromotionFileUpsertRequest> imageRequests = OrderItemsWithDisplayOrders(
            request.ImageRequests, oldPromotionProductFileInfos);

        foreach (ProductImageAndPromotionFileUpsertRequest upsertRequest in imageRequests)
        {
            PromotionProductFileInfo? existingPromotionFile
                = FindExisting(oldPromotionProductFileInfos, upsertRequest);

            ProductImageFileData? existingImageFile
                = FindExisting(oldProductImageFiles, upsertRequest);

            ProductImage? existingImage
                = FindExisting(allExistingProductImages, upsertRequest, existingPromotionFile);

            if (existingPromotionFile != null)
            {
                oldPromotionProductFileInfos.Remove(existingPromotionFile);
            }

            if (existingImageFile != null)
            {
                oldProductImageFiles.Remove(existingImageFile);
            }

            if (existingImage != null)
            {
                productImagesToDelete.Remove(existingImage);
            }
        }

        List<ImageUpsertDataInUpsert> imageUpsertDataInUpserts = new();
        ServiceProductFirstImageUpsertRequest? productFirstImageUpsertRequest = null;

        for (int i = 0; i < imageRequests.Count; i++)
        {
            ProductImageAndPromotionFileUpsertRequest upsertRequest = imageRequests[i];

            if (upsertRequest.Request.IsT0)
            {
                ProductImageWithFileForProductUpsertRequest imageUpsertRequest = upsertRequest.Request.AsT0;

                string? contentTypeFromFileExtension = GetContentTypeFromExtension(imageUpsertRequest.FileExtension);

                ProductImageUpsertRequest productImageUpsertRequest = new()
                {
                    ProductId = productId,
                    ExistingImageId = imageUpsertRequest.ExistingImageId,
                    ImageContentType = contentTypeFromFileExtension,
                    ImageData = imageUpsertRequest.ImageData,
                    HtmlData = imageUpsertRequest.HtmlDataOptions.Match(
                        useCurrentData => null,
                        doNotUpdate =>
                        {
                            ProductImage? existingImage
                                = allExistingProductImages.FirstOrDefault(x => x.Id == imageUpsertRequest.ExistingImageId);

                            return existingImage?.HtmlData;
                        },
                        updateToCustomHtmlData => updateToCustomHtmlData.HtmlData),
                };

                productFirstImageUpsertRequest ??= new()
                {
                    ProductId = productId,
                    ImageContentType = productImageUpsertRequest.ImageContentType,
                    ImageData = productImageUpsertRequest.ImageData,
                    HtmlData = productImageUpsertRequest.HtmlData,
                };

                imageUpsertDataInUpserts.Add(new()
                {
                    UpsertRequest = upsertRequest,
                    ImageCrudOptions = productImageUpsertRequest,
                    ReplaceRequestHtmlWithPropertyHtml = imageUpsertRequest.HtmlDataOptions.IsT0,
                });

                continue;
            }
            else if (upsertRequest.Request.IsT3)
            {
                PromotionProductFileForProductUpsertRequest promotionProductFileUpsertRequest = upsertRequest.Request.AsT3;

                if (promotionProductFileUpsertRequest.UpsertInProductImagesRequest == null) continue;

                PromotionFileInfo promotionFile = validationData.PromotionFiles.Find(
                    x => x.Id == promotionProductFileUpsertRequest.PromotionFileInfoId)!;

                using Stream? promotionFileDataStream
                    = await _promotionFileService.GetFileDataByIdAsync(promotionProductFileUpsertRequest.PromotionFileInfoId);

                if (promotionFileDataStream == null)
                {
                    return LogAndReturnErrorResult(
                        new UnexpectedFailureResult(), productId, "gathering image data");
                }

                using MemoryStream memoryStream = new();

                await promotionFileDataStream.CopyToAsync(memoryStream);

                byte[] fileData = memoryStream.ToArray();

                string? contentType = GetContentTypeFromExtension(promotionFile.FileName);

                if (promotionProductFileUpsertRequest.Id == null)
                {
                    ServiceProductImageCreateRequest productImageCreateRequest = new()
                    {
                        ProductId = productId,
                        HtmlData = promotionProductFileUpsertRequest.UpsertInProductImagesRequest.HtmlDataOptions.Match(
                            useCurrentData => null,
                            doNotUpdate => null,
                            updateToCustomHtmlData => updateToCustomHtmlData.HtmlData),
                        ImageData = fileData,
                        ImageContentType = contentType,
                    };

                    productFirstImageUpsertRequest ??= new()
                    {
                        ProductId = productId,
                        ImageContentType = productImageCreateRequest.ImageContentType,
                        ImageData = productImageCreateRequest.ImageData,
                        HtmlData = productImageCreateRequest.HtmlData,
                    };

                    imageUpsertDataInUpserts.Add(new()
                    {
                        UpsertRequest = upsertRequest,
                        ImageCrudOptions = productImageCreateRequest,
                        ReplaceRequestHtmlWithPropertyHtml = promotionProductFileUpsertRequest.UpsertInProductImagesRequest.HtmlDataOptions.IsT0,
                    });

                    continue;
                }

                PromotionProductFileInfo promotionProductFile = validationData.PromotionProductFiles.Find(
                    x => x.Id == promotionProductFileUpsertRequest.Id.Value)!;

                ProductImageUpsertRequest productImageUpsertRequest = new()
                {
                    ProductId = productId,
                    ExistingImageId = promotionProductFile.ProductImageId,
                    ImageContentType = contentType,
                    ImageData = fileData,
                    HtmlData = promotionProductFileUpsertRequest.UpsertInProductImagesRequest.HtmlDataOptions.Match(
                        useCurrentData => null,
                        doNotUpdate =>
                        {
                            PromotionProductFileInfo? existingPromotionProductFile
                                = oldPromotionProductFileInfos.Find(x => x.Id == promotionProductFileUpsertRequest.Id);

                            if (existingPromotionProductFile?.ProductImageId == null) return null;

                            ProductImage? existingImage
                                = allExistingProductImages.Find(x => x.Id == existingPromotionProductFile.ProductImageId);

                            return existingImage?.HtmlData;
                        },
                        updateToCustomHtmlData => updateToCustomHtmlData.HtmlData),
                };

                productFirstImageUpsertRequest ??= new()
                {
                    ProductId = productId,
                    ImageContentType = productImageUpsertRequest.ImageContentType,
                    ImageData = productImageUpsertRequest.ImageData,
                    HtmlData = productImageUpsertRequest.HtmlData,
                };

                imageUpsertDataInUpserts.Add(new()
                {
                    UpsertRequest = upsertRequest,
                    ImageCrudOptions = productImageUpsertRequest,
                    ReplaceRequestHtmlWithPropertyHtml = promotionProductFileUpsertRequest.UpsertInProductImagesRequest.HtmlDataOptions.IsT0,
                });
            }
        }

        OneOf<Success, UnexpectedFailureResult> updatePropertiesResult
            = await _productPropertyCrudService.FullSave_UpsertAllProductPropertiesInternalAsync(
                productId,
                request.PropertyRequests,
                existingProductProperties,
                validationData.ProductCharacteristics);

        if (updatePropertiesResult.IsT1)
        {
            return LogAndReturnErrorResult(updatePropertiesResult.AsT1, productId, "updating properties");
        }

        // List<ProductProperty> productProperties = await _notCachedProductPropertyCrudService.GetAllInProductAsync(productId);
        List<ProductProperty> productProperties = await _productPropertyCrudService.GetAllInProductAsync(productId);

        GetHtmlDataForProductRequest getHtmlDataRequest = new()
        {
            Product = product,
            ProductProperties = productProperties,
        };

        HtmlProductsData productHtmlData = _productToHtmlProductService.GetHtmlProductDataFromProducts([getHtmlDataRequest]);

        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(productHtmlData);

        if (!getProductHtmlResult.IsT0)
        {
            return LogAndReturnErrorResult(new UnexpectedFailureResult(), productId, "generating product HTML");
        }

        string productHtml = getProductHtmlResult.AsT0;

        Dictionary<ProductImageAndPromotionFileUpsertRequest, int> imageIdsForRequests = new();

        using (TransactionScope imagesTransactionScope
            = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            foreach (ProductImage oldImageToBeRemoved in productImagesToDelete)
            {
                bool imageDeleteResult = await _productImageCrudService.DeleteInAllImagesByIdAsync(oldImageToBeRemoved.Id);

                if (!imageDeleteResult)
                {
                    UnexpectedFailureResult result = new();

                    return LogAndReturnErrorResult(
                        result, productId, $"deleting image (ID: {oldImageToBeRemoved.Id})");
                }

                allExistingProductImages.Remove(oldImageToBeRemoved);
            }

            foreach (ImageUpsertDataInUpsert imageUpsertData in imageUpsertDataInUpserts)
            {
                OneOf<ServiceProductImageCreateRequest, ProductImageUpsertRequest> imageCrudOptions = imageUpsertData.ImageCrudOptions;

                if (imageCrudOptions.IsT0)
                {
                    ServiceProductImageCreateRequest imageCreateRequest = imageCrudOptions.AsT0;

                    if (imageUpsertData.ReplaceRequestHtmlWithPropertyHtml)
                    {
                        imageCreateRequest.HtmlData = productHtml;
                    }

                    OneOf<int, UnexpectedFailureResult> imageCreateResult
                        = await _productImageCrudService.FullSave_InsertInAllImagesAsync(imageCreateRequest);

                    if (imageCreateResult.IsT1)
                    {
                        return LogAndReturnErrorResult(
                            imageCreateResult.AsT1, productId, "creating image");
                    }

                    imageIdsForRequests.Add(imageUpsertData.UpsertRequest, imageCreateResult.AsT0);
                }
                else
                {
                    ProductImageUpsertRequest imageUpsertRequest = imageCrudOptions.AsT1;

                    if (imageUpsertData.ReplaceRequestHtmlWithPropertyHtml)
                    {
                        imageUpsertRequest.HtmlData = productHtml;
                    }

                    int? id = imageUpsertRequest.ExistingImageId;

                    if (imageUpsertRequest.ExistingImageId != null)
                    {
                        ProductImage existingProductImage = allExistingProductImages.Find(x => x.Id == imageUpsertRequest.ExistingImageId)!;

                        if (IsRequestEqualToExistingData(imageUpsertRequest, existingProductImage))
                        {
                            imageIdsForRequests.Add(imageUpsertData.UpsertRequest, imageUpsertRequest.ExistingImageId.Value);

                            continue;
                        }
                    }

                    OneOf<int, UnexpectedFailureResult> imageUpsertResult
                        = await _productImageCrudService.FullSave_UpsertInAllImagesAsync(imageUpsertRequest);

                    if (!imageUpsertResult.IsT0)
                    {
                        return imageUpsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                            id => new UnexpectedFailureResult(),
                            unexpectedFailureResult =>
                            {
                                string operationText = id == null ? "upserting image" : $"upserting image (ID: {id})";

                                return LogAndReturnErrorResult(unexpectedFailureResult, productId, operationText);
                            });
                    }

                    imageIdsForRequests.Add(imageUpsertData.UpsertRequest, imageUpsertResult.AsT0);
                }
            }

            if (productFirstImageUpsertRequest != null)
            {
                ProductImage? existingFirstImage = await _productImageCrudService.GetByProductIdInFirstImagesAsync(productId);

                if (existingFirstImage == null
                    || !IsRequestEqualToExistingData(productFirstImageUpsertRequest, existingFirstImage))
                {
                    OneOf<Success, UnexpectedFailureResult> productFirstImageUpsertResult
                        = await _productImageCrudService.FullSave_UpsertInFirstImagesAsync(productFirstImageUpsertRequest);

                    if (!productFirstImageUpsertResult.IsT0)
                    {
                        return productFirstImageUpsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                            success => success,
                            unexpectedFailureResult => LogAndReturnErrorResult(unexpectedFailureResult, productId, "upserting first image"));
                    }
                }
            }
            else
            {
                //ProductImage? oldProductFirstImage = await _productImageCrudService.GetByProductIdInFirstImagesAsync(productId);

                //if (oldProductFirstImage == null) return new Success();

                //bool isFirstImageDeleted = await _productImageCrudService.DeleteInFirstImagesByProductIdAsync(productId);

                //if (!isFirstImageDeleted) return new UnexpectedFailureResult();
            }

            imagesTransactionScope.Complete();
        }

        foreach (PromotionProductFileInfo oldPromotionProductFileToBeRemoved in oldPromotionProductFileInfos)
        {
            bool promotionProductFileDeleteResult = await _promotionProductFileInfoService.DeleteAsync(oldPromotionProductFileToBeRemoved.Id);

            if (!promotionProductFileDeleteResult)
            {
                UnexpectedFailureResult result = new();

                return LogAndReturnErrorResult(result, productId, $"deleting promotion product file (ID: {oldPromotionProductFileToBeRemoved.Id})");
            }
        }

        foreach (ProductImageFileData oldImageFileToBeRemoved in oldProductImageFiles)
        {
            OneOf<Success, NotFound, FileDoesntExistResult> imageFileDeleteResult
                = await _productImageFileService.FullSave_DeleteFileInternalAsync(oldImageFileToBeRemoved);

            if (!imageFileDeleteResult.IsT0)
            {
                UnexpectedFailureResult result = new();

                return LogAndReturnErrorResult(result, productId, $"deleting product image file (ID: {oldImageFileToBeRemoved.Id})");
            }
        }

        for (int i = 0; i < imageRequests.Count; i++)
        {
            ProductImageAndPromotionFileUpsertRequest upsertRequest = imageRequests[i];

            if (upsertRequest.Request.IsT0)
            {
                ProductImageWithFileForProductUpsertRequest imageCreateRequest = upsertRequest.Request.AsT0;

                if (imageCreateRequest.FileUpsertRequest == null) continue;

                int imageId = imageIdsForRequests[upsertRequest];

                string fileExtensionWithDot = imageCreateRequest.FileExtension.StartsWith('.')
                    ? imageCreateRequest.FileExtension
                    : '.' + imageCreateRequest.FileExtension;

                ProductImageFileData? fileData = oldProductImageFiles.Find(x => x.ImageId == imageId);

                OneOf<int, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertFileResult
                    = await FullSave_UpsertImageFileForImageAsync(
                        fileData,
                        productId,
                        imageId,
                        imageCreateRequest.ImageData,
                        imageCreateRequest.FileUpsertRequest.Active,
                        imageCreateRequest.FileUpsertRequest.CustomDisplayOrder,
                        fileExtensionWithDot,
                        request.UpsertUserName);

                if (!upsertFileResult.IsT0)
                {
                    return upsertFileResult.Match(
                        success => new UnexpectedFailureResult(),
                        fileSaveFailureResult => LogAndReturnErrorResult(fileSaveFailureResult, productId, "upserting image file"),
                        fileDoesntExistResult => LogAndReturnErrorResult(fileDoesntExistResult, productId, "upserting image file"),
                        fileAlreadyExistsResult => LogAndReturnErrorResult(fileAlreadyExistsResult, productId, "upserting image file"),
                        unexpectedFailureResult => LogAndReturnErrorResult(unexpectedFailureResult, productId, "upserting image file"));
                }
            }
            else if (upsertRequest.Request.IsT1)
            {
                ProductImageFileForProductCreateRequest imageFileCreateRequest = upsertRequest.Request.AsT1;

                ProductImageFileCreateRequest requestInner = new()
                {
                    ProductId = productId,
                    ImageId = null,
                    Active = imageFileCreateRequest.Active,
                    CustomDisplayOrder = imageFileCreateRequest.CustomDisplayOrder,
                    FileData = imageFileCreateRequest.FileData,
                    CreateUserName = request.UpsertUserName,
                };

                OneOf<int, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createFileResult
                    = await _productImageFileService.FullSave_InsertFileInternalAsync(requestInner);

                if (!createFileResult.IsT0)
                {
                    return createFileResult.Match(
                        success => new UnexpectedFailureResult(),
                        fileSaveFailureResult => LogAndReturnErrorResult(fileSaveFailureResult, productId, "creating image file"),
                        fileAlreadyExistsResult => LogAndReturnErrorResult(fileAlreadyExistsResult, productId, "creating image file"),
                        unexpectedFailureResult => LogAndReturnErrorResult(unexpectedFailureResult, productId, "creating image file"));
                }
            }
            else if (upsertRequest.Request.IsT2)
            {
                ProductImageFileForProductUpdateRequest imageFileUpdateRequest = upsertRequest.Request.AsT2;

                int? newDisplayOrder = null;

                if (imageFileUpdateRequest.UpdateDisplayOrderRequest.IsT0)
                {
                    newDisplayOrder = imageFileUpdateRequest.UpdateDisplayOrderRequest.AsT0;
                }

                ProductImageFileUpdateFileDataRequest? updateFileDataRequest = null;

                if (imageFileUpdateRequest.UpdateFileDataRequest.IsT0)
                {
                    updateFileDataRequest = new() { FileData = imageFileUpdateRequest.UpdateFileDataRequest.AsT0 };
                }

                ProductImageFileUpdateRequest productImageFileUpdateRequest = new()
                {
                    Id = imageFileUpdateRequest.Id,
                    Active = imageFileUpdateRequest.Active,
                    NewDisplayOrder = newDisplayOrder,
                    UpdateFileDataRequest = updateFileDataRequest,
                    UpdateUserName = request.UpsertUserName,
                };

                ProductImageFileData imageFile = oldProductImageFiles.Find(x => x.Id == imageFileUpdateRequest.Id)!;

                OneOf<Success, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
                    = await _productImageFileService.FullSave_UpdateFileInternalAsync(productImageFileUpdateRequest, imageFile);

                if (!updateFileResult.IsT0)
                {
                    return updateFileResult.Match(
                        success => new UnexpectedFailureResult(),
                        fileSaveFailureResult => LogAndReturnErrorResult(fileSaveFailureResult, productId, "updating image file"),
                        fileDoesntExistResult => LogAndReturnErrorResult(fileDoesntExistResult, productId, "updating image file"),
                        fileAlreadyExistsResult => LogAndReturnErrorResult(fileAlreadyExistsResult, productId, "updating image file"),
                        unexpectedFailureResult => LogAndReturnErrorResult(unexpectedFailureResult, productId, "updating image file"));
                }
            }
            else
            {
                PromotionProductFileForProductUpsertRequest promotionProductFileUpsertRequest = upsertRequest.Request.AsT3;

                PromotionFileInfo promotionFile = validationData.PromotionFiles.Find(
                    x => x.Id == promotionProductFileUpsertRequest.PromotionFileInfoId)!;

                string fileExtension = Path.GetExtension(promotionFile.FileName)!;

                int imageId = imageIdsForRequests[upsertRequest];

                if (promotionProductFileUpsertRequest.Id == null)
                {
                    ServicePromotionProductFileInfoCreateRequest promotionProductFileInfoCreateRequest = new()
                    {
                        ProductId = productId,
                        PromotionFileInfoId = promotionProductFileUpsertRequest.PromotionFileInfoId,
                        Active = promotionProductFileUpsertRequest.Active,
                        ValidFrom = promotionProductFileUpsertRequest.ValidFrom,
                        ValidTo = promotionProductFileUpsertRequest.ValidTo,
                        ProductImageId = imageId,
                        CreateUserName = request.UpsertUserName,
                    };

                    OneOf<int, UnexpectedFailureResult> createPromotionProductFileResult
                        = await _promotionProductFileInfoService.FullSave_InsertAsync(promotionProductFileInfoCreateRequest);

                    if (createPromotionProductFileResult.IsT1)
                    {
                        return LogAndReturnErrorResult(createPromotionProductFileResult.AsT1, productId, "creating promotion product file");
                    }

                    if (promotionProductFileUpsertRequest.UpsertInProductImagesRequest == null) continue;

                    string imageFileName = $"{imageId}{fileExtension}";

                    using Stream? innerPromotionFileDataStream
                        = await _promotionFileService.GetFileDataByIdAsync(promotionProductFileUpsertRequest.PromotionFileInfoId);

                    if (innerPromotionFileDataStream == null)
                    {
                        return LogAndReturnErrorResult(
                            new UnexpectedFailureResult(), productId, "creating image file");
                    }

                    using MemoryStream memoryStreamForInsert = new();

                    await innerPromotionFileDataStream.CopyToAsync(memoryStreamForInsert);

                    byte[] fileDataForInsert = memoryStreamForInsert.ToArray();

                    ProductImageFileCreateRequest productImageFileCreateRequest = new()
                    {
                        ProductId = productId,
                        ImageId = imageId,
                        FileData = new()
                        {
                            FileName = imageFileName,
                            Data = fileDataForInsert,
                        },
                        Active = promotionProductFileUpsertRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.Active,
                        CustomDisplayOrder = promotionProductFileUpsertRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.CustomDisplayOrder,
                        CreateUserName = request.UpsertUserName,
                    };

                    OneOf<int, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
                        = await _productImageFileService.FullSave_InsertFileInternalAsync(productImageFileCreateRequest);

                    if (!createImageFileResult.IsT0)
                    {
                        return createImageFileResult.Match(
                            success => new UnexpectedFailureResult(),
                            fileSaveFailureResult => LogAndReturnErrorResult(fileSaveFailureResult, productId, "creating image file"),
                            fileAlreadyExistsResult => LogAndReturnErrorResult(fileAlreadyExistsResult, productId, "creating image file"),
                            unexpectedFailureResult => LogAndReturnErrorResult(unexpectedFailureResult, productId, "creating image file"));
                    }

                    continue;
                }

                ServicePromotionProductFileInfoUpdateRequest promotionProductFileInfoUpdateRequest = new()
                {
                    Id = promotionProductFileUpsertRequest.Id.Value,
                    NewPromotionFileInfoId = promotionProductFileUpsertRequest.PromotionFileInfoId,
                    ValidFrom = promotionProductFileUpsertRequest.ValidFrom,
                    ValidTo = promotionProductFileUpsertRequest.ValidTo,
                    Active = promotionProductFileUpsertRequest.Active,
                    ProductImageId = imageId,
                    UpdateUserName = request.UpsertUserName,
                };

                OneOf<Success, NotFound> updatePromotionProductFileResult
                    = await _promotionProductFileInfoService.FullSave_UpdateAsync(promotionProductFileInfoUpdateRequest);

                if (!updatePromotionProductFileResult.IsT0)
                {
                    return updatePromotionProductFileResult.Match(
                        success => new UnexpectedFailureResult(),
                        notFound =>
                        {
                            UnexpectedFailureResult unexpectedFailureResult = new();

                            return LogAndReturnErrorResult(unexpectedFailureResult, productId, "updating promotion file");
                        });
                }

                if (promotionProductFileUpsertRequest.UpsertInProductImagesRequest == null) continue;

                using Stream? promotionFileDataStream
                    = await _promotionFileService.GetFileDataByIdAsync(promotionProductFileUpsertRequest.PromotionFileInfoId);

                if (promotionFileDataStream == null)
                {
                    return LogAndReturnErrorResult(
                        new UnexpectedFailureResult(), productId, "upserting image file");
                }

                using MemoryStream memoryStream = new();

                await promotionFileDataStream.CopyToAsync(memoryStream);

                byte[] fileData = memoryStream.ToArray();

                ProductImageFileData imageFile = oldProductImageFiles.Find(x => x.ImageId == imageId)!;

                OneOf<int, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertFileResult
                    = await FullSave_UpsertImageFileForImageAsync(
                        imageFile,
                        productId,
                        imageId,
                        fileData,
                        promotionProductFileUpsertRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.Active,
                        promotionProductFileUpsertRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.CustomDisplayOrder,
                        fileExtension,
                        request.UpsertUserName);

                if (!upsertFileResult.IsT0)
                {
                    return upsertFileResult.Match(
                        success => new UnexpectedFailureResult(),
                        fileSaveFailureResult => LogAndReturnErrorResult(fileSaveFailureResult, productId, "upserting image file"),
                        fileDoesntExistResult => LogAndReturnErrorResult(fileDoesntExistResult, productId, "upserting image file"),
                        fileAlreadyExistsResult => LogAndReturnErrorResult(fileAlreadyExistsResult, productId, "upserting image file"),
                        unexpectedFailureResult => LogAndReturnErrorResult(unexpectedFailureResult, productId, "upserting image file"));
                }
            }
        }

        foreach (ProductDocument productDocument in validationData.ProductDocuments)
        {
            ProductDocumentUpsertRequest? matchingRequest
                = request.DocumentRequests.Find(x => x.Request.IsT1 && x.Request.AsT1.ExistingId == productDocument.Id);

            if (matchingRequest != null) continue;

            OneOf<Success, NotFound> deleteResult = await _productDocumentFileService.DeleteAsync(productDocument.Id);

            if (!deleteResult.IsT0)
            {
                return LogAndReturnErrorResult(new UnexpectedFailureResult(), productId, "deleting document");
            }
        }

        for (int i = 0; i < request.DocumentRequests.Count; i++)
        {
            ProductDocumentUpsertRequest productDocumentUpsertRequest = request.DocumentRequests[i];

            if (productDocumentUpsertRequest.Request.IsT1)
            {
                ProductDocumentUpdateForProductRequest documentUpdateRequest = productDocumentUpsertRequest.Request.AsT1;

                ServiceProductDocumentUpdateRequest productDocumentUpdateRequest = new()
                {
                    IdOrFileName = documentUpdateRequest.ExistingId,
                    Description = documentUpdateRequest.Description,
                };

                OneOf<Success, NotFound> updateDocumentResult
                    = await _productDocumentFileService.FullSave_UpdateAsync(productDocumentUpdateRequest);

                if (!updateDocumentResult.IsT0)
                {
                    return updateDocumentResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                        success => LogAndReturnErrorResult(new UnexpectedFailureResult(), productId, "updating document record"),
                        notFound => LogAndReturnErrorResult(new UnexpectedFailureResult(), productId, "updating document record"));
                }

                continue;
            }

            ProductDocumentCreateForProductRequest documentCreateRequest = productDocumentUpsertRequest.Request.AsT0;

            ServiceProductDocumentCreateRequest productDocumentCreateRequest = new()
            {
                ProductId = productId,
                Description = documentCreateRequest.Description,
                FileData = documentCreateRequest.FileData,
                FileExtension = documentCreateRequest.FileExtension.ToLower(),
            };

            OneOf<ProductDocument, FileAlreadyExistsResult, UnexpectedFailureResult> documentCreateResult
                = await _productDocumentFileService.FullSave_InsertAsync(productDocumentCreateRequest);

            if (!documentCreateResult.IsT0)
            {
                return documentCreateResult.Match(
                    productDocument => LogAndReturnErrorResult(new UnexpectedFailureResult(), productId, "updating document record"),
                    fileAlreadyExistsResult => LogAndReturnErrorResult(fileAlreadyExistsResult, productId, "updating document record"),
                    unexpectedFailureResult => LogAndReturnErrorResult(new UnexpectedFailureResult(), productId, "updating document record")
                );
            }
        }

        if (existingWorkStatuses is null
            || existingWorkStatuses.ProductNewStatus == ProductNewStatus.New)
        {
            ServiceProductWorkStatusesUpsertRequest productWorkStatusesUpsertRequest = new()
            {
                ProductId = productId,
                ProductNewStatus = ProductNewStatus.WorkInProgress,
                ProductXmlStatus = existingWorkStatuses?.ProductXmlStatus ?? ProductXmlStatus.NotReady,
                ReadyForImageInsert = existingWorkStatuses?.ReadyForImageInsert ?? false,
                UpsertUserName = request.UpsertUserName,
            };

            OneOf<int, UnexpectedFailureResult> productNewStatusesUpsertResult
                = await _productWorkStatusesService.FullSave_UpsertByProductIdAsync(productWorkStatusesUpsertRequest);

            if (productNewStatusesUpsertResult.IsT1)
            {
                return LogAndReturnErrorResult(
                    productNewStatusesUpsertResult.AsT1, productId, "upserting product work status");
            }
        }

        transactionScope.Complete();

        return new Success();
    }

    private async Task<ProductDataSaveValidationData> GetValidationDataAsync(
        ProductRelatedItemsFullSaveRequest request, Product product)
    {
        List<int> relatedCategoryIds = [-1];

        if (product.CategoryId != null)
        {
            relatedCategoryIds.Add(product.CategoryId.Value);
        }

        List<ProductCharacteristicType> productCharacteristicTypes = [ProductCharacteristicType.ProductCharacteristic, ProductCharacteristicType.Link];

        List<ProductCharacteristic> relatedProductCharacteristics = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            relatedCategoryIds, productCharacteristicTypes, true);

        List<ProductImageData> images = await _productImageCrudService.GetAllInProductWithoutFileDataAsync(request.ProductId);

        List<PromotionProductFileInfo> promotionProductFiles
            = await _promotionProductFileInfoService.GetAllForProductAsync(request.ProductId);

        HashSet<int> promotionFileIds = new();

        foreach (ProductImageAndPromotionFileUpsertRequest imageRequest in request.ImageRequests)
        {
            if (imageRequest.Request.IsT3)
            {
                promotionFileIds.Add(imageRequest.Request.AsT3.PromotionFileInfoId);
            }
        }

        foreach (PromotionProductFileInfo promotionProductFile in promotionProductFiles)
        {
            promotionFileIds.Add(promotionProductFile.PromotionFileInfoId);
        }

        List<PromotionFileInfo> promotionFiles = await _promotionFileService.GetByIdsAsync(promotionFileIds);

        List<ProductImageFileData> allImageFiles = await _productImageFileService.GetAllAsync();

        List<ProductImageFileData> imageFiles = allImageFiles
            .Where(x => x.ProductId == product.Id)
            .ToList();

        List<ProductDocument> productDocuments = await _productDocumentFileService.GetAllForProductAsync(request.ProductId);

        return new(
            product,
            relatedProductCharacteristics,
            images,
            imageFiles,
            allImageFiles,
            promotionProductFiles,
            promotionFiles,
            productDocuments);
    }

    private async Task<OneOf<int, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> FullSave_UpsertImageFileForImageAsync(
        ProductImageFileData? fileNameInfo,
        int productId,
        int imageId,
        byte[] imageData,
        bool? active,
        int? customDisplayOrder,
        string fileExtensionWithDot,
        string upsertUserName)
    {
        string? newImageFileName = $"{imageId}{fileExtensionWithDot}";

        if (fileNameInfo == null)
        {
            ProductImageFileCreateRequest productImageFileCreateRequest = new()
            {
                ProductId = productId,
                ImageId = imageId,
                FileData = new()
                {
                    FileName = newImageFileName,
                    Data = imageData,
                },
                Active = active,
                CustomDisplayOrder = customDisplayOrder,
                CreateUserName = upsertUserName,
            };

            OneOf<int, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createFileResult
                = await _productImageFileService.FullSave_InsertFileInternalAsync(productImageFileCreateRequest);

            return createFileResult.Match<OneOf<int, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                fileInfoId => fileInfoId,
                fileSaveFailureResult => fileSaveFailureResult,
                fileAlreadyExistsResult => fileAlreadyExistsResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        int fileInfoId = fileNameInfo.Id;

        int? newDisplayOrder = null;

        if (customDisplayOrder != null
            && customDisplayOrder != fileNameInfo.DisplayOrder)
        {
            newDisplayOrder = customDisplayOrder.Value;
        }

        ProductImageFileUpdateRequest productImageFileUpdateRequest = new()
        {
            Id = fileInfoId,
            Active = active,
            NewDisplayOrder = newDisplayOrder,
            UpdateFileDataRequest = new()
            {
                FileData = new()
                {
                    FileName = newImageFileName,
                    Data = imageData,
                },
            },
            UpdateImageIdRequest = new No(),
            UpdateUserName = upsertUserName,
        };

        OneOf<Success, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
            = await _productImageFileService.FullSave_UpdateFileInternalAsync(productImageFileUpdateRequest, fileNameInfo);

        return updateFileResult.Match<OneOf<int, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
            success => fileInfoId,
            fileSaveFailureResult => fileSaveFailureResult,
            fileDoesntExistResult => fileDoesntExistResult,
            fileAlreadyExistsResult => fileAlreadyExistsResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    private static ProductImage? FindExisting(
       List<ProductImage> existingProductImages,
       ProductImageAndPromotionFileUpsertRequest request,
       PromotionProductFileInfo? existingPromotionFile)
    {
        return request.Request.Match(
            imageWithFileUpsertRequest =>
            {
                if (imageWithFileUpsertRequest.ExistingImageId == null) return null;

                return existingProductImages.FirstOrDefault(x => x.Id == imageWithFileUpsertRequest.ExistingImageId);
            },
            imageFileCreateRequest => null,
            imageFileUpdateRequest => null,
            promotionProductFileUpsertRequest =>
            {
                if (existingPromotionFile?.ProductImageId == null) return null;

                return existingProductImages.FirstOrDefault(x => x.Id == existingPromotionFile.ProductImageId);
            });
    }

    private static ProductImageFileData? FindExisting(
        List<ProductImageFileData> existingProductImageFileInfos,
        ProductImageAndPromotionFileUpsertRequest request)
    {
        return request.Request.Match(
            imageWithFileUpsertRequest =>
            {
                return existingProductImageFileInfos.Find(x => x.ImageId == imageWithFileUpsertRequest.ExistingImageId);
            },
            imageFileCreateRequest => null,
            imageFileUpdateRequest => existingProductImageFileInfos.Find(x => x.Id == imageFileUpdateRequest.Id),
            promotionProductFileUpsertRequest => null);
    }

    private static PromotionProductFileInfo? FindExisting(
        List<PromotionProductFileInfo> existingPromotionProductFileInfos,
        ProductImageAndPromotionFileUpsertRequest request)
    {
        if (!request.Request.IsT3) return null;

        int? id = request.Request.AsT3.Id;

        if (id == null) return null;

        return existingPromotionProductFileInfos.Find(x => x.Id == id);
    }

    private static List<ProductImageAndPromotionFileUpsertRequest> OrderItemsWithDisplayOrders(
        List<ProductImageAndPromotionFileUpsertRequest> itemsWithDisplayOrders,
        List<PromotionProductFileInfo> existingPromotionProductFileInfos)
    {
        return OrderImageRelatedItems(
            itemsWithDisplayOrders,
            request => request.Request.Match(
                imageWithFileUpsert => imageWithFileUpsert.FileUpsertRequest?.CustomDisplayOrder,
                imageFileCreate => imageFileCreate.CustomDisplayOrder,
                imageFileUpdate => imageFileUpdate.UpdateDisplayOrderRequest.IsT0 ? imageFileUpdate.UpdateDisplayOrderRequest.AsT0 : null,
                promotionProductFileUpsert => promotionProductFileUpsert.UpsertInProductImagesRequest?.ImageFileUpsertRequest.CustomDisplayOrder),

            request => request.Request.Match(
                imageWithFileUpsert => imageWithFileUpsert.ExistingImageId,
                imageFileCreate => null,
                imageFileUpdate => null,
                promotionProductFileUpsert =>
                {
                    PromotionProductFileInfo? existingPromotionFile
                        = existingPromotionProductFileInfos.Find(x => x.Id == promotionProductFileUpsert.Id);

                    return existingPromotionFile?.ProductImageId;
                }));
    }

    private static bool IsRequestEqualToExistingData(ProductImageUpsertRequest request, ProductImage existingProductImage)
    {
        if (existingProductImage.Id != request.ExistingImageId
            || existingProductImage.ImageContentType != request.ImageContentType
            || existingProductImage.HtmlData != request.HtmlData)
        {
            return false;
        }

        if (existingProductImage.ImageData is null != request.ImageData is null) return false;

        if (existingProductImage.ImageData == null) return true;

        return existingProductImage.ImageData.AsSpan().SequenceEqual(request.ImageData.AsSpan());
    }

    private static bool IsRequestEqualToExistingData(ServiceProductFirstImageUpsertRequest request, ProductImage existingProductImage)
    {
        if (existingProductImage.ProductId != request.ProductId
            || existingProductImage.ImageContentType != request.ImageContentType
            || existingProductImage.HtmlData != request.HtmlData)
        {
            return false;
        }

        if (existingProductImage.ImageData is null != request.ImageData is null) return false;

        if (existingProductImage.ImageData is null) return true;

        return existingProductImage.ImageData.AsSpan().SequenceEqual(request.ImageData.AsSpan());
    }

    private UnexpectedFailureResult LogAndReturnErrorResult(FileSaveFailureResult fileSaveFailureResult, int productId, string operation)
    {
        if (_logger == null || !_logger.IsEnabled(LogLevel.Critical))
        {
            return new UnexpectedFailureResult();
        }

        if (fileSaveFailureResult.IsUnsupportedContentType)
        {
            NotSupportedContentTypeResult data = fileSaveFailureResult.UnsupportedContentType!.Value;

            _logger.LogCritical("File save error when {operation} for product ID {ProductId}, Invalid Content type: {ContentType}",
                operation, productId, data.ContentType);
        }
        else if (fileSaveFailureResult.IsInvalidContent)
        {
            InvalidContentResult data = fileSaveFailureResult.InvalidContent!.Value;

            _logger.LogCritical("File save error when {operation} for product ID {ProductId}, Invalid Content: {Reason}",
                operation, productId, data.Reason);
        }
        else if (fileSaveFailureResult.IsTooLarge)
        {
            TooLargeResult data = fileSaveFailureResult.TooLarge!.Value;

            _logger.LogCritical("File save error when {operation} for product ID {ProductId}, Too Large: Max Allowed Size (bytes): {MaxSizeBytes}, Actual Size: {ActualSizeBytes}",
                operation, productId, data.MaxSizeBytes, data.ActualSizeBytes);
        }
        else if (fileSaveFailureResult.IsUnexpected)
        {
            UnexpectedResult data = fileSaveFailureResult.Unexpected!.Value;

            _logger.LogCritical("File save error when {operation} for product ID {ProductId}, Unexpected Error: {ExceptionMessage}",
                operation, productId, data.ExceptionMessage);
        }

        return new UnexpectedFailureResult();
    }

    private UnexpectedFailureResult LogAndReturnErrorResult(FileDoesntExistResult fileDoesntExistResult, int productId, string operation)
    {
        if (_logger == null || !_logger.IsEnabled(LogLevel.Critical))
        {
            return new UnexpectedFailureResult();
        }

        _logger.LogCritical("File doesn't exist when {operation} for product ID {ProductId}, File Name: {FileName}",
            operation, productId, fileDoesntExistResult.FileName);

        return new UnexpectedFailureResult();
    }

    private UnexpectedFailureResult LogAndReturnErrorResult(FileAlreadyExistsResult fileAlreadyExistsResult, int productId, string operation)
    {
        if (_logger == null || !_logger.IsEnabled(LogLevel.Critical))
        {
            return new UnexpectedFailureResult();
        }

        _logger.LogCritical("File already exists when {operation} for product ID {ProductId}, File Name: {FileName}",
            operation, productId, fileAlreadyExistsResult.FileName);

        return new UnexpectedFailureResult();
    }

    private UnexpectedFailureResult LogAndReturnErrorResult(UnexpectedFailureResult unexpectedFailureResult, int productId, string operation)
    {
        if (_logger == null || !_logger.IsEnabled(LogLevel.Critical))
        {
            return unexpectedFailureResult;
        }

        _logger.LogCritical("Unexpected failure when {operation} for product ID {ProductId}", operation, productId);

        return unexpectedFailureResult;
    }
}
