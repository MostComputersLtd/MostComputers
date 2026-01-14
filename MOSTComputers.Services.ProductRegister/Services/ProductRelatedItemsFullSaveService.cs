using Azure.Core;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImage;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionProductFiles;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Configuration;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductHtml;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageAndPromotionFileSave;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.ProductRegister.Validation;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using static MOSTComputers.Services.ProductRegister.Utils.ImageUtils;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Utils.Files.ContentTypeUtils;
using static MOSTComputers.Utils.Files.FileExtensionUtils;

namespace MOSTComputers.Services.ProductRegister.Services;
internal class ProductRelatedItemsFullSaveService : IProductRelatedItemsFullSaveService
{
    /// <remarks>
    /// notCachedProductPropertyCrudService is used for fetching updated product properties in scenarios where they might have changed during the transaction.
    /// </remarks>
    public ProductRelatedItemsFullSaveService(
        IProductPropertyCrudService productPropertyCrudService,
        [FromKeyedServices(ConfigureServices.ProductPropertyCrudServiceKey)] IProductPropertyCrudService notCachedProductPropertyCrudService,
        IProductImageCrudService productImageCrudService,
        IProductImageAndFileService productImageAndFileService,
        IProductImageFileService productImageFileService,
        IProductImageFileManagementService productImageFileManagementService,
        IPromotionFileService promotionFileService,
        IPromotionProductFileService promotionProductFileService,
        IPromotionProductFileInfoService promotionProductFileInfoService,
        IProductWorkStatusesWorkflowService productWorkStatusesWorkflowService,
        IProductRepository productRepository,
        IProductToHtmlProductService productToHtmlProductService,
        IProductHtmlService productHtmlService,
        ITransactionExecuteService transactionExecuteService,
        IValidator<ProductRelatedItemsFullSaveRequest>? productRelatedItemsFullSaveRequestValidator = null,
        IValidator<ServiceProductImageCreateRequest>? serviceProductImageCreateRequestValidator = null,
        IValidator<ServiceProductImageUpdateRequest>? serviceProductImageUpdateRequestValidator = null,
        IValidator<ProductImageUpsertRequest>? productImageUpsertRequestValidator = null,
        IValidator<ServicePromotionProductFileInfoCreateRequest>? servicePromotionProductFileInfoCreateRequestValidator = null,
        IValidator<ServicePromotionProductFileInfoUpdateRequest>? servicePromotionProductFileInfoUpdateRequestValidator = null,
        ILogger<ProductRelatedItemsFullSaveService>? logger = null)
    {
        _productPropertyCrudService = productPropertyCrudService;
        _notCachedProductPropertyCrudService = notCachedProductPropertyCrudService;
        _productImageCrudService = productImageCrudService;
        _productImageAndFileService = productImageAndFileService;
        _productImageFileService = productImageFileService;
        _productImageFileManagementService = productImageFileManagementService;
        _promotionFileService = promotionFileService;
        _promotionProductFileService = promotionProductFileService;
        _promotionProductFileInfoService = promotionProductFileInfoService;
        _productWorkStatusesWorkflowService = productWorkStatusesWorkflowService;
        _productRepository = productRepository;
        _productToHtmlProductService = productToHtmlProductService;
        _productHtmlService = productHtmlService;
        _transactionExecuteService = transactionExecuteService;
        _productRelatedItemsFullSaveRequestValidator = productRelatedItemsFullSaveRequestValidator;
        _serviceProductImageCreateRequestValidator = serviceProductImageCreateRequestValidator;
        _serviceProductImageUpdateRequestValidator = serviceProductImageUpdateRequestValidator;
        _productImageUpsertRequestValidator = productImageUpsertRequestValidator;
        _servicePromotionProductFileInfoCreateRequestValidator = servicePromotionProductFileInfoCreateRequestValidator;
        _servicePromotionProductFileInfoUpdateRequestValidator = servicePromotionProductFileInfoUpdateRequestValidator;
        _logger = logger;
    }

    private const string _invalidProductIdErrorMessage = "Product id does not correspond to any known product id";

    private const string _invalidImageIdErrorMessage = "Image id does not correspond to any known image id";

    private const string _duplicateImageOrderErrorMessage = "Cannot have 2 image files with the same order";
    private const string _duplicateImageFileNameErrorMessage = "Cannot have 2 image files with the same name";

    private const string _invalidImageFileIdErrorMessage = "Image File id does not correspond to any known image id";
    private const string _invalidPromotionFileIdErrorMessage = "Id does not correspond to any known promotion file id";
    private const string _promotionFileIsNotAnImageErrorMessage = "Promotion file is not an image";
    private const string _invalidPromotionProductFileIdErrorMessage = "Promotion Product File id does not correspond to any known image id";
    private const string _invalidFileTypeErrorMessage = "File type not supported";

    private const string _unsupportedIdForDeleteErrorMessage = "Currently, we only support removing the images that were initially created from this application (to avoid conflicts)";

    private readonly IProductPropertyCrudService _productPropertyCrudService;
    private readonly IProductPropertyCrudService _notCachedProductPropertyCrudService;
    private readonly IProductImageCrudService _productImageCrudService;
    private readonly IProductImageAndFileService _productImageAndFileService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly IProductImageFileManagementService _productImageFileManagementService;
    private readonly IPromotionFileService _promotionFileService;
    private readonly IPromotionProductFileService _promotionProductFileService;
    private readonly IPromotionProductFileInfoService _promotionProductFileInfoService;
    private readonly IProductWorkStatusesWorkflowService _productWorkStatusesWorkflowService;
    private readonly IProductRepository _productRepository;
    private readonly IProductToHtmlProductService _productToHtmlProductService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly IValidator<ProductRelatedItemsFullSaveRequest>? _productRelatedItemsFullSaveRequestValidator;
    private readonly IValidator<ServiceProductImageCreateRequest>? _serviceProductImageCreateRequestValidator;
    private readonly IValidator<ServiceProductImageUpdateRequest>? _serviceProductImageUpdateRequestValidator;
    private readonly IValidator<ProductImageUpsertRequest>? _productImageUpsertRequestValidator;
    private readonly IValidator<ServicePromotionProductFileInfoCreateRequest>? _servicePromotionProductFileInfoCreateRequestValidator;
    private readonly IValidator<ServicePromotionProductFileInfoUpdateRequest>? _servicePromotionProductFileInfoUpdateRequestValidator;
    private readonly ILogger<ProductRelatedItemsFullSaveService>? _logger;

    private sealed class ImageUpsertDataInUpsert
    {
        public required ProductImageAndPromotionFileUpsertRequest UpsertRequest { get; init; }
        public OneOf<ServiceProductImageCreateRequest, ServiceProductImageUpdateRequest, ProductImageUpsertRequest, No> ImageCrudOptions { get; set; }
        public bool ReplaceRequestHtmlWithPropertyHtml { get; set; } = false;
    }

