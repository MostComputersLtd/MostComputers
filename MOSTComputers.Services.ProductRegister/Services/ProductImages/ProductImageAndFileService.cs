using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Services.ProductRegister.Utils.ImageUtils;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;
using static MOSTComputers.Utils.Files.FileExtensionUtils;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages;
internal sealed class ProductImageAndFileService : IProductImageAndFileService
{
    public ProductImageAndFileService(
        IProductImageCrudService productImageCrudService,
        IProductImageFileService productImageFileService,
        IProductToHtmlProductService productToHtmlProductService,
        IProductHtmlService productHtmlService,
        ITransactionExecuteService transactionExecuteService,
        IValidator<ProductImageWithFileCreateRequest>? createWithFileRequestValidator = null,
        IValidator<ProductImageWithFileUpdateRequest>? updateWithFileRequestValidator = null,
        IValidator<ProductImageWithFileUpsertRequest>? upsertWithFileRequestValidator = null,
        IValidator<ProductImageWithFileForProductUpsertRequest>? upsertWithFileForProductRequestValidator = null,
        IValidator<ProductImageForProductUpsertRequest>? upsertForProductRequestValidator = null)
    {
        _productImageCrudService = productImageCrudService;
        _productImageFileService = productImageFileService;
        _productToHtmlProductService = productToHtmlProductService;
        _productHtmlService = productHtmlService;
        _transactionExecuteService = transactionExecuteService;

        _createWithFileRequestValidator = createWithFileRequestValidator;
        _upsertWithFileForProductRequestValidator = upsertWithFileForProductRequestValidator;
        _updateWithFileRequestValidator = updateWithFileRequestValidator;
        _upsertWithFileRequestValidator = upsertWithFileRequestValidator;
        _upsertForProductRequestValidator = upsertForProductRequestValidator;
    }

    private const string _invalidImageIdErrorMessage = "Image id does not correspond to any known image id";
    private const string _invalidFileTypeErrorMessage = "File type not supported";

    private readonly IProductImageCrudService _productImageCrudService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly IProductToHtmlProductService _productToHtmlProductService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    private readonly IValidator<ProductImageWithFileCreateRequest>? _createWithFileRequestValidator;
    private readonly IValidator<ProductImageWithFileUpdateRequest>? _updateWithFileRequestValidator;
    private readonly IValidator<ProductImageWithFileUpsertRequest>? _upsertWithFileRequestValidator;
    private readonly IValidator<ProductImageForProductUpsertRequest>? _upsertForProductRequestValidator;
    private readonly IValidator<ProductImageWithFileForProductUpsertRequest>? _upsertWithFileForProductRequestValidator;

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllWithoutFileDataAsync()
    {
        return await _productImageCrudService.GetAllWithoutFileDataAsync();
    }

