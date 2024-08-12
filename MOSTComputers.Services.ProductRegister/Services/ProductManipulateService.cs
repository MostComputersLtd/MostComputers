using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.Services;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using System.Transactions;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Utils.ProductImageFileNameUtils;

using static MOSTComputers.Services.ProductImageFileManagement.Utils.ProductImageFileManagementUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageAndFileNameRelationsUtils;
using static MOSTComputers.Utils.OneOf.OneOfExtensions;
using SixLabors.ImageSharp;
using MOSTComputers.Utils.OneOf;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;

namespace MOSTComputers.Services.ProductRegister.Services;
internal class ProductManipulateService : IProductManipulateService
{
    public ProductManipulateService(
        IProductService productService,
        IProductPropertyService productPropertyService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductWorkStatusesService productWorkStatusesService,
        IProductImageFileManagementService productImageFileManagementService,
        IProductHtmlService productHtmlService,
        ITransactionExecuteService transactionExecuteService,
        IValidator<ProductCreateRequest>? createRequestValidator = null,
        IValidator<ProductUpdateRequest>? updateRequestValidator = null)
    {
        _productService = productService;
        _productPropertyService = productPropertyService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productWorkStatusesService = productWorkStatusesService;
        _productImageFileManagementService = productImageFileManagementService;
        _productHtmlService = productHtmlService;
        _transactionExecuteService = transactionExecuteService;

        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IProductService _productService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IProductImageFileManagementService _productImageFileManagementService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly IValidator<ProductCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductUpdateRequest>? _updateRequestValidator;

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        InsertProductWithImagesOnlyInDirectoryAsync(ProductCreateWithoutImagesInDatabaseRequest productWithoutImagesInDBCreateRequest)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => InsertProductFullWithImagesOnlyInDirectoryInternalAsync(productWithoutImagesInDBCreateRequest),
            result => result.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false,
                directoryNotFoundResult => false,
                fileDoesntExistResult => false));
    }

    private async Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        InsertProductFullWithImagesOnlyInDirectoryInternalAsync(ProductCreateWithoutImagesInDatabaseRequest productWithoutImagesInDBCreateRequest)
    {
        ProductCreateRequest productCreateRequest = MapProductToCreateRequestWithoutImages(productWithoutImagesInDBCreateRequest);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(productCreateRequest);

        int productId = -1;

        bool isProductInsertSuccessful = productInsertResult.Match(
            id =>
            {
                productId = id;

                return true;
            },
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isProductInsertSuccessful
            || productWithoutImagesInDBCreateRequest.ImagesAndFileNames is null)
        {
            return productInsertResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                id => (int)id,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        foreach (ImageAndImageFileNameRelation imageAndFileNameRelation in productWithoutImagesInDBCreateRequest.ImagesAndFileNames)
        {
            if (imageAndFileNameRelation.ProductImageFileNameInfo?.FileName is null
                || imageAndFileNameRelation.ProductImage?.ImageData is null) continue;

            string fileExtension = GetFileExtensionWithoutDot(imageAndFileNameRelation.ProductImageFileNameInfo.FileName);

            AllowedImageFileType? allowedImageFileType = GetAllowedImageFileTypeFromFileExtension(fileExtension);

            if (allowedImageFileType is null) continue;

            OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> imageInsertInDirectoryResult
                = await SaveImageInDirectoryBasedOnDataAsync(productId, productWithoutImagesInDBCreateRequest.ImagesAndFileNames, imageAndFileNameRelation);

            bool isImageInsertInDirectorySuccessful = imageInsertInDirectoryResult.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false,
                directoryNotFoundResult => false,
                fileDoesntExistResult => false);

            if (!isImageInsertInDirectorySuccessful)
            {
                return imageInsertInDirectoryResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                    success => productId,
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult,
                    directoryNotFoundResult => directoryNotFoundResult,
                    fileDoesntExistResult => fileDoesntExistResult);
            }
        }

        return productId;
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        UpdateProductWithoutSavingImagesInDBAsync(ProductUpdateWithoutImagesInDatabaseRequest productUpdateWithoutImagesInDBRequest)
    {
        int productId = productUpdateWithoutImagesInDBRequest.Id;

        Product? oldProductData = _productService.GetProductFull(productId);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        if (oldProductData is null
            || productWorkStatuses is null
            || productWorkStatuses.ProductNewStatus == ProductNewStatusEnum.New
            || productWorkStatuses.ProductXmlStatus == ProductXmlStatusEnum.NotReady)
        {
            List<ValidationFailure> validationFailuresLocal = new()
            {
                new(nameof(productUpdateWithoutImagesInDBRequest), "Not ready for update")
            };

            return new ValidationResult(validationFailuresLocal);
        }

        List<ImageAndImageFileNameRelation> imagesAndFileNameInfosRelated = productUpdateWithoutImagesInDBRequest.ImagesAndFileNames
            ?? GetImageDictionaryFromImagesAndImageFileInfos(
                oldProductData.Images?.ToList(),
                oldProductData.ImageFileNames?.ToList());

        productUpdateWithoutImagesInDBRequest.Properties ??= oldProductData.Properties.ToList();

        ProductUpdateRequest productUpdateRequest = MapToUpdateRequestWithoutImagesAndProps(productUpdateWithoutImagesInDBRequest);

        OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> saveProductResult
            = await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                () => UpdateProductWithoutSavingImagesInDBInternalAsync(productUpdateWithoutImagesInDBRequest, productUpdateRequest, oldProductData),
                result => result.IsT0);

        return saveProductResult;
    }

    private static ProductCreateRequest MapProductToCreateRequestWithoutImages(ProductCreateWithoutImagesInDatabaseRequest productCreateWithoutImagesInDBRequest)
    {
        return new()
        {
            Name = productCreateWithoutImagesInDBRequest.Name,
            AdditionalWarrantyPrice = productCreateWithoutImagesInDBRequest.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productCreateWithoutImagesInDBRequest.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productCreateWithoutImagesInDBRequest.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productCreateWithoutImagesInDBRequest.StandardWarrantyTermMonths,
            DisplayOrder = productCreateWithoutImagesInDBRequest.DisplayOrder,
            Status = productCreateWithoutImagesInDBRequest.Status,
            PlShow = productCreateWithoutImagesInDBRequest.PlShow,
            DisplayPrice = productCreateWithoutImagesInDBRequest.DisplayPrice,
            Currency = productCreateWithoutImagesInDBRequest.Currency,
            RowGuid = productCreateWithoutImagesInDBRequest.RowGuid,
            PromotionId = productCreateWithoutImagesInDBRequest.PromotionId,
            PromRid = productCreateWithoutImagesInDBRequest.PromRid,
            PromotionPictureId = productCreateWithoutImagesInDBRequest.PromotionPictureId,
            PromotionExpireDate = productCreateWithoutImagesInDBRequest.PromotionExpireDate,
            AlertPictureId = productCreateWithoutImagesInDBRequest.AlertPictureId,
            AlertExpireDate = productCreateWithoutImagesInDBRequest.AlertExpireDate,
            PriceListDescription = productCreateWithoutImagesInDBRequest.PriceListDescription,
            PartNumber1 = productCreateWithoutImagesInDBRequest.PartNumber1,
            PartNumber2 = productCreateWithoutImagesInDBRequest.PartNumber2,
            SearchString = productCreateWithoutImagesInDBRequest.SearchString,
            CategoryId = productCreateWithoutImagesInDBRequest.CategoryId,
            ManifacturerId = productCreateWithoutImagesInDBRequest.ManifacturerId,
            SubCategoryId = productCreateWithoutImagesInDBRequest.SubCategoryId,
            Properties = productCreateWithoutImagesInDBRequest.Properties?
                .Select(property => new CurrentProductPropertyCreateRequest()
                {
                    ProductCharacteristicId = property.ProductCharacteristicId,
                    Value = property.Value,
                    DisplayOrder = property.DisplayOrder,
                    XmlPlacement = property.XmlPlacement,
                }).ToList(),

            Images = new(),
            ImageFileNames = new(),
        };
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateProductWithoutSavingImagesInDBInternalAsync(
        ProductUpdateWithoutImagesInDatabaseRequest productUpdateWithoutImagesInDbRequest,
        ProductUpdateRequest productUpdateRequest,
        Product oldProduct)
    {
        Product productFromRequest = MapToProduct(productUpdateWithoutImagesInDbRequest);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyUpdateResult
            = UpdateProperties(productFromRequest, oldProduct, oldProduct.Id, productUpdateRequest);

        bool isPropertyUpdateSuccessful = propertyUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isPropertyUpdateSuccessful)
        {
            return propertyUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        bool isProductUpdateSuccessful = productUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isProductUpdateSuccessful)
        {
            return productUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
        }

        if (productUpdateWithoutImagesInDbRequest.ImagesAndFileNames is null)
        {
            List<ValidationFailure> validationFailuresLocal = new()
            {
                new(nameof(productUpdateWithoutImagesInDbRequest.ImagesAndFileNames), "Must not be null")
            };

            return new ValidationResult(validationFailuresLocal);
        }

        foreach (ImageAndImageFileNameRelation imageAndFileNameRelation in productUpdateWithoutImagesInDbRequest.ImagesAndFileNames)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> saveImageResult
                = await SaveImageInDirectoryBasedOnDataAsync(productUpdateWithoutImagesInDbRequest.Id, productUpdateWithoutImagesInDbRequest.ImagesAndFileNames, imageAndFileNameRelation);

            if (!saveImageResult.IsT0) return saveImageResult;
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> SaveImageInDirectoryBasedOnDataAsync(
        int productId,
        List<ImageAndImageFileNameRelation> imagesAndFileNamesRelated,
        ImageAndImageFileNameRelation imageAndImageFileNameRelation)
    {
        ProductImage? image = imageAndImageFileNameRelation.ProductImage;
        ProductImageFileNameInfo? fileNameInfo = imageAndImageFileNameRelation.ProductImageFileNameInfo;

        if (image is null
            && fileNameInfo is null) return new UnexpectedFailureResult();

        if (fileNameInfo is null)
        {
            if (image!.Id > 0
                && image!.ImageData is not null
                && !string.IsNullOrWhiteSpace(image.ImageContentType))
            {
                return await HandleImageFileNameNullValidCaseAsync(productId, imagesAndFileNamesRelated, image);
            }

            List<ValidationFailure> validationFailures = new();

            if (image!.Id <= 0)
            {
                validationFailures.Add(new(nameof(ProductImage.Id), "Cannot be less than 0"));
            }

            if (image!.ImageData is null)
            {
                validationFailures.Add(new(nameof(ProductImage.ImageData), "Cannot be null"));
            }

            if (string.IsNullOrWhiteSpace(image.ImageContentType))
            {
                validationFailures.Add(new(nameof(ProductImage.ImageContentType), "Cannot be null, empty, or whitespace"));
            }

            return new ValidationResult(validationFailures);
        }
        else if (image is null)
        {
            if (string.IsNullOrWhiteSpace(fileNameInfo.FileName)) return new UnexpectedFailureResult();

            bool doesFileExist = _productImageFileManagementService.CheckIfImageFileExists(fileNameInfo.FileName);

            return doesFileExist ? new Success() : new FileDoesntExistResult() { FileName = fileNameInfo.FileName };
        }

        return await HandleImageAndFileNameInfoNotNullCaseAsync(productId, image, imagesAndFileNamesRelated, fileNameInfo);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> HandleImageFileNameNullValidCaseAsync(
        int productId,
        List<ImageAndImageFileNameRelation> imagesAndFileNamesRelated,
        ProductImage image)
    {
        string? imageFileName = GetImageFileNameFromImageData(image.Id, image.ImageContentType!);

        if (imageFileName is null) return new UnexpectedFailureResult();

        ServiceProductImageFileNameInfoCreateRequest imageFileNameInfoCreateRequest = new()
        {
            ProductId = productId,
            FileName = imageFileName,
            Active = false,
            DisplayOrder = GetLowestUnpopulatedDisplayOrder(imagesAndFileNamesRelated)
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult
            = _productImageFileNameInfoService.Insert(imageFileNameInfoCreateRequest);

        bool isimageFileNameInfoInsertSuccessful = imageFileNameInfoInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isimageFileNameInfoInsertSuccessful)
        {
            return imageFileNameInfoInsertResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
        }

        string localFileExtension = GetFileExtensionWithoutDot(imageFileName);

        AllowedImageFileType? localAllowedImageFileType = GetAllowedImageFileTypeFromFileExtension(localFileExtension);

        if (localAllowedImageFileType is null)
        {
            List<ValidationFailure> validationFailuresLocal = new()
                {
                    new(nameof(ProductImageFileNameInfo.FileName), "Unsupported file type")
                };

            return new ValidationResult(validationFailuresLocal);
        }

        OneOf<Success, DirectoryNotFoundResult> imageFileCreateResult
            = await _productImageFileManagementService.AddOrUpdateImageAsync(image.Id.ToString(), image.ImageData!, localAllowedImageFileType);

        return imageFileCreateResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> HandleImageAndFileNameInfoNotNullCaseAsync(
        int productId,
        ProductImage image,
        List<ImageAndImageFileNameRelation> imagesAndFileNamesRelated,
        ProductImageFileNameInfo fileNameInfo)
    {
        List<ValidationFailure> finalValidationFailures = new();

        if (image!.ImageData is null)
        {
            finalValidationFailures.Add(new(nameof(ProductImage.ImageData), "Cannot be null"));
        }

        if (string.IsNullOrWhiteSpace(image.ImageContentType))
        {
            finalValidationFailures.Add(new(nameof(ProductImage.ImageContentType), "Cannot be null, empty, or whitespace"));
        }

        if (finalValidationFailures.Count > 0)
        {
            return new ValidationResult(finalValidationFailures);
        }

        if (image.Id <= 0)
        {
            fileNameInfo.FileName ??= GetTemporaryIdFromFileNameInfoAndContentType(fileNameInfo, image.ImageContentType!);
        }

        if (fileNameInfo.FileName is null) return new UnexpectedFailureResult();

        string fileName = fileNameInfo.FileName;

        string fileExtension = GetFileExtensionWithoutDot(fileName);

        AllowedImageFileType? allowedImageFileType = GetAllowedImageFileTypeFromFileExtension(fileExtension);

        if (allowedImageFileType is null)
        {
            List<ValidationFailure> validationFailuresLocal = new()
            {
                new(nameof(ProductImageFileNameInfo.FileName), "Unsupported file type")
            };

            return new ValidationResult(validationFailuresLocal);
        }

        OneOf<int, (int, int), False> idOrTemporaryId = GetImageIdOrTempIdFromImageFileName(fileName);

        OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> imageInsertResult
            = await idOrTemporaryId.Match<Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>>(
            async imageId =>
            {
                OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult> addOrUpdateFileNameInfoAndFileResult
                    = await AddOrUpdateFileNameInfoAndImageFileAsync(
                        productId, imagesAndFileNamesRelated, image, fileNameInfo, imageId.ToString(), allowedImageFileType);

                return addOrUpdateFileNameInfoAndFileResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
            },
            async temporaryIdParts =>
            {
                string temporaryFileNameToUseWithoutExtension = $"{temporaryIdParts.Item1}-{temporaryIdParts.Item2}";

                OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult> addOrUpdateFileNameInfoAndFileResult
                    = await AddOrUpdateFileNameInfoAndImageFileAsync(
                        productId, imagesAndFileNamesRelated, image, fileNameInfo, temporaryFileNameToUseWithoutExtension, allowedImageFileType);

                return addOrUpdateFileNameInfoAndFileResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
            },
            async falseResult => await Task.Run(() => new UnexpectedFailureResult()));

        return imageInsertResult;
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult>>
        AddOrUpdateFileNameInfoAndImageFileAsync(
        int productId,
        List<ImageAndImageFileNameRelation> imagesAndFileNamesRelated,
        ProductImage image,
        ProductImageFileNameInfo fileNameInfo,
        string fileNameToUseWithoutExtension,
        AllowedImageFileType allowedImageFileType)
    {
        fileNameInfo.FileName = $"{fileNameToUseWithoutExtension}.{allowedImageFileType.FileExtension}";

        OneOf<Success, ValidationResult, UnexpectedFailureResult> addOrUpdateFileNameInfoResult
            = AddOrUpdateFileNameInfo(productId, imagesAndFileNamesRelated, fileNameInfo);

        bool isimageFileNameInfoInsertSuccessful = addOrUpdateFileNameInfoResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isimageFileNameInfoInsertSuccessful)
        {
            return addOrUpdateFileNameInfoResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult>();
        }

        OneOf<Success, DirectoryNotFoundResult> imageAddOrUpdateResult
            = await _productImageFileManagementService.AddOrUpdateImageAsync(fileNameToUseWithoutExtension, image.ImageData!, allowedImageFileType);

        return imageAddOrUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult>();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> AddOrUpdateFileNameInfo(
        int productId,
        List<ImageAndImageFileNameRelation> imagesAndFileNamesRelated,
        ProductImageFileNameInfo fileNameInfo)
    {
        ProductImageFileNameInfo? savedImageFileInfo = _productImageFileNameInfoService.GetAllInProduct(productId)
            .FirstOrDefault(x => x.FileName == fileNameInfo.FileName);

        if (savedImageFileInfo is null)
        {
            ServiceProductImageFileNameInfoCreateRequest fileNameInfoCreateRequest = new()
            {
                ProductId = productId,
                FileName = fileNameInfo.FileName,
                DisplayOrder = fileNameInfo.DisplayOrder ?? GetLowestUnpopulatedDisplayOrder(imagesAndFileNamesRelated),
                Active = fileNameInfo.Active,
            };

            OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(fileNameInfoCreateRequest);

            bool isimageFileNameInfoInsertSuccessful = fileNameInfoInsertResult.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false);

            return fileNameInfoInsertResult;
        }

        ServiceProductImageFileNameInfoByImageNumberUpdateRequest imageFileNameInfoUpdateRequest = new()
        {
            ProductId = productId,
            FileName = fileNameInfo.FileName,
            ImageNumber = savedImageFileInfo.ImageNumber,
            NewDisplayOrder = null,
            Active = fileNameInfo.Active,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoUpdateResult
            = _productImageFileNameInfoService.Update(imageFileNameInfoUpdateRequest);

        return fileNameInfoUpdateResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult> UpdateProductFull(ProductFullUpdateRequest productFullUpdateRequest)
    {
        if (productFullUpdateRequest is null)
        {
            List<ValidationFailure> validationFailures = new()
            {
                new(nameof(productFullUpdateRequest), "Must not be null")
            };

            return new ValidationResult(validationFailures);
        }

        try
        {
            return _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
                () => UpdateProductInternal(productFullUpdateRequest),
                result => result.Match(
                    success => true,
                    validationResult => false,
                    unexpectedFailureResult => false,
                    notSupportedFileTypeResult => false));
        }
        catch (TransactionException)
        {
            return new UnexpectedFailureResult();
        }
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult> UpdateProductInternal(ProductFullUpdateRequest productFullUpdateRequest)
    {
        if (productFullUpdateRequest is null)
        {
            List<ValidationFailure> validationFailures = new()
            {
                new(nameof(productFullUpdateRequest), "Must not be null")
            };

            return new ValidationResult(validationFailures);
        }

        int productId = productFullUpdateRequest.Id;

        Product? oldProduct = _productService.GetProductFull(productFullUpdateRequest.Id);

        if (oldProduct is null)
        {
            List<ValidationFailure> validationFailures = new()
            {
                new(nameof(oldProduct), "Must not be null")
            };

            return new ValidationResult(validationFailures);
        }

        ProductUpdateRequest productUpdateRequest = MapToUpdateRequestWithoutImagesAndProps(productFullUpdateRequest);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertiesUpdateActionResult = UpdateProperties(
            productFullUpdateRequest, oldProduct, productId, productUpdateRequest);

        if (!propertiesUpdateActionResult.IsT0)
        {
            return propertiesUpdateActionResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imagesUpdateActionResult = UpdateImages(
            productFullUpdateRequest, oldProduct, productId, productUpdateRequest);

        if (!imagesUpdateActionResult.IsT0)
        {
            return imagesUpdateActionResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult> imageFileNamesUpdateActionResult = UpdateImageFileNames(
            productFullUpdateRequest, oldProduct, productId);

        if (!imageFileNamesUpdateActionResult.IsT0)
        {
            return imageFileNamesUpdateActionResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        if (productUpdateResult.Value is not Success)
        {
            return productUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
        }

        Product? updatedProduct = _productService.GetProductFull(productId);

        if (updatedProduct is null) return new UnexpectedFailureResult();

        return UpdateImagesHtmlData(updatedProduct);
    }

    private static ProductUpdateRequest MapToUpdateRequestWithoutImagesAndProps(Product productToUpdate)
    {
        return new()
        {
            Id = productToUpdate.Id,
            Name = productToUpdate.Name,
            AdditionalWarrantyPrice = productToUpdate.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productToUpdate.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productToUpdate.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productToUpdate.StandardWarrantyTermMonths,
            DisplayOrder = productToUpdate.DisplayOrder,
            Status = productToUpdate.Status,
            PlShow = productToUpdate.PlShow,
            DisplayPrice = productToUpdate.Price,
            Currency = productToUpdate.Currency,
            RowGuid = productToUpdate.RowGuid,
            PromotionId = productToUpdate.PromotionId,
            PromRid = productToUpdate.PromRid,
            PromotionPictureId = productToUpdate.PromotionPictureId,
            PromotionExpireDate = productToUpdate.PromotionExpireDate,
            AlertPictureId = productToUpdate.AlertPictureId,
            AlertExpireDate = productToUpdate.AlertExpireDate,
            PriceListDescription = productToUpdate.PriceListDescription,
            PartNumber1 = productToUpdate.PartNumber1,
            PartNumber2 = productToUpdate.PartNumber2,
            SearchString = productToUpdate.SearchString,

            Properties = new(),
            Images = new(),
            ImageFileNames = new(),

            CategoryId = productToUpdate.CategoryId,
            ManifacturerId = productToUpdate.ManifacturerId,
            SubCategoryId = productToUpdate.SubCategoryId,
        };
    }

    private static ProductUpdateRequest MapToUpdateRequestWithoutImagesAndProps(ProductFullUpdateRequest productFullUpdateRequest)
    {
        return new()
        {
            Id = productFullUpdateRequest.Id,
            Name = productFullUpdateRequest.Name,
            AdditionalWarrantyPrice = productFullUpdateRequest.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productFullUpdateRequest.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productFullUpdateRequest.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productFullUpdateRequest.StandardWarrantyTermMonths,
            DisplayOrder = productFullUpdateRequest.DisplayOrder,
            Status = productFullUpdateRequest.Status,
            PlShow = productFullUpdateRequest.PlShow,
            Price1 = productFullUpdateRequest.Price1,
            DisplayPrice = productFullUpdateRequest.DisplayPrice,
            Price3 = productFullUpdateRequest.Price3,
            Currency = productFullUpdateRequest.Currency,
            RowGuid = productFullUpdateRequest.RowGuid,
            PromotionId = productFullUpdateRequest.PromotionId,
            PromRid = productFullUpdateRequest.PromRid,
            PromotionPictureId = productFullUpdateRequest.PromotionPictureId,
            PromotionExpireDate = productFullUpdateRequest.PromotionExpireDate,
            AlertPictureId = productFullUpdateRequest.AlertPictureId,
            AlertExpireDate = productFullUpdateRequest.AlertExpireDate,
            PriceListDescription = productFullUpdateRequest.PriceListDescription,
            PartNumber1 = productFullUpdateRequest.PartNumber1,
            PartNumber2 = productFullUpdateRequest.PartNumber2,
            SearchString = productFullUpdateRequest.SearchString,

            Properties = new(),
            Images = new(),
            ImageFileNames = new(),

            CategoryId = productFullUpdateRequest.CategoryId,
            ManifacturerId = productFullUpdateRequest.ManifacturerId,
            SubCategoryId = productFullUpdateRequest.SubCategoryId,
        };
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateProperties(
        ProductFullUpdateRequest productFullUpdateRequest,
        Product oldFirstProduct,
        int productId,
        ProductUpdateRequest productUpdateRequestToAddPropsTo)
    {
        if (productFullUpdateRequest.Properties is not null
            && productFullUpdateRequest.Properties.Count > 0)
        {
            foreach (ProductProperty newProductProp in productFullUpdateRequest.Properties)
            {
                ProductProperty? oldProductPropForSameCharacteristic = oldFirstProduct.Properties
                    .Find(prop => prop.ProductCharacteristicId == newProductProp.ProductCharacteristicId);

                if (oldProductPropForSameCharacteristic is null)
                {
                    ProductPropertyByCharacteristicIdCreateRequest propCreateRequest = new()
                    {
                        ProductCharacteristicId = newProductProp.ProductCharacteristicId,
                        ProductId = productId,
                        DisplayOrder = newProductProp.DisplayOrder,
                        Value = newProductProp.Value,
                        XmlPlacement = newProductProp.XmlPlacement,
                    };

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult
                        = _productPropertyService.InsertWithCharacteristicId(propCreateRequest);

                    bool isPropertyInsertSuccessful = propertyInsertResult.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isPropertyInsertSuccessful)
                    {
                        return propertyInsertResult;
                    }

                    continue;
                }

                if (oldProductPropForSameCharacteristic.Value == newProductProp.Value
                    && oldProductPropForSameCharacteristic.XmlPlacement == newProductProp.XmlPlacement)
                {
                    oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);

                    continue;
                }

                CurrentProductPropertyUpdateRequest propUpdateRequest = new()
                {
                    ProductCharacteristicId = newProductProp.ProductCharacteristicId,
                    DisplayOrder = newProductProp.DisplayOrder,
                    Value = newProductProp.Value,
                    XmlPlacement = newProductProp.XmlPlacement,
                };

                productUpdateRequestToAddPropsTo.Properties.Add(propUpdateRequest);

                oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);
            }
        }

        foreach (ProductProperty oldPropertyToDelete in oldFirstProduct.Properties)
        {
            if (oldPropertyToDelete.ProductCharacteristicId is null) return new UnexpectedFailureResult();

            bool imageFileNameDeleteSuccess = _productPropertyService.Delete(
                productId, oldPropertyToDelete.ProductCharacteristicId.Value);

            if (!imageFileNameDeleteSuccess) return new UnexpectedFailureResult();
        }

        return new Success();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateProperties(
        Product productToUpdate,
        Product oldFirstProduct,
        int productId,
        ProductUpdateRequest productUpdateRequestToAddPropsTo)
    {
        if (productToUpdate.Properties is not null
            && productToUpdate.Properties.Count > 0)
        {
            foreach (ProductProperty newProductProp in productToUpdate.Properties)
            {
                ProductProperty? oldProductPropForSameCharacteristic = oldFirstProduct.Properties
                    .Find(prop => prop.ProductCharacteristicId == newProductProp.ProductCharacteristicId);

                if (oldProductPropForSameCharacteristic is null)
                {
                    ProductPropertyByCharacteristicIdCreateRequest propCreateRequest = new()
                    {
                        ProductCharacteristicId = newProductProp.ProductCharacteristicId,
                        ProductId = productId,
                        DisplayOrder = newProductProp.DisplayOrder,
                        Value = newProductProp.Value,
                        XmlPlacement = newProductProp.XmlPlacement,
                    };

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult
                        = _productPropertyService.InsertWithCharacteristicId(propCreateRequest);

                    bool isPropertyInsertSuccessful = propertyInsertResult.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isPropertyInsertSuccessful)
                    {
                        return propertyInsertResult;
                    }

                    continue;
                }

                if (oldProductPropForSameCharacteristic.Value == newProductProp.Value
                    && oldProductPropForSameCharacteristic.XmlPlacement == newProductProp.XmlPlacement)
                {
                    oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);

                    continue;
                }

                CurrentProductPropertyUpdateRequest propUpdateRequest = new()
                {
                    ProductCharacteristicId = newProductProp.ProductCharacteristicId,
                    DisplayOrder = newProductProp.DisplayOrder,
                    Value = newProductProp.Value,
                    XmlPlacement = newProductProp.XmlPlacement,
                };

                productUpdateRequestToAddPropsTo.Properties.Add(propUpdateRequest);

                oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);
            }
        }

        foreach (ProductProperty oldPropertyToDelete in oldFirstProduct.Properties)
        {
            if (oldPropertyToDelete.ProductCharacteristicId is null) return new UnexpectedFailureResult();

            bool imageFileNameDeleteSuccess = _productPropertyService.Delete(
                productId, oldPropertyToDelete.ProductCharacteristicId.Value);

            if (!imageFileNameDeleteSuccess) return new UnexpectedFailureResult();
        }

        return new Success();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateImages(
        ProductFullUpdateRequest productFullUpdateRequest,
        Product oldProduct,
        int productId,
        ProductUpdateRequest productUpdateRequestToAddImagesTo)
        //List<(ProductImage productImage, int displayOrder)> ordersForImagesInProduct)
    {
        if (productFullUpdateRequest.ImagesAndFileNames is not null)
        {
            ProductImage? productFirstImage = null;

            for (int i = 0; i < productFullUpdateRequest.ImagesAndFileNames.Count; i++)
            {
                ImageAndImageFileNameRelation imageAndFileNameInfoRelation = productFullUpdateRequest.ImagesAndFileNames[i];

                ProductImage? image = imageAndFileNameInfoRelation.ProductImage;

                if (image is null) continue;

                ProductImage? imageInOldProduct = oldProduct.Images?.Find(
                    img => img.Id == image.Id);

                if (imageInOldProduct is null)
                {
                    ServiceProductImageCreateRequest productImageCreateRequest = new()
                    {
                        ProductId = productId,
                        ImageData = image.ImageData,
                        ImageContentType = image.ImageContentType,
                        HtmlData = image.HtmlData,
                    };

                    OneOf<int, ValidationResult, UnexpectedFailureResult> imageInsertResult
                        = _productImageService.InsertInAllImages(productImageCreateRequest);

                    int imageId = -1;

                    bool isImageInsertSuccessful = imageInsertResult.Match(
                        id =>
                        {
                            imageId = id;

                            return true;
                        },
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isImageInsertSuccessful)
                    {
                        return imageInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                            id => new Success(),
                            validationResult => validationResult,
                            unexpectedFailureResult => unexpectedFailureResult);
                    }

                    image.Id = imageId;

                    if (imageAndFileNameInfoRelation.ProductImageFileNameInfo?.DisplayOrder == 1)
                    {
                        productFirstImage = image;
                    }    

                    //int imageInOrderedListIndex = ordersForImagesInProduct.FindIndex(
                    //    x => x.productImage == image);

                    //if (imageInOrderedListIndex < 0) return new UnexpectedFailureResult();

                    //if (ordersForImagesInProduct[imageInOrderedListIndex].displayOrder == 1)
                    //{
                    //    productFirstImage = image;
                    //}

                    //(ProductImage productImage, int displayOrder) = ordersForImagesInProduct[imageInOrderedListIndex];

                    //productImage.Id = imageId;

                    //ordersForImagesInProduct[imageInOrderedListIndex] = new(productImage, displayOrder);

                    continue;
                }

                if (i == 0
                    || imageAndFileNameInfoRelation.ProductImageFileNameInfo?.DisplayOrder == 1)
                {
                    productFirstImage = image;
                }

                if (CompareByteArrays(image.ImageData, imageInOldProduct.ImageData)
                    && image.ImageContentType == imageInOldProduct.ImageContentType
                    && image.HtmlData == imageInOldProduct.HtmlData)
                {
                    oldProduct.Images?.Remove(imageInOldProduct);

                    continue;
                }

                CurrentProductImageUpdateRequest productImageUpdateRequest = new()
                {
                    ImageData = image.ImageData,
                    ImageContentType = image.ImageContentType,
                    HtmlData = image.HtmlData,
                };

                productUpdateRequestToAddImagesTo.Images ??= new();

                productUpdateRequestToAddImagesTo.Images.Add(productImageUpdateRequest);

                oldProduct.Images?.Remove(imageInOldProduct);
            }

            if (productFirstImage is not null)
            {
                ProductImage? oldProductFirstImage = _productImageService.GetFirstImageForProduct(productId);

                if (oldProductFirstImage is null)
                {
                    OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult = _productImageService.InsertInFirstImages(new()
                    {
                        ProductId = productId,
                        ImageData = productFirstImage.ImageData,
                        ImageContentType = productFirstImage.ImageContentType,
                        HtmlData = productFirstImage.HtmlData,
                    });

                    bool isFirstImageInsertSuccessful = insertFirstImageResult.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isFirstImageInsertSuccessful) return insertFirstImageResult;
                }
                else
                {
                    OneOf<Success, ValidationResult, UnexpectedFailureResult> updateFirstImageResult = _productImageService.UpdateInFirstImages(new()
                    {
                        ProductId = productId,
                        ImageData = productFirstImage.ImageData,
                        ImageContentType = productFirstImage.ImageContentType,
                        HtmlData = productFirstImage.HtmlData,
                    });

                    bool isFirstImageInsertSuccessful = updateFirstImageResult.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isFirstImageInsertSuccessful) return updateFirstImageResult;
                }
            }
        }

        if (oldProduct.Images is not null
            && oldProduct.Images.Count > 0)
        {
            foreach (ProductImage oldImageToBeRemoved in oldProduct.Images)
            {
                bool imageDeleteResult = _productImageService.DeleteInAllImagesById(oldImageToBeRemoved.Id);

                if (!imageDeleteResult) return new UnexpectedFailureResult();
            }
        }

        return new Success();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult> UpdateImageFileNames(
        ProductFullUpdateRequest productFullUpdateRequest,
        Product oldProduct,
        int productId)
    {
        List<ServiceProductImageFileNameInfoCreateRequest> imageFileNameInfoCreateRequests = new();

        if (oldProduct.ImageFileNames is not null
            && oldProduct.ImageFileNames.Count > 0)
        {
            if (productFullUpdateRequest.ImagesAndFileNames is null
                || productFullUpdateRequest.ImagesAndFileNames.Count <= 0)
            {
                foreach (ProductImageFileNameInfo oldProductImageFileName in oldProduct.ImageFileNames)
                {
                    if (oldProductImageFileName.DisplayOrder is null) return new UnexpectedFailureResult();

                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
                        productId, oldProductImageFileName.DisplayOrder.Value);

                    if (!imageFileNameDeleteResult) return new UnexpectedFailureResult();

                    for (int k = 0; k < oldProduct.ImageFileNames.Count; k++)
                    {
                        ProductImageFileNameInfo oldImageFileName = oldProduct.ImageFileNames[k];

                        oldImageFileName.DisplayOrder = k + 1;
                    }
                }
            }
            else
            {
                for (int i = 0; i < oldProduct.ImageFileNames.Count; i++)
                {
                    ProductImageFileNameInfo oldProductImageFileName = oldProduct.ImageFileNames[i];

                    if (oldProductImageFileName.DisplayOrder is null) return new UnexpectedFailureResult();

                    ProductImageFileNameInfo? oldImageFileNameInCurrentProduct = productFullUpdateRequest.ImagesAndFileNames.Find(
                        imgFileNameInfo => oldProductImageFileName.FileName == imgFileNameInfo.ProductImageFileNameInfo?.FileName)?
                        .ProductImageFileNameInfo;

                    if (oldImageFileNameInCurrentProduct is not null) continue;

                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
                        productId, oldProductImageFileName.DisplayOrder.Value);

                    if (!imageFileNameDeleteResult) return new UnexpectedFailureResult();

                    oldProduct.ImageFileNames.RemoveAt(i);

                    i--;

                    for (int k = 0; k < oldProduct.ImageFileNames.Count; k++)
                    {
                        ProductImageFileNameInfo oldImageFileName = oldProduct.ImageFileNames[k];

                        oldImageFileName.DisplayOrder = k + 1;
                    }
                }
            }
        }

        if (productFullUpdateRequest.ImagesAndFileNames is not null)
        {
            int newImageFileNamesCount = 0;

            foreach (ImageAndImageFileNameRelation imageAndFileNameInfoRelation in productFullUpdateRequest.ImagesAndFileNames)
            {
                ProductImageFileNameInfo? imageFileNameInfo = imageAndFileNameInfoRelation.ProductImageFileNameInfo;

                if (imageFileNameInfo is null) continue;

                ProductImageFileNameInfo? imageFileNameOfOldProduct = oldProduct.ImageFileNames?.Find(
                    imgFileNameInfo => imgFileNameInfo.FileName == imageFileNameInfo.FileName);

                if (imageFileNameOfOldProduct is null)
                {
                    ProductImage? imageRelatedToThatFileNameInfoLocal = imageAndFileNameInfoRelation.ProductImage;

                    if (imageRelatedToThatFileNameInfoLocal is null
                        || imageRelatedToThatFileNameInfoLocal.Id <= 0
                        || string.IsNullOrWhiteSpace(imageRelatedToThatFileNameInfoLocal.ImageContentType)) return new UnexpectedFailureResult();

                    if (imageRelatedToThatFileNameInfoLocal is null) return new UnexpectedFailureResult();

                    string newFileName = $"{imageRelatedToThatFileNameInfoLocal.Id}.{GetImageFileExtensionFromContentType(imageRelatedToThatFileNameInfoLocal.ImageContentType)}";

                    ServiceProductImageFileNameInfoCreateRequest imageFileNameCreateRequest = new()
                    {
                        ProductId = productFullUpdateRequest.Id,
                        DisplayOrder = imageFileNameInfo.DisplayOrder,
                        Active = imageFileNameInfo.Active,
                        FileName = newFileName,
                    };

                    imageFileNameInfoCreateRequests.Add(imageFileNameCreateRequest);

                    string fileExtension = GetFileExtensionWithoutDot(newFileName);

                    AllowedImageFileType? allowedImageFileTypeFromFileName = GetAllowedImageFileTypeFromFileExtension(fileExtension);

                    if (allowedImageFileTypeFromFileName is null) return new NotSupportedFileTypeResult() { FileExtension = fileExtension };

                    if (imageFileNameInfo.FileName is not null)
                    {
                        OneOf<Success, ValidationResult, NotSupportedFileTypeResult> renameResult
                            = TryRenameFile(imageFileNameInfo.FileName, newFileName);

                        if (!renameResult.IsT0)
                        {
                            return renameResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
                        }
                    }

                    newImageFileNamesCount++;

                    continue;
                }

                if (imageFileNameInfo.FileName is null
                    || imageFileNameInfo.DisplayOrder is null) continue;

                ProductImage? imageRelatedToThatFileNameInfo = imageAndFileNameInfoRelation.ProductImage;

                if (imageRelatedToThatFileNameInfo is null
                    || imageRelatedToThatFileNameInfo.Id <= 0
                    || string.IsNullOrWhiteSpace(imageRelatedToThatFileNameInfo.ImageContentType)) return new UnexpectedFailureResult();

                string? temporaryId = GetTemporaryIdFromFileNameInfoAndContentType(imageFileNameInfo, imageRelatedToThatFileNameInfo.ImageContentType);

                if (temporaryId is not null)
                {
                    string? fullFileNameFromImageData = GetImageFileNameFromImageData(
                        imageRelatedToThatFileNameInfo.Id, imageRelatedToThatFileNameInfo.ImageContentType);

                    if (fullFileNameFromImageData is null) return new UnexpectedFailureResult();

                    string fileExtension = GetFileExtensionWithoutDot(fullFileNameFromImageData);

                    AllowedImageFileType? allowedImageFileTypeFromFileName = GetAllowedImageFileTypeFromFileExtension(fileExtension);

                    if (allowedImageFileTypeFromFileName is null) return new NotSupportedFileTypeResult() { FileExtension = fileExtension };

                    if (imageFileNameInfo.FileName is not null)
                    {
                        OneOf<Success, ValidationResult, NotSupportedFileTypeResult> renameResult
                            = TryRenameFile(imageFileNameInfo.FileName, fullFileNameFromImageData);

                        if (!renameResult.IsT0)
                        {
                            return renameResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
                        }
                    }

                    imageFileNameInfo.FileName = fullFileNameFromImageData;
                }

                int displayOrder = imageFileNameInfo.DisplayOrder.Value - newImageFileNamesCount;

                bool isFileNameInfoSameAsOldOne = imageFileNameInfo.FileName == imageFileNameOfOldProduct.FileName
                    && displayOrder == imageFileNameOfOldProduct.DisplayOrder
                    && imageFileNameInfo.Active == imageFileNameOfOldProduct.Active;

                if (isFileNameInfoSameAsOldOne) continue;

                ServiceProductImageFileNameInfoByFileNameUpdateRequest imageFileNameUpdateRequest = new()
                {
                    ProductId = productId,
                    NewDisplayOrder = displayOrder,
                    Active = imageFileNameInfo.Active,
                    FileName = imageFileNameOfOldProduct.FileName!,
                    NewFileName = imageFileNameInfo.FileName,
                };

                OneOf<Success, ValidationResult, UnexpectedFailureResult> productImageFileNameUpdateResult
                    = _productImageFileNameInfoService.UpdateByFileName(imageFileNameUpdateRequest);

                bool isImageFileNameUpdateSuccessful = productImageFileNameUpdateResult.Match(
                    id => true,
                    validationResult => false,
                    unexpectedFailureResult => false);

                if (!isImageFileNameUpdateSuccessful)
                {
                    return productImageFileNameUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
                }
            }
        }

        IEnumerable<ServiceProductImageFileNameInfoCreateRequest> orderedImageFileNameInfoCreateRequests = imageFileNameInfoCreateRequests
            .OrderBy(imageFileName => imageFileName.DisplayOrder);

        foreach (ServiceProductImageFileNameInfoCreateRequest fileNameInfoCreateRequest in orderedImageFileNameInfoCreateRequests)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(fileNameInfoCreateRequest);

            bool isImageFileNameInsertSuccessful = fileNameInfoInsertResult.Match(
                    id => true,
                    validationResult => false,
                    unexpectedFailureResult => false);

            if (!isImageFileNameInsertSuccessful)
            {
                return fileNameInfoInsertResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
            }
        }

        return new Success();
    }

    private OneOf<Success, ValidationResult, NotSupportedFileTypeResult> TryRenameFile(string fileName, string newFileName)
    {
        string oldFileExtension = GetFileExtensionWithoutDot(fileName);

        string fileExtension = GetFileExtensionWithoutDot(newFileName);

        if (oldFileExtension != fileExtension)
        {
            List<ValidationFailure> validationFailures = new()
            {
                new(nameof(ProductImageFileNameInfo.FileName), "New and old filename must have the same extension")
            };

            return new ValidationResult(validationFailures);
        }

        AllowedImageFileType? allowedImageFileTypeFromFileName = GetAllowedImageFileTypeFromFileExtension(fileExtension);

        if (allowedImageFileTypeFromFileName is null) return new NotSupportedFileTypeResult() { FileExtension = fileExtension };

        _productImageFileManagementService.RenameImageFile(
            Path.GetFileNameWithoutExtension(fileName), Path.GetFileNameWithoutExtension(newFileName), allowedImageFileTypeFromFileName);

        return new Success();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult> UpdateImagesHtmlData(Product product)
    {
        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProduct(product);

        return getProductHtmlResult.Match(
            htmlData =>
            {
                OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImagesHtmlDataResult
                    = _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductId(product.Id, htmlData);

                return updateImagesHtmlDataResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult>();
            },
            invalidXmlResult => new UnexpectedFailureResult());
    }

    private static bool CompareByteArrays(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        return a.SequenceEqual(b);
    }

    private static ProductUpdateRequest MapToUpdateRequestWithoutImagesAndProps(ProductUpdateWithoutImagesInDatabaseRequest productUpdateRequestWithoutImagesInDB)
    {
        return new()
        {
            Id = productUpdateRequestWithoutImagesInDB.Id,
            Name = productUpdateRequestWithoutImagesInDB.Name,
            AdditionalWarrantyPrice = productUpdateRequestWithoutImagesInDB.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productUpdateRequestWithoutImagesInDB.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productUpdateRequestWithoutImagesInDB.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productUpdateRequestWithoutImagesInDB.StandardWarrantyTermMonths,
            DisplayOrder = productUpdateRequestWithoutImagesInDB.DisplayOrder,
            Status = productUpdateRequestWithoutImagesInDB.Status,
            PlShow = productUpdateRequestWithoutImagesInDB.PlShow,
            Price1 = productUpdateRequestWithoutImagesInDB.Price1,
            DisplayPrice = productUpdateRequestWithoutImagesInDB.DisplayPrice,
            Price3 = productUpdateRequestWithoutImagesInDB.Price3,
            Currency = productUpdateRequestWithoutImagesInDB.Currency,
            RowGuid = productUpdateRequestWithoutImagesInDB.RowGuid,
            PromotionId = productUpdateRequestWithoutImagesInDB.PromotionId,
            PromRid = productUpdateRequestWithoutImagesInDB.PromRid,
            PromotionPictureId = productUpdateRequestWithoutImagesInDB.PromotionPictureId,
            PromotionExpireDate = productUpdateRequestWithoutImagesInDB.PromotionExpireDate,
            AlertPictureId = productUpdateRequestWithoutImagesInDB.AlertPictureId,
            AlertExpireDate = productUpdateRequestWithoutImagesInDB.AlertExpireDate,
            PriceListDescription = productUpdateRequestWithoutImagesInDB.PriceListDescription,
            PartNumber1 = productUpdateRequestWithoutImagesInDB.PartNumber1,
            PartNumber2 = productUpdateRequestWithoutImagesInDB.PartNumber2,
            SearchString = productUpdateRequestWithoutImagesInDB.SearchString,

            Properties = new(),
            Images = new(),
            ImageFileNames = new(),

            CategoryId = productUpdateRequestWithoutImagesInDB.CategoryId,
            ManifacturerId = productUpdateRequestWithoutImagesInDB.ManifacturerId,
            SubCategoryId = productUpdateRequestWithoutImagesInDB.SubCategoryId,
        };
    }

    private static Product MapToProduct(ProductUpdateWithoutImagesInDatabaseRequest productUpdateRequestWithoutImagesInDB)
    {
        return new()
        {
            Id = productUpdateRequestWithoutImagesInDB.Id,
            Name = productUpdateRequestWithoutImagesInDB.Name,
            AdditionalWarrantyPrice = productUpdateRequestWithoutImagesInDB.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productUpdateRequestWithoutImagesInDB.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productUpdateRequestWithoutImagesInDB.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productUpdateRequestWithoutImagesInDB.StandardWarrantyTermMonths,
            DisplayOrder = productUpdateRequestWithoutImagesInDB.DisplayOrder,
            Status = productUpdateRequestWithoutImagesInDB.Status,
            PlShow = productUpdateRequestWithoutImagesInDB.PlShow,
            Price = productUpdateRequestWithoutImagesInDB.DisplayPrice,
            Currency = productUpdateRequestWithoutImagesInDB.Currency,
            RowGuid = productUpdateRequestWithoutImagesInDB.RowGuid,
            PromotionId = productUpdateRequestWithoutImagesInDB.PromotionId,
            PromRid = productUpdateRequestWithoutImagesInDB.PromRid,
            PromotionPictureId = productUpdateRequestWithoutImagesInDB.PromotionPictureId,
            PromotionExpireDate = productUpdateRequestWithoutImagesInDB.PromotionExpireDate,
            AlertPictureId = productUpdateRequestWithoutImagesInDB.AlertPictureId,
            AlertExpireDate = productUpdateRequestWithoutImagesInDB.AlertExpireDate,
            PriceListDescription = productUpdateRequestWithoutImagesInDB.PriceListDescription,
            PartNumber1 = productUpdateRequestWithoutImagesInDB.PartNumber1,
            PartNumber2 = productUpdateRequestWithoutImagesInDB.PartNumber2,
            SearchString = productUpdateRequestWithoutImagesInDB.SearchString,

            Properties = productUpdateRequestWithoutImagesInDB.Properties ?? new(),
            Images = productUpdateRequestWithoutImagesInDB.ImagesAndFileNames?.Select(x => x.ProductImage)
                .Where(image => image is not null)!
                .ToList<ProductImage>(),
            ImageFileNames = productUpdateRequestWithoutImagesInDB.ImagesAndFileNames?.Select(x => x.ProductImageFileNameInfo)
                .Where(fileNameInfo => fileNameInfo is not null)!
                .ToList<ProductImageFileNameInfo>(),

            CategoryId = productUpdateRequestWithoutImagesInDB.CategoryId,
            ManifacturerId = productUpdateRequestWithoutImagesInDB.ManifacturerId,
            SubCategoryId = productUpdateRequestWithoutImagesInDB.SubCategoryId,
        };
    }
}