    public async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveProductRelatedItemsAsync(ProductRelatedItemsFullSaveRequest updateAllRequest)
    {
        ValidationResult validationResult = ValidateDefault(_productRelatedItemsFullSaveRequestValidator, updateAllRequest);

        if (!validationResult.IsValid) return validationResult;

        Product? product = await _productRepository.GetByIdAsync(updateAllRequest.ProductId);

        if (product is null)
        {
            ValidationFailure validationFailure = new(nameof(updateAllRequest.ProductId), _invalidProductIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        OneOf<Yes, ValidationResult, UnexpectedFailureResult> validateImageAndPromotionFileDataResult = await ValidateImageAndPromotionFileDataAsync(updateAllRequest);

        if (!validateImageAndPromotionFileDataResult.IsT0)
        {
            return validateImageAndPromotionFileDataResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                yes => new UnexpectedFailureResult(),
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<Yes, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> validateFileDataResult
            = await ValidateFileDataAsync(updateAllRequest);

        if (!validateFileDataResult.IsT0)
        {
            return validateFileDataResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                yes => new UnexpectedFailureResult(),
                validationResult => validationResult,
                fileDoesntExistResult => fileDoesntExistResult,
                fileAlreadyExistsResult => fileAlreadyExistsResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => UpsertAllImagesAndFilesForProductInternalAsync(updateAllRequest),
        //    result => result.IsT0);

        return await SaveProductRelatedItemsInternalAsync(updateAllRequest);
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        SaveProductRelatedItemsInternalAsync(ProductRelatedItemsFullSaveRequest upsertAllRequest)
    {
        int productId = upsertAllRequest.ProductId;
        string upsertUserName = upsertAllRequest.UpsertUserName;

        List<ProductImage> allExistingProductImages = await _productImageCrudService.GetAllInProductAsync(productId);
        List<ProductImage> productImagesToDelete = new(allExistingProductImages);

        List<ProductImageFileData> oldProductImageFileInfos = await _productImageFileService.GetAllInProductAsync(productId);
        List<PromotionProductFileInfo> oldPromotionProductFileInfos = await _promotionProductFileInfoService.GetAllForProductAsync(productId);

        upsertAllRequest.ImageRequests = OrderItemsWithDisplayOrders(upsertAllRequest.ImageRequests, oldPromotionProductFileInfos);

        foreach (ProductImageAndPromotionFileUpsertRequest upsertRequest in upsertAllRequest.ImageRequests)
        {
            OneOf<PromotionProductFileInfo?, ValidationResult> getExistingPromotionFileResult
                = FindExisting(oldPromotionProductFileInfos, upsertRequest);

            if (getExistingPromotionFileResult.IsT1) return getExistingPromotionFileResult.AsT1;

            PromotionProductFileInfo? existingPromotionFile = getExistingPromotionFileResult.AsT0;

            OneOf<ProductImageFileData?, ValidationResult, UnexpectedFailureResult> getExistingImageFileResult
                = FindExisting(oldProductImageFileInfos, upsertRequest, existingPromotionFile);

            if (!getExistingImageFileResult.IsT0)
            {
                return getExistingImageFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    productImageFileNameInfo => new UnexpectedFailureResult(),
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            ProductImageFileData? existingImageFile = getExistingImageFileResult.AsT0;

            OneOf<ProductImage?, ValidationResult, UnexpectedFailureResult> getExistingImageResult
                = FindExisting(allExistingProductImages, upsertRequest, existingPromotionFile);

            if (!getExistingImageResult.IsT0)
            {
                return getExistingImageResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    productImageFileNameInfo => new UnexpectedFailureResult(),
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            ProductImage? existingImage = getExistingImageResult.AsT0;

            if (existingPromotionFile is not null)
            {
                oldPromotionProductFileInfos.Remove(existingPromotionFile);
            }

            if (existingImageFile is not null)
            {
                oldProductImageFileInfos.Remove(existingImageFile);
            }

            if (existingImage is not null)
            {
                productImagesToDelete.Remove(existingImage);
            }
        }

        foreach (ProductImage oldProductImage in productImagesToDelete)
        {
            if (oldProductImage.Id < _productImageCrudService.GetMinimumImagesAllInsertIdForLocalApplication())
            {
                ValidationFailure validationFailure = new(nameof(ProductRelatedItemsFullSaveRequest.ImageRequests), _unsupportedIdForDeleteErrorMessage);

                return new ValidationResult([validationFailure]);
            }
        }

        //using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        List<ImageUpsertDataInUpsert> imageUpsertDataInUpserts = new();

        for (int i = 0; i < upsertAllRequest.ImageRequests.Count; i++)
        {
            ProductImageAndPromotionFileUpsertRequest upsertRequest = upsertAllRequest.ImageRequests[i];

            if (upsertRequest.Request.IsT0)
            {
                ProductImageWithFileForProductUpsertRequest request = upsertRequest.Request.AsT0;

                string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(request.FileExtension);

                string? contentTypeFromFileExtension = GetContentTypeFromExtension(request.FileExtension);

                if (contentTypeFromFileExtension is null)
                {
                    string itemPrefix = GetImageRequestValidationErrorPath(upsertRequest, i);

                    string fullPropertyPath = $"{itemPrefix}.{nameof(ProductImageWithFileForProductUpsertRequest.FileExtension)}";

                    ValidationFailure validationFailure = new(fullPropertyPath, _invalidFileTypeErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
                {
                    string itemPrefix = GetImageRequestValidationErrorPath(upsertRequest, i);

                    string fullPropertyPath = $"{itemPrefix}.{nameof(ProductImageWithFileForProductUpsertRequest.FileExtension)}";

                    ValidationFailure validationFailure = new(fullPropertyPath, _invalidFileTypeErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                ProductImageUpsertRequest productImageUpsertRequest = new()
                {
                    ProductId = productId,
                    ExistingImageId = request.ExistingImageId,
                    ImageContentType = contentTypeFromFileExtension,
                    ImageData = request.ImageData,
                    HtmlData = request.HtmlDataOptions.Match(
                        useCurrentData => null,
                        doNotUpdate =>
                        {
                            ProductImage? existingImage = allExistingProductImages.FirstOrDefault(x => x.Id == request.ExistingImageId);

                            return existingImage?.HtmlData;
                        },
                        updateToCustomHtmlData => updateToCustomHtmlData.HtmlData),
                };

                ValidationResult validationResult = ValidateDefault(_productImageUpsertRequestValidator, productImageUpsertRequest);

                if (!validationResult.IsValid)
                {
                    TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, i);

                    return validationResult;
                }

                imageUpsertDataInUpserts.Add(new()
                {
                    UpsertRequest = upsertRequest,
                    ImageCrudOptions = productImageUpsertRequest,
                    ReplaceRequestHtmlWithPropertyHtml = request.HtmlDataOptions.IsT0,
                });

                continue;
            }
            else if (upsertRequest.Request.IsT1)
            {
                continue;
            }
            else if (upsertRequest.Request.IsT2)
            {
                continue;
            }
            else
            {
                PromotionProductFileForProductUpsertRequest request = upsertRequest.Request.AsT3;

                if (request.UpsertInProductImagesRequest is null) continue;

                PromotionFileInfo? promotionFile;
                byte[] fileData;

                //using (TransactionScope localDBReadSuppressedScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                //{
                promotionFile = await _promotionFileService.GetByIdAsync(request.PromotionFileInfoId);

                using Stream? promotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(request.PromotionFileInfoId);

                if (promotionFile is null || promotionFileDataStream is null)
                {
                    string itemPrefix = GetImageRequestValidationErrorPath(upsertRequest, i);

                    string fullPropertyPath = $"{itemPrefix}.{nameof(request.PromotionFileInfoId)}";

                    ValidationFailure validationFailure = new(fullPropertyPath, _invalidPromotionFileIdErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                using MemoryStream memoryStream = new();

                await promotionFileDataStream.CopyToAsync(memoryStream);

                fileData = memoryStream.ToArray();

                //localDBReadSuppressedScope.Complete();
                //}

                string? contentType = GetContentTypeFromExtension(promotionFile.FileName);

                if (!IsImageContentType(contentType))
                {
                    ValidationFailure validationFailure = new(nameof(PromotionFileInfo.FileName), _promotionFileIsNotAnImageErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                if (request.Id is null)
                {
                    ServiceProductImageCreateRequest productImageCreateRequest = new()
                    {
                        ProductId = productId,
                        HtmlData = request.UpsertInProductImagesRequest.HtmlDataOptions.Match(
                            useCurrentData => null,
                            doNotUpdate => null,
                            updateToCustomHtmlData => updateToCustomHtmlData.HtmlData),
                        ImageData = fileData,
                        ImageContentType = contentType,
                    };

                    ValidationResult createValidationResult = ValidateDefault(_serviceProductImageCreateRequestValidator, productImageCreateRequest);

                    if (!createValidationResult.IsValid)
                    {
                        TransformValidationResultErrorPathsForImageRequests(createValidationResult, upsertRequest, i);

                        return createValidationResult;
                    }

                    imageUpsertDataInUpserts.Add(new()
                    {
                        UpsertRequest = upsertRequest,
                        ImageCrudOptions = productImageCreateRequest,
                        ReplaceRequestHtmlWithPropertyHtml = request.UpsertInProductImagesRequest.HtmlDataOptions.IsT0,
                    });

                    continue;
                }

                PromotionProductFileInfo? promotionProductFile;

                //using (TransactionScope localDBReadSuppressedScope2 = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                //{
                promotionProductFile = await _promotionProductFileInfoService.GetByIdAsync(request.Id.Value);

                //    localDBReadSuppressedScope2.Complete();
                //}

                if (promotionProductFile is null)
                {
                    string requestValidationErrorPath = GetImageRequestValidationErrorPath(upsertRequest, i);

                    ValidationFailure validationFailure = new($"{requestValidationErrorPath}.{nameof(request.Id)}", _invalidPromotionProductFileIdErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                ProductImageUpsertRequest productImageUpsertRequest = new()
                {
                    ProductId = productId,
                    ExistingImageId = promotionProductFile.ProductImageId,
                    ImageContentType = contentType,
                    ImageData = fileData,
                    HtmlData = request.UpsertInProductImagesRequest.HtmlDataOptions.Match(
                        useCurrentData => null,
                        doNotUpdate =>
                        {
                            PromotionProductFileInfo? existingPromotionProductFile
                                = oldPromotionProductFileInfos.FirstOrDefault(x => x.Id == request.Id);

                            if (existingPromotionProductFile?.ProductImageId is null) return null;

                            ProductImage? existingImage = allExistingProductImages.FirstOrDefault(x => x.Id == existingPromotionProductFile.ProductImageId);

                            return existingImage?.HtmlData;
                        },
                        updateToCustomHtmlData => updateToCustomHtmlData.HtmlData),
                };

                ValidationResult validationResult = ValidateDefault(_productImageUpsertRequestValidator, productImageUpsertRequest);

                if (!validationResult.IsValid)
                {
                    TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, i);

                    return validationResult;
                }

                imageUpsertDataInUpserts.Add(new()
                {
                    UpsertRequest = upsertRequest,
                    ImageCrudOptions = productImageUpsertRequest,
                    ReplaceRequestHtmlWithPropertyHtml = request.UpsertInProductImagesRequest.HtmlDataOptions.IsT0,
                });
            }
        }

        Dictionary<ProductImageAndPromotionFileUpsertRequest, int> imageIdsForRequests = new();

        using TransactionScope transactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        ProductPropertyUpsertAllForProductRequest productPropertyChangeAllForProductRequest = new()
        {
            ProductId = productId,
            NewProperties = upsertAllRequest.PropertyRequests,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updatePropertiesResult
            = await _productPropertyCrudService.UpsertAllProductPropertiesAsync(productPropertyChangeAllForProductRequest);

        if (!updatePropertiesResult.IsT0)
        {
            return updatePropertiesResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => success,
                validationResult =>
                {
                    TransformValidationResultErrors(validationResult, error =>
                    {
                        int propertyListIndex = error.PropertyName.IndexOf(nameof(ProductPropertyUpsertAllForProductRequest.NewProperties));

                        if (propertyListIndex < 0) return;

                        error.PropertyName = error.PropertyName.Remove(propertyListIndex, nameof(ProductPropertyUpsertAllForProductRequest.NewProperties).Length);

                        error.PropertyName = error.PropertyName.Insert(propertyListIndex, nameof(ProductRelatedItemsFullSaveRequest.PropertyRequests));
                    });

                    return validationResult;
                },
                unexpectedFailureResult => unexpectedFailureResult);
        }

        Product? product = await _productRepository.GetByIdAsync(productId);

        if (product is null) return new UnexpectedFailureResult();

        List<ProductProperty> productProperties = await _notCachedProductPropertyCrudService.GetAllInProductAsync(productId);

        GetHtmlDataForProductRequest getHtmlDataRequest = new()
        {
            Product = product,
            ProductProperties = productProperties,
        };

        HtmlProductsData productHtmlData = _productToHtmlProductService.GetHtmlProductDataFromProducts([getHtmlDataRequest]);

        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(productHtmlData);

        if (!getProductHtmlResult.IsT0) return new UnexpectedFailureResult();

        string productHtml = getProductHtmlResult.AsT0;

        using (TransactionScope imagesTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            foreach (ProductImage oldImageToBeRemoved in productImagesToDelete)
            {
                bool imageDeleteResult = await _productImageCrudService.DeleteInAllImagesByIdAsync(oldImageToBeRemoved.Id);

                if (!imageDeleteResult)
                {
                    UnexpectedFailureResult result = new();

                    return LogUnexpectedFailureResult(result, productId, $"deleting image (ID: {oldImageToBeRemoved.Id})");
                }

                allExistingProductImages.Remove(oldImageToBeRemoved);
            }

            ServiceProductFirstImageUpsertRequest? productFirstImageUpsertRequest = null;

            string? productFirstImageRequestTraceData = null;

            foreach (ImageUpsertDataInUpsert imageUpsertData in imageUpsertDataInUpserts)
            {
                OneOf<ServiceProductImageCreateRequest, ServiceProductImageUpdateRequest, ProductImageUpsertRequest, No> imageCrudOptions
                    = imageUpsertData.ImageCrudOptions;

                if (imageCrudOptions.IsT0)
                {
                    ServiceProductImageCreateRequest request = imageCrudOptions.AsT0;

                    if (imageUpsertData.ReplaceRequestHtmlWithPropertyHtml)
                    {
                        request.HtmlData = productHtml;
                    }

                    if (productFirstImageUpsertRequest is null)
                    {
                        productFirstImageUpsertRequest = new()
                        {
                            ProductId = productId,
                            ImageContentType = request.ImageContentType,
                            ImageData = request.ImageData,
                            HtmlData = request.HtmlData,
                        };

                        int imageRequestIndex = upsertAllRequest.ImageRequests.IndexOf(imageUpsertData.UpsertRequest);

                        productFirstImageRequestTraceData = GetImageRequestValidationErrorPath(imageUpsertData.UpsertRequest, imageRequestIndex);
                    }

                    OneOf<int, ValidationResult, UnexpectedFailureResult> imageCreateResult
                        = await _productImageCrudService.InsertInAllImagesAsync(request);

                    if (!imageCreateResult.IsT0)
                    {
                        return imageCreateResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                            id => new UnexpectedFailureResult(),
                            validationResult =>
                            {
                                TransformValidationResultErrorPathsForImageRequests(validationResult, imageUpsertData.UpsertRequest, upsertAllRequest.ImageRequests);

                                LogValidationResult(validationResult, productId, "creating image");

                                return validationResult;
                            },
                            unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "creating image"));
                    }

                    imageIdsForRequests.Add(imageUpsertData.UpsertRequest, imageCreateResult.AsT0);
                }
                else if (imageCrudOptions.IsT1)
                {
                    ServiceProductImageUpdateRequest request = imageCrudOptions.AsT1;

                    if (imageUpsertData.ReplaceRequestHtmlWithPropertyHtml)
                    {
                        request.HtmlData = productHtml;
                    }

                    ProductImage? existingProductImage = allExistingProductImages.Find(x => x.Id == request.Id);

                    if (existingProductImage is null) return new UnexpectedFailureResult();

                    if (productFirstImageUpsertRequest is null)
                    {
                        productFirstImageUpsertRequest = new()
                        {
                            ProductId = productId,
                            ImageContentType = request.ImageContentType,
                            ImageData = request.ImageData,
                            HtmlData = request.HtmlData,
                        };

                        int imageRequestIndex = upsertAllRequest.ImageRequests.IndexOf(imageUpsertData.UpsertRequest);

                        productFirstImageRequestTraceData = GetImageRequestValidationErrorPath(imageUpsertData.UpsertRequest, imageRequestIndex);
                    }

                    if (IsRequestEqualToExistingData(request, existingProductImage))
                    {
                        continue;
                    }

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> imageUpdateResult
                        = await _productImageCrudService.UpdateInAllImagesAsync(request);

                    if (!imageUpdateResult.IsT0)
                    {
                        return imageUpdateResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                            success => new UnexpectedFailureResult(),
                            validationResult =>
                            {
                                TransformValidationResultErrorPathsForImageRequests(validationResult, imageUpsertData.UpsertRequest, upsertAllRequest.ImageRequests);

                                LogValidationResult(validationResult, productId, "updating image");

                                return validationResult;
                            },
                            unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "updating image"));
                    }
                }
                else if (imageCrudOptions.IsT2)
                {
                    ProductImageUpsertRequest request = imageCrudOptions.AsT2;

                    if (imageUpsertData.ReplaceRequestHtmlWithPropertyHtml)
                    {
                        request.HtmlData = productHtml;
                    }

                    int? id = request.ExistingImageId;

                    if (productFirstImageUpsertRequest is null)
                    {
                        productFirstImageUpsertRequest ??= new()
                        {
                            ProductId = productId,
                            ImageContentType = request.ImageContentType,
                            ImageData = request.ImageData,
                            HtmlData = request.HtmlData,
                        };

                        int imageRequestIndex = upsertAllRequest.ImageRequests.IndexOf(imageUpsertData.UpsertRequest);

                        productFirstImageRequestTraceData = GetImageRequestValidationErrorPath(imageUpsertData.UpsertRequest, imageRequestIndex);
                    }

                    if (request.ExistingImageId is not null)
                    {
                        ProductImage? existingProductImage = allExistingProductImages.Find(x => x.Id == request.ExistingImageId);

                        if (existingProductImage is null) return new UnexpectedFailureResult();

                        if (IsRequestEqualToExistingData(request, existingProductImage))
                        {
                            imageIdsForRequests.Add(imageUpsertData.UpsertRequest, request.ExistingImageId.Value);

                            continue;
                        }
                    }

                    OneOf<int, ValidationResult, UnexpectedFailureResult> imageUpsertResult = await _productImageCrudService.UpsertInAllImagesAsync(request);

                    if (!imageUpsertResult.IsT0)
                    {
                        return imageUpsertResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                            id => new UnexpectedFailureResult(),
                            validationResult =>
                            {
                                TransformValidationResultErrorPathsForImageRequests(validationResult, imageUpsertData.UpsertRequest, upsertAllRequest.ImageRequests);

                                LogValidationResult(validationResult, productId, "upserting image");

                                return validationResult;
                            },
                            unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "upserting image"));
                    }

                    imageIdsForRequests.Add(imageUpsertData.UpsertRequest, imageUpsertResult.AsT0);
                }
            }

            if (productFirstImageUpsertRequest is not null)
            {
                ProductImage? existingFirstImage = await _productImageCrudService.GetByProductIdInFirstImagesAsync(productId);

                if (existingFirstImage is null || !IsRequestEqualToExistingData(productFirstImageUpsertRequest, existingFirstImage))
                {
                    OneOf<Success, ValidationResult, UnexpectedFailureResult> productFirstImageUpsertResult
                        = await _productImageCrudService.UpsertInFirstImagesAsync(productFirstImageUpsertRequest);

                    if (!productFirstImageUpsertResult.IsT0)
                    {
                        return productFirstImageUpsertResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                            success => success,
                            validationResult =>
                            {
                                PrefixValidationResultErrorPaths(validationResult, productFirstImageRequestTraceData!);

                                return validationResult;
                            },
                            unexpectedFailureResult => unexpectedFailureResult);
                    }
                }
            }
            else
            {
                //ProductImage? oldProductFirstImage = await _productImageCrudService.GetByProductIdInFirstImagesAsync(productId);

                //if (oldProductFirstImage is null) return new Success();

                //bool isFirstImageDeleted = await _productImageCrudService.DeleteInFirstImagesByProductIdAsync(productId);

                //if (!isFirstImageDeleted) return new UnexpectedFailureResult();
            }

            imagesTransactionScope.Complete();
        }


        //using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);
        //using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        foreach (PromotionProductFileInfo oldPromotionProductFileToBeRemoved in oldPromotionProductFileInfos)
        {
            bool promotionProductFileDeleteResult = await _promotionProductFileInfoService.DeleteAsync(oldPromotionProductFileToBeRemoved.Id);

            if (!promotionProductFileDeleteResult)
            {
                UnexpectedFailureResult result = new();

                return LogUnexpectedFailureResult(result, productId, $"deleting promotion product file (ID: {oldPromotionProductFileToBeRemoved.Id})");
            }
        }

        foreach (ProductImageFileData oldImageFileToBeRemoved in oldProductImageFileInfos)
        {
            OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> imageFileDeleteResult
                = await _productImageFileService.DeleteFileAsync(oldImageFileToBeRemoved.Id, upsertUserName);

            if (!imageFileDeleteResult.IsT0)
            {
                UnexpectedFailureResult result = new();

                return LogUnexpectedFailureResult(result, productId, $"deleting product image file (ID: {oldImageFileToBeRemoved.Id})");
            }
        }

        for (int i = 0; i < upsertAllRequest.ImageRequests.Count; i++)
        {
            ProductImageAndPromotionFileUpsertRequest upsertRequest = upsertAllRequest.ImageRequests[i];

            if (upsertRequest.Request.IsT0)
            {
                ProductImageWithFileForProductUpsertRequest request = upsertRequest.Request.AsT0;

                if (request.FileUpsertRequest is null) continue;

                int imageId = imageIdsForRequests[upsertRequest];

                string fileExtensionWithDot = Path.GetExtension(request.FileExtension);

                OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertFileResult
                    = await UpsertImageFileForImageAsync(
                        productId,
                        imageId,
                        request.ImageData,
                        request.FileUpsertRequest.Active,
                        request.FileUpsertRequest.CustomDisplayOrder,
                        fileExtensionWithDot,
                        upsertUserName);

                if (!upsertFileResult.IsT0)
                {
                    return upsertFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                        success => new UnexpectedFailureResult(),
                        validationResult =>
                        {
                            TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, upsertAllRequest.ImageRequests);

                            LogValidationResult(validationResult, productId, "upserting image file");

                            return validationResult;
                        },
                        fileSaveFailureResult => LogFileSaveFailureResult(fileSaveFailureResult, productId, "upserting image file"),
                        fileDoesntExistResult => LogFileDoesntExistResult(fileDoesntExistResult, productId, "upserting image file"),
                        fileAlreadyExistsResult => LogFileAlreadyExistsResult(fileAlreadyExistsResult, productId, "upserting image file"),
                        unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "upserting image file"));
                }
            }
            else if (upsertRequest.Request.IsT1)
            {
                ProductImageFileForProductCreateRequest request = upsertRequest.Request.AsT1;

                ProductImageFileCreateRequest requestInner = new()
                {
                    ProductId = productId,
                    ImageId = null,
                    Active = request.Active,
                    CustomDisplayOrder = request.CustomDisplayOrder,
                    FileData = request.FileData,
                    CreateUserName = upsertUserName,
                };

                OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createFileResult
                    = await _productImageFileService.InsertFileAsync(requestInner);

                if (!createFileResult.IsT0)
                {
                    return createFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                        success => new UnexpectedFailureResult(),
                        validationResult =>
                        {
                            TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, upsertAllRequest.ImageRequests);

                            LogValidationResult(validationResult, productId, "creating image file");

                            return validationResult;
                        },
                        fileSaveFailureResult => LogFileSaveFailureResult(fileSaveFailureResult, productId, "creating image file"),
                        fileAlreadyExistsResult => LogFileAlreadyExistsResult(fileAlreadyExistsResult, productId, "creating image file"),
                        unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "creating image file"));
                }
            }
            else if (upsertRequest.Request.IsT2)
            {
                ProductImageFileForProductUpdateRequest request = upsertRequest.Request.AsT2;

                int? newDisplayOrder = null;

                if (request.UpdateDisplayOrderRequest.IsT0)
                {
                    newDisplayOrder = request.UpdateDisplayOrderRequest.AsT0;
                }

                ProductImageFileUpdateFileDataRequest? updateFileDataRequest = null;

                if (request.UpdateFileDataRequest.IsT0)
                {
                    updateFileDataRequest = new() { FileData = request.UpdateFileDataRequest.AsT0 };
                }

                ProductImageFileUpdateRequest productImageFileUpdateRequest = new()
                {
                    Id = request.Id,
                    Active = request.Active,
                    NewDisplayOrder = newDisplayOrder,
                    UpdateFileDataRequest = updateFileDataRequest,
                    UpdateUserName = upsertUserName,
                };

                OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
                    = await _productImageFileService.UpdateFileAsync(productImageFileUpdateRequest);

                if (!updateFileResult.IsT0)
                {
                    updateFileResult.Switch(
                        success => { },
                        validationResult =>
                        {
                            TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, upsertAllRequest.ImageRequests);

                            LogValidationResult(validationResult, productId, "updating image file");
                        },
                        fileSaveFailureResult => LogFileSaveFailureResult(fileSaveFailureResult, productId, "updating image file"),
                        fileDoesntExistResult => LogFileDoesntExistResult(fileDoesntExistResult, productId, "updating image file"),
                        fileAlreadyExistsResult => LogFileAlreadyExistsResult(fileAlreadyExistsResult, productId, "updating image file"),
                        unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "updating image file"));

                    return updateFileResult;
                }
            }
            else
            {
                PromotionProductFileForProductUpsertRequest request = upsertRequest.Request.AsT3;

                PromotionFileInfo? promotionFile = await _promotionFileService.GetByIdAsync(request.PromotionFileInfoId);

                using Stream? promotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(request.PromotionFileInfoId);

                if (promotionFile is null || promotionFileDataStream is null)
                {
                    string itemPrefix = GetImageRequestValidationErrorPath(upsertRequest, i);

                    string fullPropertyPath = $"{itemPrefix}.{nameof(request.PromotionFileInfoId)}";

                    ValidationFailure validationFailure = new(nameof(request.PromotionFileInfoId), _invalidPromotionFileIdErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                string fileExtension = Path.GetExtension(promotionFile.FileName);

                string? contentType = GetContentTypeFromExtension(promotionFile.FileName);

                if (!IsImageContentType(contentType))
                {
                    ValidationFailure validationFailure = new(nameof(PromotionFileInfo.FileName), _promotionFileIsNotAnImageErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                int imageId = imageIdsForRequests[upsertRequest];

                if (request.Id is null)
                {
                    ServicePromotionProductFileInfoCreateRequest promotionProductFileInfoCreateRequest = new()
                    {
                        ProductId = productId,
                        PromotionFileInfoId = request.PromotionFileInfoId,
                        Active = request.Active,
                        ValidFrom = request.ValidFrom,
                        ValidTo = request.ValidTo,
                        ProductImageId = imageId,
                        CreateUserName = upsertUserName,
                    };

                    OneOf<int, ValidationResult, UnexpectedFailureResult> createPromotionProductFileResult
                        = await _promotionProductFileInfoService.InsertAsync(promotionProductFileInfoCreateRequest);

                    if (!createPromotionProductFileResult.IsT0)
                    {
                        return createPromotionProductFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                            id => new UnexpectedFailureResult(),
                            validationResult =>
                            {
                                TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, upsertAllRequest.ImageRequests);

                                LogValidationResult(validationResult, productId, "creating promotion product file");

                                return validationResult;
                            },
                            unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "creating promotion product file"));
                    }

                    if (request.UpsertInProductImagesRequest is null) continue;

                    string imageFileName = $"{imageId}{fileExtension}";
                    
                    using MemoryStream memoryStreamForInsert = new();

                    await promotionFileDataStream.CopyToAsync(memoryStreamForInsert);

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
                        Active = request.UpsertInProductImagesRequest.ImageFileUpsertRequest.Active,
                        CustomDisplayOrder = request.UpsertInProductImagesRequest.ImageFileUpsertRequest.CustomDisplayOrder,
                        CreateUserName = upsertUserName,
                    };

                    OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
                        = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

                    if (!createImageFileResult.IsT0)
                    {
                        return createImageFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                            success => new UnexpectedFailureResult(),
                            validationResult =>
                            {
                                TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, upsertAllRequest.ImageRequests);

                                LogValidationResult(validationResult, productId, "creating image file");

                                return validationResult;
                            },
                            fileSaveFailureResult => LogFileSaveFailureResult(fileSaveFailureResult, productId, "creating image file"),
                            fileAlreadyExistsResult => LogFileAlreadyExistsResult(fileAlreadyExistsResult, productId, "creating image file"),
                            unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "creating image file"));
                    }

                    continue;
                }