    public async Task<List<IGrouping<int, ProductImage>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageCrudService.GetAllInProductsAsync(productIds);
    }

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllInProductsWithoutFileDataAsync(IEnumerable<int> productIds)
    {
        return await _productImageCrudService.GetAllInProductsWithoutFileDataAsync(productIds);
    }

    public async Task<List<ProductImage>> GetAllInProductAsync(int productId)
    {
        return await _productImageCrudService.GetAllInProductAsync(productId);
    }

    public async Task<List<ProductImageData>> GetAllInProductWithoutFileDataAsync(int productId)
    {
        return await _productImageCrudService.GetAllInProductWithoutFileDataAsync(productId);
    }

    public async Task<ProductImage?> GetByIdInAllImagesAsync(int id)
    {
        return await _productImageCrudService.GetByIdInAllImagesAsync(id);
    }

    public async Task<ProductImageData?> GetByIdInAllImagesWithoutFileDataAsync(int id)
    {
        return await _productImageCrudService.GetByIdInAllImagesWithoutFileDataAsync(id);
    }

    public async Task<List<ProductImage>> GetAllFirstImagesForAllProductsAsync()
    {
        return await _productImageCrudService.GetAllFirstImagesForAllProductsAsync();
    }

    public async Task<List<ProductImageData>> GetAllFirstImagesWithoutFileDataForAllProductsAsync()
    {
        return await _productImageCrudService.GetAllFirstImagesWithoutFileDataForAllProductsAsync();
    }

    public async Task<List<ProductImage>> GetFirstImagesForSelectionOfProductsAsync(List<int> productIds)
    {
        return await _productImageCrudService.GetFirstImagesForSelectionOfProductsAsync(productIds);
    }

    public async Task<List<ProductImageData>> GetFirstImagesWithoutFileDataForSelectionOfProductsAsync(List<int> productIds)
    {
        return await _productImageCrudService.GetFirstImagesWithoutFileDataForSelectionOfProductsAsync(productIds);
    }

    public async Task<ProductImage?> GetByProductIdInFirstImagesAsync(int productId)
    {
        return await _productImageCrudService.GetByProductIdInFirstImagesAsync(productId);
    }

    public async Task<List<ProductImagesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        return await _productImageCrudService.GetCountOfAllInProductsAsync(productIds);
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        return await _productImageCrudService.GetCountOfAllInProductAsync(productId);
    }

    public async Task<bool> DoesProductImageExistAsync(int imageId)
    {
        return await _productImageCrudService.DoesProductImageExistAsync(imageId);
    }

    public async Task<List<ProductFirstImageExistsForProductData>> DoProductsHaveImagesInFirstImagesAsync(List<int> productIds)
    {
        return await _productImageCrudService.DoProductsHaveImagesInFirstImagesAsync(productIds);
    }

    public async Task<bool> DoesProductHaveImageInFirstImagesAsync(int productId)
    {
        return await _productImageCrudService.DoesProductHaveImageInFirstImagesAsync(productId);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertInAllImagesAsync(ServiceProductImageCreateRequest createRequest)
    {
        return await _productImageCrudService.InsertInAllImagesAsync(createRequest);
    }

    public async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInAllImagesWithFileAsync(
       ProductImageWithFileCreateRequest productImageWithFileCreateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createWithFileRequestValidator, productImageWithFileCreateRequest);

        if (!validationResult.IsValid) return validationResult;

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => InsertInAllImagesWithFileInternalAsync(productImageWithFileCreateRequest),
        //    result => result.IsT0);

        return await InsertInAllImagesWithFileInternalAsync(productImageWithFileCreateRequest);
    }

    private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInAllImagesWithFileInternalAsync(
        ProductImageWithFileCreateRequest productImageWithFileCreateRequest)
    {
        string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileCreateRequest.FileExtension);

        string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileCreateRequest.FileExtension);

        if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImageWithFileCreateRequest.FileExtension), _invalidFileTypeErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        ServiceProductImageCreateRequest productImageCreateRequest = new()
        {
            ProductId = productImageWithFileCreateRequest.ProductId,
            ImageContentType = contentTypeFromFileExtension,
            ImageData = productImageWithFileCreateRequest.ImageData,
            HtmlData = productImageWithFileCreateRequest.HtmlData,
        };

        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertImageResult = await InsertInAllImagesAsync(productImageCreateRequest);

        if (!insertImageResult.IsT0)
        {
            return insertImageResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                imageId => new UnexpectedFailureResult(),
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        int newImageId = insertImageResult.AsT0;

        string? newImageFileName = $"{newImageId}{fileExtensionWithDot}";

        ProductImageFileCreateRequest productImageFileCreateRequest = new()
        {
            ProductId = productImageWithFileCreateRequest.ProductId,
            ImageId = newImageId,
            FileData = new()
            {
                FileName = newImageFileName,
                Data = productImageWithFileCreateRequest.ImageData,
            },
            Active = productImageWithFileCreateRequest.Active,
            CustomDisplayOrder = productImageWithFileCreateRequest.CustomDisplayOrder,
            CreateUserName = productImageWithFileCreateRequest.CreateUserName,
        };

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
            = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
            = createImageFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                fileInfoId => new ImageAndFileIdsInfo()
                {
                    ImageId = newImageId,
                    FileInfoId = fileInfoId
                },
                validationResult => validationResult,
                fileSaveFailureResult => fileSaveFailureResult,
                fileAlreadyExistsResult => fileAlreadyExistsResult,
                unexpectedFailureResult => unexpectedFailureResult);

        if (!result.IsT0) return result;

        localDBTransactionScope.Complete();

        replicationDBTransactionScope.Complete();

        return result;
    }

    //private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInAllImagesWithFileInternalAsync(
    //    ProductImageWithFileCreateRequest productImageWithFileCreateRequest)
    //{
    //    string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileCreateRequest.FileExtension);

    //    string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileCreateRequest.FileExtension);

    //    if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
    //    {
    //        ValidationFailure validationFailure = new(nameof(ProductImageWithFileCreateRequest.FileExtension), _invalidFileTypeErrorMessage);

    //        return CreateValidationResultFromErrors(validationFailure);
    //    }

    //    ServiceProductImageCreateRequest productImageCreateRequest = new()
    //    {
    //        ProductId = productImageWithFileCreateRequest.ProductId,
    //        ImageContentType = contentTypeFromFileExtension,
    //        ImageData = productImageWithFileCreateRequest.ImageData,
    //        HtmlData = productImageWithFileCreateRequest.HtmlData,
    //    };

    //    OneOf<int, ValidationResult, UnexpectedFailureResult> insertImageResult = await InsertInAllImagesAsync(productImageCreateRequest);

    //    if (!insertImageResult.IsT0)
    //    {
    //        return insertImageResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //            imageId => new UnexpectedFailureResult(),
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    int newImageId = insertImageResult.AsT0;

    //    string? newImageFileName = $"{newImageId}{fileExtensionWithDot}";

    //    ProductImageFileCreateRequest productImageFileCreateRequest = new()
    //    {
    //        ProductId = productImageWithFileCreateRequest.ProductId,
    //        ImageId = newImageId,
    //        FileData = new()
    //        {
    //            FileName = newImageFileName,
    //            Data = productImageWithFileCreateRequest.ImageData,
    //        },
    //        Active = productImageWithFileCreateRequest.Active,
    //        CustomDisplayOrder = productImageWithFileCreateRequest.CustomDisplayOrder,
    //        CreateUserName = productImageWithFileCreateRequest.CreateUserName,
    //    };

    //    OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
    //        = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

    //    OneOf<ImageAndFileIdsInfo, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
    //        = createImageFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //            fileInfoId => new ImageAndFileIdsInfo()
    //            {
    //                ImageId = newImageId,
    //                FileInfoId = fileInfoId
    //            },
    //            validationResult => validationResult,
    //            fileAlreadyExistsResult => fileAlreadyExistsResult,
    //            unexpectedFailureResult => unexpectedFailureResult);

    //    return result;
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInFirstImagesAsync(ServiceProductFirstImageCreateRequest createRequest)
    {
        return await _productImageCrudService.InsertInFirstImagesAsync(createRequest);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInAllImagesAsync(ServiceProductImageUpdateRequest updateRequest)
    {
        return await _productImageCrudService.UpdateInAllImagesAsync(updateRequest);
    }

    public async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        UpdateInAllImagesWithFileAsync(ProductImageWithFileUpdateRequest productImageWithFileUpdateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateWithFileRequestValidator, productImageWithFileUpdateRequest);

        if (!validationResult.IsValid) return validationResult;

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => UpdateInAllImagesWithFileInternalAsync(productImageWithFileUpdateRequest),
        //    result => result.IsT0);

        return await UpdateInAllImagesWithFileInternalAsync(productImageWithFileUpdateRequest);
    }

    private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        UpdateInAllImagesWithFileInternalAsync(ProductImageWithFileUpdateRequest productImageWithFileUpdateRequest)
    {
        ProductImage? image = await GetByIdInAllImagesAsync(productImageWithFileUpdateRequest.ImageId);

        if (image is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImageWithFileUpdateRequest.ImageId), _invalidImageIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        if (image.ProductId is null)
        {
            return new UnexpectedFailureResult();
        }

        int productId = image.ProductId.Value;

        string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileUpdateRequest.FileExtension);

        string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileUpdateRequest.FileExtension);

        if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
        {
            ValidationFailure validationFailure = new(nameof(FileData.FileName), _invalidFileTypeErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        string newImageFileName = $"{productImageWithFileUpdateRequest.ImageId}{fileExtensionWithDot}";

        ServiceProductImageUpdateRequest productImageCreateRequest = new()
        {
            Id = productImageWithFileUpdateRequest.ImageId,
            ImageContentType = contentTypeFromFileExtension,
            ImageData = productImageWithFileUpdateRequest.ImageData,
            HtmlData = productImageWithFileUpdateRequest.HtmlData,
        };

        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImageResult = await UpdateInAllImagesAsync(productImageCreateRequest);

        if (!updateImageResult.IsT0)
        {
            return updateImageResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => new ImageAndFileIdsInfo() { ImageId = productImageWithFileUpdateRequest.ImageId },
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        ProductImageFileData? fileNameInfo = await _productImageFileService.GetByProductIdAndImageIdAsync(productId, productImageWithFileUpdateRequest.ImageId);

        if (fileNameInfo is null)
        {
            ProductImageFileCreateRequest productImageFileCreateRequest = new()
            {
                ProductId = productId,
                ImageId = productImageWithFileUpdateRequest.ImageId,
                FileData = new()
                {
                    FileName = newImageFileName,
                    Data = productImageWithFileUpdateRequest.ImageData,
                },
                Active = productImageWithFileUpdateRequest.Active,
                CustomDisplayOrder = productImageWithFileUpdateRequest.CustomDisplayOrder,
                CreateUserName = productImageWithFileUpdateRequest.UpdateUserName,
            };

            OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
                = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

            OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResultMapped
                = createImageFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    fileInfoId => new ImageAndFileIdsInfo()
                    {
                        ImageId = productImageWithFileUpdateRequest.ImageId,
                        FileInfoId = fileInfoId
                    },
                    validationResult => validationResult,
                    fileSaveFailureResult => fileSaveFailureResult,
                    fileAlreadyExistsResult => fileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult);

            if (!createImageFileResultMapped.IsT0) return createImageFileResultMapped;

            localDBTransactionScope.Complete();

            replicationDBTransactionScope.Complete();

            return createImageFileResultMapped;
        }

        int? newDisplayOrder = null;

        if (productImageWithFileUpdateRequest.CustomDisplayOrder is not null
            && productImageWithFileUpdateRequest.CustomDisplayOrder != fileNameInfo.DisplayOrder)
        {
            newDisplayOrder = productImageWithFileUpdateRequest.CustomDisplayOrder.Value;
        }

        ProductImageFileUpdateRequest productImageFileNameInfoUpdateRequest = new()
        {
            Id = fileNameInfo.Id,
            Active = productImageWithFileUpdateRequest.Active,
            NewDisplayOrder = newDisplayOrder,
            UpdateFileDataRequest = new()
            {
                FileData = new()
                {
                    FileName = newImageFileName,
                    Data = productImageWithFileUpdateRequest.ImageData,
                },
            },
            UpdateImageIdRequest = new No(),
            UpdateUserName = productImageWithFileUpdateRequest.UpdateUserName,
        };

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
            = await _productImageFileService.UpdateFileAsync(productImageFileNameInfoUpdateRequest);

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResultMapped
            = updateFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => new ImageAndFileIdsInfo()
                {
                    ImageId = productImageWithFileUpdateRequest.ImageId,
                    FileInfoId = fileNameInfo.Id
                },
                validationResult => validationResult,
                fileSaveFailureResult => fileSaveFailureResult,
                fileDoesntExistResult => fileDoesntExistResult,
                fileAlreadyExistsResult => fileAlreadyExistsResult,
                unexpectedFailureResult => unexpectedFailureResult);

        if (!updateFileResultMapped.IsT0) return updateFileResultMapped;

        localDBTransactionScope.Complete();

        replicationDBTransactionScope.Complete();

        return updateFileResultMapped;
    }

    //private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
    //    UpdateInAllImagesWithFileInternalAsync(ProductImageWithFileUpdateRequest productImageWithFileUpdateRequest)
    //{
    //    ProductImage? image = await GetByIdInAllImagesAsync(productImageWithFileUpdateRequest.ImageId);

    //    if (image is null)
    //    {
    //        ValidationFailure validationFailure = new(nameof(ProductImageWithFileUpdateRequest.ImageId), _invalidImageIdErrorMessage);

    //        return CreateValidationResultFromErrors(validationFailure);
    //    }

    //    if (image.ProductId is null)
    //    {
    //        return new UnexpectedFailureResult();
    //    }

    //    int productId = image.ProductId.Value;

    //    string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileUpdateRequest.FileExtension);

    //    string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileUpdateRequest.FileExtension);

    //    if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
    //    {
    //        ValidationFailure validationFailure = new(nameof(FileData.FileName), _invalidFileTypeErrorMessage);

    //        return CreateValidationResultFromErrors(validationFailure);
    //    }

    //    string newImageFileName = $"{productImageWithFileUpdateRequest.ImageId}{fileExtensionWithDot}";

    //    ServiceProductImageUpdateRequest productImageCreateRequest = new()
    //    {
    //        Id = productImageWithFileUpdateRequest.ImageId,
    //        ImageContentType = contentTypeFromFileExtension,
    //        ImageData = productImageWithFileUpdateRequest.ImageData,
    //        HtmlData = productImageWithFileUpdateRequest.HtmlData,
    //    };

    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImageResult = await UpdateInAllImagesAsync(productImageCreateRequest);

    //    if (!updateImageResult.IsT0)
    //    {
    //        return updateImageResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //            success => new ImageAndFileIdsInfo() { ImageId = productImageWithFileUpdateRequest.ImageId },
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    ProductImageFileData? fileNameInfo = await _productImageFileService.GetByProductIdAndImageIdAsync(productId, productImageWithFileUpdateRequest.ImageId);

    //    if (fileNameInfo is null)
    //    {
    //        ProductImageFileCreateRequest productImageFileCreateRequest = new()
    //        {
    //            ProductId = productId,
    //            ImageId = productImageWithFileUpdateRequest.ImageId,
    //            FileData = new()
    //            {
    //                FileName = newImageFileName,
    //                Data = productImageWithFileUpdateRequest.ImageData,
    //            },
    //            Active = productImageWithFileUpdateRequest.Active,
    //            CustomDisplayOrder = productImageWithFileUpdateRequest.CustomDisplayOrder,
    //            CreateUserName = productImageWithFileUpdateRequest.UpdateUserName,
    //        };

    //        OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
    //            = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

    //        return createImageFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //            fileInfoId => new ImageAndFileIdsInfo()
    //            {
    //                ImageId = productImageWithFileUpdateRequest.ImageId,
    //                FileInfoId = fileInfoId
    //            },
    //            validationResult => validationResult,
    //            fileAlreadyExistsResult => fileAlreadyExistsResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    int? newDisplayOrder = null;

    //    if (productImageWithFileUpdateRequest.CustomDisplayOrder is not null
    //        && productImageWithFileUpdateRequest.CustomDisplayOrder != fileNameInfo.DisplayOrder)
    //    {
    //        newDisplayOrder = productImageWithFileUpdateRequest.CustomDisplayOrder.Value;
    //    }

    //    ProductImageFileUpdateRequest productImageFileNameInfoUpdateRequest = new()
    //    {
    //        Id = fileNameInfo.Id,
    //        Active = productImageWithFileUpdateRequest.Active,
    //        NewDisplayOrder = newDisplayOrder,
    //        UpdateFileDataRequest = new()
    //        {
    //            FileData = new()
    //            {
    //                FileName = newImageFileName,
    //                Data = productImageWithFileUpdateRequest.ImageData,
    //            },
    //        },
    //        UpdateImageIdRequest = new No(),
    //        UpdateUserName = productImageWithFileUpdateRequest.UpdateUserName,
    //    };

    //    OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
    //        = await _productImageFileService.UpdateFileAsync(productImageFileNameInfoUpdateRequest);

    //    return updateFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //        success => new ImageAndFileIdsInfo()
    //        {
    //            ImageId = productImageWithFileUpdateRequest.ImageId,
    //            FileInfoId = fileNameInfo.Id
    //        },
    //        validationResult => validationResult,
    //        fileDoesntExistResult => fileDoesntExistResult,
    //        fileAlreadyExistsResult => fileAlreadyExistsResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInFirstImagesAsync(ServiceProductFirstImageUpdateRequest updateRequest)
    {
        return await _productImageCrudService.UpdateInFirstImagesAsync(updateRequest);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertInAllImagesAsync(ProductImageUpsertRequest productImageUpsertRequest)
    {
        return await _productImageCrudService.UpsertInAllImagesAsync(productImageUpsertRequest);
    }

    public async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        UpsertInAllImagesWithFileAsync(ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest)
    {
        ValidationResult validationResult = ValidateDefault(_upsertWithFileRequestValidator, productImageWithFileUpsertRequest);

        if (!validationResult.IsValid) return validationResult;

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => UpsertInAllImagesWithFileInternalAsync(productImageWithFileUpsertRequest),
        //    result => result.IsT0);

        return await UpsertInAllImagesWithFileInternalAsync(productImageWithFileUpsertRequest);
    }

    private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        UpsertInAllImagesWithFileInternalAsync(ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest)
    {
        string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileUpsertRequest.FileExtension);

        string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileUpsertRequest.FileExtension);

        if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
        {
            ValidationFailure validationFailure = new(nameof(FileData.FileName), _invalidFileTypeErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        ProductImageUpsertRequest productImageUpsertRequest = new()
        {
            ProductId = productImageWithFileUpsertRequest.ProductId,
            ExistingImageId = productImageWithFileUpsertRequest.ExistingImageId,
            ImageContentType = contentTypeFromFileExtension,
            ImageData = productImageWithFileUpsertRequest.ImageData,
            HtmlData = productImageWithFileUpsertRequest.HtmlData,
        };

        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<int, ValidationResult, UnexpectedFailureResult> imageUpsertResult = await UpsertInAllImagesAsync(productImageUpsertRequest);

        if (!imageUpsertResult.IsT0)
        {
            return imageUpsertResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                id => new UnexpectedFailureResult(),
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        int imageId = imageUpsertResult.AsT0;

        if (productImageWithFileUpsertRequest.FileUpsertRequest is null)
        {
            return new ImageAndFileIdsInfo() { ImageId = imageId };
        }

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> result
            = await UpsertImageFileForImageAsync(
                productImageWithFileUpsertRequest.ProductId,
                imageId,
                productImageWithFileUpsertRequest.ImageData,
                productImageWithFileUpsertRequest.FileUpsertRequest.Active,
                productImageWithFileUpsertRequest.FileUpsertRequest.CustomDisplayOrder,
                fileExtensionWithDot,
                productImageWithFileUpsertRequest.FileUpsertRequest.UpsertUserName);

        if (!result.IsT0) return result;

        localDBTransactionScope.Complete();

        replicationDBTransactionScope.Complete();

        return result;
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

        ProductImageFileUpdateRequest productImageFileNameInfoUpdateRequest = new()
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
            = await _productImageFileService.UpdateFileAsync(productImageFileNameInfoUpdateRequest);

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

    //private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
    //    UpsertInAllImagesWithFileInternalAsync(ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest)
    //{
    //    string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileUpsertRequest.FileExtension);

    //    string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileUpsertRequest.FileExtension);

    //    if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
    //    {
    //        ValidationFailure validationFailure = new(nameof(FileData.FileName), _invalidFileTypeErrorMessage);

    //        return CreateValidationResultFromErrors(validationFailure);
    //    }

    //    ProductImageUpsertRequest productImageUpsertRequest = new()
    //    {
    //        ProductId = productImageWithFileUpsertRequest.ProductId,
    //        ExistingImageId = productImageWithFileUpsertRequest.ExistingImageId,
    //        ImageContentType = contentTypeFromFileExtension,
    //        ImageData = productImageWithFileUpsertRequest.ImageData,
    //        HtmlData = productImageWithFileUpsertRequest.HtmlData,
    //    };

    //    OneOf<int, ValidationResult, UnexpectedFailureResult> imageUpsertResult = await UpsertInAllImagesAsync(productImageUpsertRequest);

    //    if (!imageUpsertResult.IsT0)
    //    {
    //        return imageUpsertResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //            id => new UnexpectedFailureResult(),
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    int imageId = imageUpsertResult.AsT0;

    //    if (productImageWithFileUpsertRequest.FileUpsertRequest is null)
    //    {
    //        return new ImageAndFileIdsInfo() { ImageId = imageId };
    //    }

    //    string? newImageFileName = $"{imageId}{fileExtensionWithDot}";

    //    FileForImageUpsertRequest fileUpsertRequest = productImageWithFileUpsertRequest.FileUpsertRequest;

    //    ProductImageFileData? fileNameInfo = await _productImageFileService.GetByProductIdAndImageIdAsync(productImageWithFileUpsertRequest.ProductId, imageId);

    //    if (fileNameInfo is null)
    //    {
    //        ProductImageFileCreateRequest productImageFileCreateRequest = new()
    //        {
    //            ProductId = productImageWithFileUpsertRequest.ProductId,
    //            ImageId = imageId,
    //            FileData = new()
    //            {
    //                FileName = newImageFileName,
    //                Data = productImageWithFileUpsertRequest.ImageData,
    //            },
    //            Active = fileUpsertRequest.Active,
    //            CustomDisplayOrder = fileUpsertRequest.CustomDisplayOrder,
    //            CreateUserName = fileUpsertRequest.UpsertUserName,
    //        };

    //        OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResult
    //            = await _productImageFileService.InsertFileAsync(productImageFileCreateRequest);

    //        OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> createImageFileResultMapped
    //            = createImageFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //                fileInfoId => new ImageAndFileIdsInfo()
    //                {
    //                    ImageId = imageId,
    //                    FileInfoId = fileInfoId
    //                },
    //                validationResult => validationResult,
    //                fileAlreadyExistsResult => fileAlreadyExistsResult,
    //                unexpectedFailureResult => unexpectedFailureResult);

    //        return createImageFileResultMapped;
    //    }

    //    int fileInfoId = fileNameInfo.Id;

    //    int? newDisplayOrder = null;

    //    if (fileUpsertRequest.CustomDisplayOrder is not null
    //        && fileUpsertRequest.CustomDisplayOrder != fileNameInfo.DisplayOrder)
    //    {
    //        newDisplayOrder = fileUpsertRequest.CustomDisplayOrder.Value;
    //    }

    //    ProductImageFileUpdateRequest productImageFileNameInfoUpdateRequest = new()
    //    {
    //        Id = fileInfoId,
    //        Active = fileUpsertRequest.Active,
    //        NewDisplayOrder = newDisplayOrder,
    //        UpdateFileDataRequest = new()
    //        {
    //            FileData = new()
    //            {
    //                FileName = newImageFileName,
    //                Data = productImageWithFileUpsertRequest.ImageData,
    //            },
    //        },
    //        UpdateImageIdRequest = new No(),
    //        UpdateUserName = fileUpsertRequest.UpsertUserName,
    //    };

    //    OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResult
    //        = await _productImageFileService.UpdateFileAsync(productImageFileNameInfoUpdateRequest);

    //    OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateFileResultMapped
    //        = updateFileResult.Match<OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //            success => new ImageAndFileIdsInfo()
    //            {
    //                ImageId = imageId,
    //                FileInfoId = fileInfoId
    //            },
    //            validationResult => validationResult,
    //            fileDoesntExistResult => fileDoesntExistResult,
    //            fileAlreadyExistsResult => fileAlreadyExistsResult,
    //            unexpectedFailureResult => unexpectedFailureResult);

    //    return updateFileResultMapped;
    //}

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInFirstImagesAsync(ServiceProductFirstImageUpsertRequest upsertRequest)
    {
        return await _productImageCrudService.UpsertInFirstImagesAsync(upsertRequest);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesForProductAsync(
        int productId,
        List<ProductImageForProductUpsertRequest> imageUpsertRequests)
    {
        foreach (ProductImageForProductUpsertRequest imageUpsertRequest in imageUpsertRequests)
        {
            ValidationResult validationResult = ValidateDefault(_upsertForProductRequestValidator, imageUpsertRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertFirstAndAllImagesForProductInternalAsync(productId, imageUpsertRequests),
            result => result.IsT0);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesForProductInternalAsync(
        int productId,
        List<ProductImageForProductUpsertRequest> imageUpsertRequests)
    {
        List<ProductImage>? oldProductImages = await GetAllInProductAsync(productId);

        ServiceProductFirstImageUpsertRequest? productFirstImageUpsertRequest = null;

        foreach (ProductImageForProductUpsertRequest productImageUpsertRequest in imageUpsertRequests)
        {
            ProductImageUpsertRequest productImageUpsertRequestInternal = new()
            {
                ProductId = productId,
                ExistingImageId = productImageUpsertRequest.ExistingImageId,
                ImageContentType = productImageUpsertRequest.ImageContentType,
                ImageData = productImageUpsertRequest.ImageData,
                HtmlData = productImageUpsertRequest.HtmlData,
            };

            OneOf<int, ValidationResult, UnexpectedFailureResult> upsertImageResult = await UpsertInAllImagesAsync(productImageUpsertRequestInternal);

            if (!upsertImageResult.IsT0)
            {
                return upsertImageResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                    success => new UnexpectedFailureResult(),
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            if (productImageUpsertRequestInternal.ExistingImageId is not null)
            {
                int imageIndex = oldProductImages.FindIndex(x => x.Id == productImageUpsertRequestInternal.ExistingImageId);

                oldProductImages.RemoveAt(imageIndex);
            }

            productFirstImageUpsertRequest ??= new()
            {
                ProductId = productId,
                ImageContentType = productImageUpsertRequest.ImageContentType,
                ImageData = productImageUpsertRequest.ImageData,
                HtmlData = productImageUpsertRequest.HtmlData,
            };
        }

        foreach (ProductImage oldImageToBeRemoved in oldProductImages)
        {
            bool imageDeleteResult = await _productImageCrudService.DeleteInAllImagesByIdAsync(oldImageToBeRemoved.Id);

            if (!imageDeleteResult) return new UnexpectedFailureResult();
        }

        if (productFirstImageUpsertRequest is not null)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> productFirstImageUpsertResult = await UpsertInFirstImagesAsync(productFirstImageUpsertRequest);

            if (!productFirstImageUpsertResult.IsT0)
            {
                return productFirstImageUpsertResult;
            }
        }
        else
        {
            ProductImage? oldProductFirstImage = await GetByProductIdInFirstImagesAsync(productId);

            if (oldProductFirstImage is null) return new Success();

            bool isFirstImageDeleted = await DeleteInFirstImagesByProductIdAsync(productId);

            if (!isFirstImageDeleted) return new UnexpectedFailureResult();
        }

        return new Success();
    }

    public async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        UpsertFirstAndAllImagesWithFilesForProductAsync(
            int productId,
            List<ProductImageWithFileForProductUpsertRequest> imageAndFileNameUpsertRequests,
            string deleteUserName)
    {
        foreach (ProductImageWithFileForProductUpsertRequest imageWithFileForProductUpsertRequest in imageAndFileNameUpsertRequests)
        {
            ValidationResult validationResult = ValidateDefault(_upsertWithFileForProductRequestValidator, imageWithFileForProductUpsertRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //   () => UpsertFirstAndAllImagesWithFilesForProductInternalAsync(productId, imageAndFileNameUpsertRequests, deleteUserName),
        //   result => result.IsT0);

        return await UpsertFirstAndAllImagesWithFilesForProductInternalAsync(productId, imageAndFileNameUpsertRequests, deleteUserName);
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
        UpsertFirstAndAllImagesWithFilesForProductInternalAsync(
            int productId,
            List<ProductImageWithFileForProductUpsertRequest> imageAndFileNameUpsertRequests,
            string deleteUserName)
    {
        List<ProductImage>? oldProductImages = await GetAllInProductAsync(productId);
        List<ProductImageFileData>? oldProductImageFileInfos = await _productImageFileService.GetAllInProductAsync(productId);

        imageAndFileNameUpsertRequests = OrderProductImageWithFileUpsertRequests(imageAndFileNameUpsertRequests);

        foreach (ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest in imageAndFileNameUpsertRequests)
        {
            if (productImageWithFileUpsertRequest.ExistingImageId is null) continue;

            int imageIndex = oldProductImages.FindIndex(x => x.Id == productImageWithFileUpsertRequest.ExistingImageId);

            if (imageIndex < 0)
            {
                ValidationFailure imageDoesNotExistError = new(nameof(ProductImageWithFileForProductUpsertRequest.ExistingImageId), _invalidImageIdErrorMessage);

                return CreateValidationResultFromErrors(imageDoesNotExistError);
            }

            oldProductImages.RemoveAt(imageIndex);

            int imageFileNameIndex = oldProductImageFileInfos.FindIndex(x => x.ImageId == productImageWithFileUpsertRequest.ExistingImageId);

            if (imageFileNameIndex < 0) continue;

            oldProductImageFileInfos.RemoveAt(imageFileNameIndex);
        }

        HtmlProductsData htmlProductsData = await _productToHtmlProductService.GetHtmlProductDataFromProductsAsync([productId]);

        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProducts(htmlProductsData);

        if (!getProductHtmlResult.IsT0) return new UnexpectedFailureResult();

        string productHtml = getProductHtmlResult.AsT0;

        ServiceProductFirstImageUpsertRequest? productFirstImageUpsertRequest = null;

        //using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        Dictionary<ProductImageWithFileForProductUpsertRequest, int> imageIdsForRequests = new();

        using (TransactionScope imagesTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            foreach (ProductImage oldImageToBeRemoved in oldProductImages)
            {
                bool imageDeleteResult = await _productImageCrudService.DeleteInAllImagesByIdAsync(oldImageToBeRemoved.Id);

                if (!imageDeleteResult) return new UnexpectedFailureResult();
            }

            foreach (ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest in imageAndFileNameUpsertRequests)
            {
                string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileUpsertRequest.FileExtension);

                string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileUpsertRequest.FileExtension);

                if (fileExtensionWithDot is null || contentTypeFromFileExtension is null)
                {
                    ValidationFailure validationFailure = new(nameof(FileData.FileName), _invalidFileTypeErrorMessage);

                    return CreateValidationResultFromErrors(validationFailure);
                }

                ProductImageUpsertRequest productImageUpsertRequest = new()
                {
                    ProductId = productId,
                    ExistingImageId = productImageWithFileUpsertRequest.ExistingImageId,
                    ImageContentType = contentTypeFromFileExtension,
                    ImageData = productImageWithFileUpsertRequest.ImageData,
                    HtmlData = productImageWithFileUpsertRequest.HtmlDataOptions.Match(
                        useCurrentData => productHtml,
                        doNotUpdate => oldProductImages.FirstOrDefault(x => x.Id == productImageWithFileUpsertRequest.ExistingImageId)?.HtmlData,
                        useCustomData => useCustomData.HtmlData),
                };

                OneOf<int, ValidationResult, UnexpectedFailureResult> imageUpsertResult = await UpsertInAllImagesAsync(productImageUpsertRequest);

                if (!imageUpsertResult.IsT0)
                {
                    return imageUpsertResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                        id => new UnexpectedFailureResult(),
                        validationResult => validationResult,
                        unexpectedFailureResult => unexpectedFailureResult);
                }

                imageIdsForRequests.Add(productImageWithFileUpsertRequest, imageUpsertResult.AsT0);

                if (productFirstImageUpsertRequest is null)
                {
                    productFirstImageUpsertRequest ??= new()
                    {
                        ProductId = productId,
                        ImageContentType = contentTypeFromFileExtension,
                        ImageData = productImageWithFileUpsertRequest.ImageData,
                        HtmlData = productImageWithFileUpsertRequest.HtmlDataOptions.Match(
                            useCurrentData => productHtml,
                            doNotUpdate => oldProductImages.FirstOrDefault(x => x.Id == productImageWithFileUpsertRequest.ExistingImageId)?.HtmlData,
                            useCustomData => useCustomData.HtmlData),
                    };
                }
            }

            if (productFirstImageUpsertRequest is not null)
            {
                OneOf<Success, ValidationResult, UnexpectedFailureResult> productFirstImageUpsertResult = await UpsertInFirstImagesAsync(productFirstImageUpsertRequest);

                if (!productFirstImageUpsertResult.IsT0)
                {
                    return productFirstImageUpsertResult.Map<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
                }
            }
            else
            {
                ProductImage? oldProductFirstImage = await GetByProductIdInFirstImagesAsync(productId);

                if (oldProductFirstImage is null) return new Success();

                bool isFirstImageDeleted = await DeleteInFirstImagesByProductIdAsync(productId);

                if (!isFirstImageDeleted) return new UnexpectedFailureResult();
            }

            imagesTransactionScope.Complete();
        }

        //using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        foreach (ProductImageFileData oldImageFileToBeRemoved in oldProductImageFileInfos)
        {
            OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> imageFileDeleteResult
                = await _productImageFileService.DeleteFileAsync(oldImageFileToBeRemoved.Id, deleteUserName);

            if (!imageFileDeleteResult.IsT0) return new UnexpectedFailureResult();
        }

        foreach (ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest in imageAndFileNameUpsertRequests)
        {
            if (productImageWithFileUpsertRequest.FileUpsertRequest is null) continue;

            int imageId = imageIdsForRequests[productImageWithFileUpsertRequest];

            string? fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(productImageWithFileUpsertRequest.FileExtension);

            if (fileExtensionWithDot is null)
            {
                ValidationFailure validationFailure = new(nameof(FileData.FileName), _invalidFileTypeErrorMessage);

                return CreateValidationResultFromErrors(validationFailure);
            }

            OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertImageWithFileResult
                = await UpsertImageFileForImageAsync(
                    productId,
                    imageId,
                    productImageWithFileUpsertRequest.ImageData,
                    productImageWithFileUpsertRequest.FileUpsertRequest.Active,
                    productImageWithFileUpsertRequest.FileUpsertRequest.CustomDisplayOrder,
                    fileExtensionWithDot,
                    deleteUserName);

            if (!upsertImageWithFileResult.IsT0)
            {
                return upsertImageWithFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    success => new UnexpectedFailureResult(),
                    validationResult => validationResult,
                    fileSaveFailureResult => fileSaveFailureResult,
                    fileDoesntExistResult => fileDoesntExistResult,
                    fileAlreadyExistsResult => fileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }
        }

        //localDBTransactionScope.Complete();

        //replicationDBTransactionScope.Complete();

        return new Success();
    }

    //private async Task<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>
    //   UpsertFirstAndAllImagesWithFilesForProductInternalAsync(
    //       int productId,
    //       List<ProductImageWithFileForProductUpsertRequest> imageAndFileNameUpsertRequests,
    //       string deleteUserName)
    //{
    //    List<ProductImage>? oldProductImages = await GetAllInProductAsync(productId);
    //    List<ProductImageFileData>? oldProductImageFileInfos = await _productImageFileService.GetAllInProductAsync(productId);

    //    imageAndFileNameUpsertRequests = OrderProductImageWithFileUpsertRequests(imageAndFileNameUpsertRequests);

    //    foreach (ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest in imageAndFileNameUpsertRequests)
    //    {
    //        if (productImageWithFileUpsertRequest.ExistingImageId is null) continue;

    //        int imageIndex = oldProductImages.FindIndex(x => x.Id == productImageWithFileUpsertRequest.ExistingImageId);

    //        if (imageIndex < 0)
    //        {
    //            ValidationFailure imageDoesNotExistError = new(nameof(ProductImageWithFileForProductUpsertRequest.ExistingImageId), _invalidImageIdErrorMessage);

    //            return CreateValidationResultFromErrors(imageDoesNotExistError);
    //        }

    //        oldProductImages.RemoveAt(imageIndex);

    //        int imageFileNameIndex = oldProductImageFileInfos.FindIndex(x => x.ImageId == productImageWithFileUpsertRequest.ExistingImageId);

    //        if (imageFileNameIndex < 0) continue;

    //        oldProductImageFileInfos.RemoveAt(imageFileNameIndex);
    //    }

    //    ServiceProductFirstImageUpsertRequest? productFirstImageUpsertRequest = null;

    //    foreach (ProductImage oldImageToBeRemoved in oldProductImages)
    //    {
    //        bool imageDeleteResult = await _productImageCrudService.DeleteInAllImagesByIdAsync(oldImageToBeRemoved.Id);

    //        if (!imageDeleteResult) return new UnexpectedFailureResult();
    //    }

    //    foreach (ProductImageFileData oldImageFileToBeRemoved in oldProductImageFileInfos)
    //    {
    //        OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> imageFileDeleteResult
    //            = await _productImageFileService.DeleteFileAsync(oldImageFileToBeRemoved.Id, deleteUserName);

    //        if (!imageFileDeleteResult.IsT0) return new UnexpectedFailureResult();
    //    }

    //    foreach (ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest in imageAndFileNameUpsertRequests)
    //    {
    //        FileForImageUpsertRequest? fileForImageUpsertRequest = null;

    //        if (productImageWithFileUpsertRequest.FileUpsertRequest is not null)
    //        {
    //            fileForImageUpsertRequest = new()
    //            {
    //                Active = productImageWithFileUpsertRequest.FileUpsertRequest.Active,
    //                CustomDisplayOrder = productImageWithFileUpsertRequest.FileUpsertRequest.CustomDisplayOrder,
    //                UpsertUserName = deleteUserName,
    //            };
    //        }

    //        ProductImageWithFileUpsertRequest productImageWithFileUpsertRequestInternal = new()
    //        {
    //            ProductId = productId,
    //            ExistingImageId = productImageWithFileUpsertRequest.ExistingImageId,
    //            FileExtension = productImageWithFileUpsertRequest.FileExtension,
    //            ImageData = productImageWithFileUpsertRequest.ImageData,
    //            HtmlData = productImageWithFileUpsertRequest.HtmlData,
    //            FileUpsertRequest = fileForImageUpsertRequest,
    //        };

    //        OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertImageWithFileResult
    //            = await UpsertInAllImagesWithFileAsync(productImageWithFileUpsertRequestInternal);

    //        if (!upsertImageWithFileResult.IsT0)
    //        {
    //            return upsertImageWithFileResult.Match<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //                success => new UnexpectedFailureResult(),
    //                validationResult => validationResult,
    //                fileDoesntExistResult => fileDoesntExistResult,
    //                fileAlreadyExistsResult => fileAlreadyExistsResult,
    //                unexpectedFailureResult => unexpectedFailureResult);
    //        }

    //        if (productFirstImageUpsertRequest is null)
    //        {
    //            string? contentTypeFromFileExtension = GetContentTypeFromExtension(productImageWithFileUpsertRequest.FileExtension);

    //            if (contentTypeFromFileExtension is null)
    //            {
    //                ValidationFailure validationFailure = new(nameof(FileData.FileName), _invalidFileTypeErrorMessage);

    //                return CreateValidationResultFromErrors(validationFailure);
    //            }

    //            productFirstImageUpsertRequest ??= new()
    //            {
    //                ProductId = productId,
    //                ImageContentType = contentTypeFromFileExtension,
    //                ImageData = productImageWithFileUpsertRequest.ImageData,
    //                HtmlData = productImageWithFileUpsertRequest.HtmlData,
    //            };
    //        }
    //    }

    //    if (productFirstImageUpsertRequest is not null)
    //    {
    //        OneOf<Success, ValidationResult, UnexpectedFailureResult> productFirstImageUpsertResult = await UpsertInFirstImagesAsync(productFirstImageUpsertRequest);

    //        if (!productFirstImageUpsertResult.IsT0)
    //        {
    //            return productFirstImageUpsertResult.Map<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
    //        }
    //    }
    //    else
    //    {
    //        ProductImage? oldProductFirstImage = await GetByProductIdInFirstImagesAsync(productId);

    //        if (oldProductFirstImage is null) return new Success();

    //        bool isFirstImageDeleted = await DeleteInFirstImagesByProductIdAsync(productId);

    //        if (!isFirstImageDeleted) return new UnexpectedFailureResult();
    //    }

    //    return new Success();
    //}

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInAllImagesByIdAsync(int imageId, string htmlData)
    {
        return await _productImageCrudService.UpdateHtmlDataInAllImagesByIdAsync(imageId, htmlData);
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstImagesByProductIdAsync(int productId, string htmlData)
    {
        return await _productImageCrudService.UpdateHtmlDataInFirstImagesByProductIdAsync(productId, htmlData);
    }

    public async Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(int productId, string htmlData)
    {
        return await _productImageCrudService.UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(productId, htmlData);
    }

    public async Task<bool> DeleteAllImagesForProductAsync(int productId)
    {
        return await _productImageCrudService.DeleteAllInAllImagesByProductIdAsync(productId);
    }

    public async Task<bool> DeleteInAllImagesByIdAsync(int id)
    {
        return await _productImageCrudService.DeleteInAllImagesByIdAsync(id);
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInAllImagesByIdWithFileAsync(
        int id, string deleteUserName)
    {
        if (id <= 0) return new NotFound();

        ProductImageData? image = await GetByIdInAllImagesWithoutFileDataAsync(id);

        if (image is null) return new NotFound();

        if (image.ProductId is null) return new UnexpectedFailureResult();

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => DeleteInAllImagesByIdWithFileInternalAsync(id, image.ProductId.Value, deleteUserName),
        //    result => result.IsT0);

        return await DeleteInAllImagesByIdWithFileInternalAsync(id, image.ProductId.Value, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInAllImagesByIdWithFileInternalAsync(
        int id, int productId, string deleteUserName)
    {
        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        bool isImageDeleted = await _productImageCrudService.DeleteInAllImagesByIdAsync(id);

        if (!isImageDeleted) return new NotFound();

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> fileDeleteResult
            = await _productImageFileService.DeleteFileByProductIdAndImageIdAsync(productId, id, deleteUserName);

        if (!fileDeleteResult.IsT0)
        {
            return fileDeleteResult.Match<OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult>>(
                success => success,
                notFound => new Success(),
                fileDoesntExistResult => fileDoesntExistResult,
                validationResult => new UnexpectedFailureResult(),
                unexpectedFailureResult => unexpectedFailureResult);
        }

        localDBTransactionScope.Complete();

        replicationDBTransactionScope.Complete();

        return fileDeleteResult.AsT0;
    }

    //private async Task<OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInAllImagesByIdWithFileInternalAsync(
    //    int id, int productId, string deleteUserName)
    //{
    //    bool isImageDeleted = await _productImageCrudService.DeleteInAllImagesByIdAsync(id);

    //    if (!isImageDeleted) return new NotFound();

    //    OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> fileDeleteResult
    //        = await _productImageFileService.DeleteFileByProductIdAndImageIdAsync(productId, id, deleteUserName);

    //    return fileDeleteResult.Match<OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult>>(
    //        success => success,
    //        notFound => new Success(),
    //        fileDoesntExistResult => fileDoesntExistResult,
    //        validationResult => new UnexpectedFailureResult(),
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    public async Task<bool> DeleteInFirstImagesByProductIdAsync(int productId)
    {
        return await _productImageCrudService.DeleteInFirstImagesByProductIdAsync(productId);
    }
}