                ServicePromotionProductFileInfoUpdateRequest promotionProductFileInfoUpdateRequest = new()
                {
                    Id = request.Id.Value,
                    NewPromotionFileInfoId = request.PromotionFileInfoId,
                    ValidFrom = request.ValidFrom,
                    ValidTo = request.ValidTo,
                    Active = request.Active,
                    ProductImageId = imageId,
                    UpdateUserName = upsertUserName,
                };

                OneOf<Success, NotFound, ValidationResult> updatePromotionProductFileResult
                    = await _promotionProductFileInfoService.UpdateAsync(promotionProductFileInfoUpdateRequest);

                if (!updatePromotionProductFileResult.IsT0)
                {
                    return updatePromotionProductFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                        success => success,
                        notFound =>
                        {
                            LogNotFound(notFound, promotionProductFileInfoUpdateRequest.Id.ToString(), productId, "updating promotion file");

                            return new UnexpectedFailureResult();
                        },
                        validationResult =>
                        {
                            TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, upsertAllRequest.ImageRequests);

                            LogValidationResult(validationResult, productId, "updating promotion file");

                            return validationResult;
                        });
                }

                if (request.UpsertInProductImagesRequest is null) continue;

                using MemoryStream memoryStream = new();

                await promotionFileDataStream.CopyToAsync(memoryStream);

                byte[] fileData = memoryStream.ToArray();

                OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertFileResult
                    = await UpsertImageFileForImageAsync(
                        productId,
                        imageId,
                        fileData,
                        request.UpsertInProductImagesRequest.ImageFileUpsertRequest.Active,
                        request.UpsertInProductImagesRequest.ImageFileUpsertRequest.CustomDisplayOrder,
                        fileExtension,
                        upsertUserName);

                if (!upsertFileResult.IsT0)
                {
                    return upsertFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                        success => new UnexpectedFailureResult(),
                        validationResult =>
                        {
                            TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, upsertAllRequest.ImageRequests);

                            LogValidationResult(validationResult, productId, "upserting image file");

                            return validationResult;
                        },
                        fileSaveFailureResult => LogFileSaveFailureResult(fileSaveFailureResult, productId, "upserting image file"),
                        fileDoesntExistResult => LogFileDoesntExistResult(fileDoesntExistResult, productId, "upserting image file"),
                        fileAlreadyExistsResult => LogFileAlreadyExistsResult(fileAlreadyExistsResult, productId, "upserting image file"),
                        unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "upserting image file"));
                }
            }
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
           = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
               productId, ProductNewStatus.WorkInProgress, upsertUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                statusId => new UnexpectedFailureResult(),
                validationResult => LogValidationResult(validationResult, productId, "upserting product status"),
                unexpectedFailureResult => LogUnexpectedFailureResult(unexpectedFailureResult, productId, "upserting product status"));
        }

        //localDBTransactionScope.Complete();

        //replicationDBTransactionScope.Complete();

        transactionScope.Complete();

        return new Success();
    }

    private static bool IsRequestEqualToExistingData(ServiceProductImageUpdateRequest request, ProductImage existingProductImage)
    {
        if (existingProductImage.Id != request.Id
            || existingProductImage.ImageContentType != request.ImageContentType
            || existingProductImage.HtmlData != request.HtmlData)
        {
            return false;
        }

        if (existingProductImage.ImageData is null != request.ImageData is null) return false;

        if (existingProductImage.ImageData is null) return true;

        return existingProductImage.ImageData.SequenceEqual(request.ImageData!);
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

        if (existingProductImage.ImageData is null) return true;

        return existingProductImage.ImageData.AsSpan().SequenceEqual(request.ImageData!.AsSpan());
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

        return existingProductImage.ImageData.AsSpan().SequenceEqual(request.ImageData!.AsSpan());
    }

    private async Task<OneOf<Yes, ValidationResult, UnexpectedFailureResult>> ValidateImageAndPromotionFileDataAsync(
        ProductRelatedItemsFullSaveRequest updateAllRequest)
    {
        List<ProductImageData> images = await _productImageCrudService.GetAllInProductWithoutFileDataAsync(updateAllRequest.ProductId);
        List<PromotionProductFileInfo> promotionProductFiles = await _promotionProductFileInfoService.GetAllForProductAsync(updateAllRequest.ProductId);

        foreach (ProductImageAndPromotionFileUpsertRequest upsertRequest in updateAllRequest.ImageRequests)
        {
            if (upsertRequest.Request.IsT0)
            {
                ProductImageWithFileForProductUpsertRequest request = upsertRequest.Request.AsT0;

                if (request.ExistingImageId is not null && !images.Exists(x => x.Id == request.ExistingImageId))
                {
                    return new UnexpectedFailureResult();
                }
            }
            else if (upsertRequest.Request.IsT3)
            {
                PromotionProductFileForProductUpsertRequest request = upsertRequest.Request.AsT3;

                if (request.Id is not null && !promotionProductFiles.Exists(x => x.Id == request.Id))
                {
                    return new UnexpectedFailureResult();
                }
            }
        }

        return new Yes();
    }

    private async Task<OneOf<Yes, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> ValidateFileDataAsync(
        ProductRelatedItemsFullSaveRequest updateAllRequest)
    {
        List<int?> existingDisplayOrders = new();

        for (int i = 0; i < updateAllRequest.ImageRequests.Count; i++)
        {
            ProductImageAndPromotionFileUpsertRequest request = updateAllRequest.ImageRequests[i];

            int? customDisplayOrder = request.Request.Match(
                x => x.FileUpsertRequest?.CustomDisplayOrder,
                x => x.CustomDisplayOrder,
                x => x.UpdateDisplayOrderRequest.IsT0 ? x.UpdateDisplayOrderRequest.AsT0 : null,
                x => x.UpsertInProductImagesRequest?.ImageFileUpsertRequest?.CustomDisplayOrder);

            if (customDisplayOrder is null) continue;

            if (existingDisplayOrders.Contains(customDisplayOrder.Value))
            {
                string propertyPathFromItem = request.Request.Match(
                    x =>
                    {
                        const string fileUpsertRequestName = nameof(x.FileUpsertRequest);
                        const string fileUpsertRequestCustomDisplayOrderName = nameof(x.FileUpsertRequest.CustomDisplayOrder);

                        return $"{fileUpsertRequestName}.{fileUpsertRequestCustomDisplayOrderName}";
                    },
                    x => $"{nameof(x.CustomDisplayOrder)}",
                    x => $"{nameof(x.UpdateDisplayOrderRequest)}",
                    x =>
                    {
                        const string imageUpsertRequestName = nameof(x.UpsertInProductImagesRequest);
                        const string imageFileUpsertRequestName = nameof(x.UpsertInProductImagesRequest.ImageFileUpsertRequest);
                        const string imageFileUpsertCustomDisplayOrderName
                            = nameof(x.UpsertInProductImagesRequest.ImageFileUpsertRequest.CustomDisplayOrder);

                        return $"{imageUpsertRequestName}.{imageFileUpsertRequestName}.{imageFileUpsertCustomDisplayOrderName}";
                    });

                string itemPrefix = GetImageRequestValidationErrorPath(request, i);

                string fullPropertyPath = $"{itemPrefix}.{propertyPathFromItem}";

                ValidationFailure validationFailure = new(fullPropertyPath, _duplicateImageOrderErrorMessage);

                return CreateValidationResultFromErrors(validationFailure);
            }
        }

        List<ProductImageFileData> allFileInfos = await _productImageFileService.GetAllAsync();

        List<string?> imageFileNames = new();

        for (int i = 0; i < updateAllRequest.ImageRequests.Count; i++)
        {
            ProductImageAndPromotionFileUpsertRequest request = updateAllRequest.ImageRequests[i];

            if (request.Request.IsT1)
            {
                ProductImageFileForProductCreateRequest fileCreateRequest = request.Request.AsT1;

                string? customFileName = fileCreateRequest.FileData?.FileName;

                if (customFileName is null) continue;

                if (imageFileNames.Contains(customFileName))
                {
                    string itemPrefix = GetImageRequestValidationErrorPath(request, i);

                    string fullPropertyPath = $"{itemPrefix}.{nameof(fileCreateRequest.FileData)}.{nameof(fileCreateRequest.FileData.FileName)}";

                    ValidationFailure validationFailure = new(fullPropertyPath, _duplicateImageFileNameErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                if (allFileInfos.Exists(x => x.FileName == customFileName))
                {
                    return new FileAlreadyExistsResult() { FileName = customFileName };
                }

                imageFileNames.Add(customFileName);
            }
            else if (request.Request.IsT2)
            {
                ProductImageFileForProductUpdateRequest fileUpdateRequest = request.Request.AsT2;

                OneOf<FileData?, No> updateFileDataRequest = fileUpdateRequest.UpdateFileDataRequest;

                string? customFileName = updateFileDataRequest.IsT0 ? updateFileDataRequest.AsT0!.FileName : null;

                if (customFileName is null) continue;

                if (imageFileNames.Contains(customFileName))
                {
                    string itemPrefix = GetImageRequestValidationErrorPath(request, i);

                    const string updateFileDataRequestName = nameof(fileUpdateRequest.UpdateFileDataRequest);
                    const string updateFileDataRequestAsT0Name = nameof(fileUpdateRequest.UpdateFileDataRequest.AsT0);
                    const string updateFileDataRequestFileNameName = nameof(fileUpdateRequest.UpdateFileDataRequest.AsT0.FileName);

                    string fullPropertyPath = $"{itemPrefix}.{updateFileDataRequestName}.{updateFileDataRequestAsT0Name}.{updateFileDataRequestFileNameName}";

                    ValidationFailure validationFailure = new(fullPropertyPath, _duplicateImageFileNameErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                ProductImageFileData? productImageFileData = allFileInfos.FirstOrDefault(x => x.FileName == customFileName);

                if (productImageFileData is null)
                {
                    return new FileDoesntExistResult() { FileName = customFileName };
                }
                else if (productImageFileData.ProductId != updateAllRequest.ProductId)
                {
                    return new FileAlreadyExistsResult() { FileName = customFileName };
                }

                imageFileNames.Add(customFileName);
            }
        }

        return new Yes();
    }

    private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertImageFileForImageAsync(
        int productId,
        int imageId,
        byte[] imageData,
        bool? active,
        int? customDisplayOrder,
        string fileExtensionWithDot,
        string upsertUserName)
    {
        string? newImageFileName = $"{imageId}{fileExtensionWithDot}";

        ProductImageFileData? fileNameInfo = await _productImageFileService.GetByProductIdAndImageIdAsync(productId, imageId);

        if (fileNameInfo is null)
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

            OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
                = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

            OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResultMapped
                = createImageFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    fileInfoId => new ImageAndFileIdsInfo()
                    {
                        ImageId = imageId,
                        FileInfoId = fileInfoId
                    },
                    validationResult => validationResult,
                    fileSaveFailureResult => fileSaveFailureResult,
                    fileAlreadyExistsResult => fileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult);

            return createImageFileResultMapped;
        }

        int fileInfoId = fileNameInfo.Id;

        int? newDisplayOrder = null;

        if (customDisplayOrder is not null
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

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
            = await _productImageFileService.UpdateFileAsync(productImageFileUpdateRequest);

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResultMapped
            = updateFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => new ImageAndFileIdsInfo()
                {
                    ImageId = imageId,
                    FileInfoId = fileInfoId
                },
                validationResult => validationResult,
                fileSaveFailureResult => fileSaveFailureResult,
                fileDoesntExistResult => fileDoesntExistResult,
                fileAlreadyExistsResult => fileAlreadyExistsResult,
                unexpectedFailureResult => unexpectedFailureResult);

        return updateFileResultMapped;
    }

    private static OneOf<PromotionProductFileInfo?, ValidationResult> FindExisting(
        List<PromotionProductFileInfo> existingPromotionProductFileInfos,
        ProductImageAndPromotionFileUpsertRequest request)
    {
        if (request.Request.IsT3)
        {
            int? id = request.Request.AsT3.Id;

            if (id is null) return OneOf<PromotionProductFileInfo?, ValidationResult>.FromT0(null);

            PromotionProductFileInfo? existingPromotionFile
                = existingPromotionProductFileInfos.Find(x => x.Id == id);

            if (existingPromotionFile is null)
            {
                ValidationFailure validationError = new(
                    nameof(PromotionProductFileForProductUpsertRequest.Id),
                    _invalidPromotionProductFileIdErrorMessage);

                return CreateValidationResultFromErrors(validationError);
            }

            return existingPromotionFile;
        }

        return OneOf<PromotionProductFileInfo?, ValidationResult>.FromT0(null);
    }

    private static OneOf<ProductImageFileData?, ValidationResult, UnexpectedFailureResult> FindExisting(
        List<ProductImageFileData> existingProductImageFileInfos,
        ProductImageAndPromotionFileUpsertRequest request,
        PromotionProductFileInfo? existingPromotionFile)
    {
        return request.Request.Match<OneOf<ProductImageFileData?, ValidationResult, UnexpectedFailureResult>>(
            imageWithFileUpsertRequest =>
            {
                return existingProductImageFileInfos.FirstOrDefault(
                    x => x.ImageId == imageWithFileUpsertRequest.ExistingImageId);
            },
            imageFileCreateRequest => OneOf<ProductImageFileData?, ValidationResult, UnexpectedFailureResult>.FromT0(null),
            imageFileUpdateRequest =>
            {
                ProductImageFileData? existingImageFile = existingProductImageFileInfos.FirstOrDefault(
                    x => x.Id == imageFileUpdateRequest.Id);

                if (existingImageFile is null)
                {
                    ValidationFailure validationError = new(
                        nameof(ProductImageFileForProductUpdateRequest.Id),
                        _invalidImageFileIdErrorMessage);

                    return CreateValidationResultFromErrors(validationError);
                }

                return existingImageFile;
            },
            promotionProductFileUpsertRequest =>
            {
                if (existingPromotionFile?.ProductImageId is null)
                {
                    return OneOf<ProductImageFileData?, ValidationResult, UnexpectedFailureResult>.FromT0(null);
                }

                ProductImageFileData? existingImageFile = existingProductImageFileInfos.FirstOrDefault(
                    x => x.ImageId == existingPromotionFile.ProductImageId);

                if (existingImageFile is null)
                {
                    return new UnexpectedFailureResult();
                }

                return OneOf<ProductImageFileData?, ValidationResult, UnexpectedFailureResult>.FromT0(null);
            });
    }

    private static OneOf<ProductImage?, ValidationResult, UnexpectedFailureResult> FindExisting(
       List<ProductImage> existingProductImages,
       ProductImageAndPromotionFileUpsertRequest request,
       PromotionProductFileInfo? existingPromotionFile)
    {
        return request.Request.Match(
            imageWithFileUpsertRequest =>
            {
                if (imageWithFileUpsertRequest.ExistingImageId is null)
                {
                    return OneOf<ProductImage?, ValidationResult, UnexpectedFailureResult>.FromT0(null);
                }

                ProductImage? existingImage = existingProductImages.FirstOrDefault(
                    x => x.Id == imageWithFileUpsertRequest.ExistingImageId);

                if (existingImage is null)
                {
                    ValidationFailure imageDoesNotExistError = new(
                        nameof(ProductImageWithFileForProductUpsertRequest.ExistingImageId),
                        _invalidImageIdErrorMessage);

                    return CreateValidationResultFromErrors(imageDoesNotExistError);
                }

                return existingImage;
            },
            imageFileCreateRequest => OneOf<ProductImage?, ValidationResult, UnexpectedFailureResult>.FromT0(null),
            imageFileUpdateRequest => OneOf<ProductImage?, ValidationResult, UnexpectedFailureResult>.FromT0(null),
            promotionProductFileUpsertRequest =>
            {
                if (existingPromotionFile?.ProductImageId is null)
                {
                    return OneOf<ProductImage?, ValidationResult, UnexpectedFailureResult>.FromT0(null);
                }

                ProductImage? existingImage = existingProductImages.FirstOrDefault(
                    x => x.Id == existingPromotionFile.ProductImageId);

                if (existingImage is null)
                {
                    return new UnexpectedFailureResult();
                }

                return existingImage;
            });
    }

    private static List<ProductImageAndPromotionFileUpsertRequest> OrderItemsWithDisplayOrders(
        List<ProductImageAndPromotionFileUpsertRequest> itemsWithDisplayOrders,
        List<PromotionProductFileInfo> existingPromotionProductFileInfos)
    {
        return OrderImageRelatedItems(itemsWithDisplayOrders,
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

    private static void TransformValidationResultErrorPathsForImageRequests(
        ValidationResult validationResult,
        ProductImageAndPromotionFileUpsertRequest upsertRequest,
        List<ProductImageAndPromotionFileUpsertRequest> imageRequests)
    {
        int requestIndex = imageRequests.IndexOf(upsertRequest);

        if (requestIndex < 0) return;

        TransformValidationResultErrorPathsForImageRequests(validationResult, upsertRequest, requestIndex);
    }

    private static void TransformValidationResultErrorPathsForImageRequests(
        ValidationResult validationResult,
        ProductImageAndPromotionFileUpsertRequest upsertRequest,
        int requestIndex)
    {
        string prefix = GetImageRequestValidationErrorPath(upsertRequest, requestIndex);

        TransformValidationResultErrors(validationResult, error =>
        {
            error.PropertyName = prefix + error.PropertyName;
        });
    }

    private static string GetImageRequestValidationErrorPath(ProductImageAndPromotionFileUpsertRequest upsertRequest, int requestIndex)
    {
        string requestType = GetImageRequestTypeInResultErrorPath(upsertRequest);

        string prefix = $"ImageRequests[{requestIndex}].Request.{requestType}.";

        return prefix;
    }

    private static string GetImageRequestTypeInResultErrorPath(ProductImageAndPromotionFileUpsertRequest upsertRequest)
    {
        return upsertRequest.Request.Match(
            x => "AsT0",
            x => "AsT1",
            x => "AsT2",
            x => "AsT3");
    }

    private static void PrefixValidationResultErrorPaths(ValidationResult validationResult, string prefix)
    {
        if (prefix.EndsWith('.'))
        {
            prefix = prefix[..1];
        }

        foreach (ValidationFailure error in validationResult.Errors)
        {
            error.PropertyName = prefix + '.' + error.PropertyName;
        }
    }

    private static void TransformValidationResultErrors(ValidationResult validationResult, Action<ValidationFailure> transformAction)
    {
        foreach (ValidationFailure error in validationResult.Errors)
        {
            transformAction(error);
        }
    }

    private NotFound LogNotFound(NotFound notFound, string searchedItemId, int productId, string operation)
    {
        _logger?.LogCritical("Item (ID: {searchedItemId}) not found when {operation} for product ID {ProductId}", searchedItemId, operation, productId);

        return notFound;
    }

    private ValidationResult LogValidationResult(ValidationResult validationResult, int productId, string operation)
    {
        if (_logger is not null)
        {
            string json = JsonSerializer.Serialize(validationResult.Errors);

            _logger.LogCritical("Validation error when {operation} images for product ID {ProductId},\n Full Messages: {ValidationResult}",
                operation, productId, json);
        }

        return validationResult;
    }

    private FileSaveFailureResult LogFileSaveFailureResult(FileSaveFailureResult fileSaveFailureResult, int productId, string operation)
    {
        if (_logger is null)
        {
            return fileSaveFailureResult;
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

        return fileSaveFailureResult;
    }

    private FileDoesntExistResult LogFileDoesntExistResult(FileDoesntExistResult fileDoesntExistResult, int productId, string operation)
    {
        _logger?.LogCritical("File doesn't exist when {operation} for product ID {ProductId}, File Name: {FileName}",
            operation, productId, fileDoesntExistResult.FileName);

        return fileDoesntExistResult;
    }

    private FileAlreadyExistsResult LogFileAlreadyExistsResult(FileAlreadyExistsResult fileAlreadyExistsResult, int productId, string operation)
    {
        _logger?.LogCritical("File already exists when {operation} for product ID {ProductId}, File Name: {FileName}",
            operation, productId, fileAlreadyExistsResult.FileName);

        return fileAlreadyExistsResult;
    }

    private UnexpectedFailureResult LogUnexpectedFailureResult(UnexpectedFailureResult unexpectedFailureResult, int productId, string operation)
    {
        _logger?.LogCritical("Unexpected failure when {operation} for product ID {ProductId}", operation, productId);

        return unexpectedFailureResult;
    }
}