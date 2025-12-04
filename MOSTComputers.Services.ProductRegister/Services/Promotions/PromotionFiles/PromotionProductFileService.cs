using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;
using static MOSTComputers.Utils.OneOf.MappingExtensions;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles;
internal sealed class PromotionProductFileService : IPromotionProductFileService
{
    public PromotionProductFileService(
        IPromotionProductFileInfoService promotionProductFileInfoService,
        IPromotionFileService promotionFileService,
        IProductImageAndFileService productImageAndFileService,
        //IProductImageFileService productImageFileService,
        IProductRepository productRepository,
        IProductToHtmlProductService productToHtmlProductService,
        IProductWorkStatusesWorkflowService productWorkStatusesWorkflowService,
        ITransactionExecuteService transactionExecuteService,
        IValidator<ServicePromotionProductFileCreateRequest>? createRequestValidator = null,
        IValidator<ServicePromotionProductFileUpdateRequest>? updateRequestValidator = null)
    {
        _promotionProductFileInfoService = promotionProductFileInfoService;
        _promotionFileService = promotionFileService;
        _productImageAndFileService = productImageAndFileService;
        //_productImageFileService = productImageFileService;
        _productRepository = productRepository;
        _productToHtmlProductService = productToHtmlProductService;
        _productWorkStatusesWorkflowService = productWorkStatusesWorkflowService;
        _transactionExecuteService = transactionExecuteService;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private const string _invalidProductIdErrorMessage = "Id does not correspond to any known product id";
    private const string _invalidPromotionFileIdErrorMessage = "Id does not correspond to any known promotion file id";
    private const string _invalidPromotionProductFileIdErrorMessage = "Id does not correspond to any known promotion product file id";
    
    private const string _promotionFileDoesNotExistErrorMessage = "Promotion file does not exist";
    private const string _promotionFileIsNotAnImageErrorMessage = "Promotion file is not an image";

    private readonly IPromotionProductFileInfoService _promotionProductFileInfoService;

    private readonly IPromotionFileService _promotionFileService;
    private readonly IProductImageAndFileService _productImageAndFileService;
    //private readonly IProductImageFileService _productImageFileService;

    private readonly IProductRepository _productRepository;
    private readonly IProductToHtmlProductService _productToHtmlProductService;
    private readonly IProductWorkStatusesWorkflowService _productWorkStatusesWorkflowService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    private readonly IValidator<ServicePromotionProductFileCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServicePromotionProductFileUpdateRequest>? _updateRequestValidator;

    public async Task<List<IGrouping<int, PromotionProductFileInfo>>> GetAllForProductsAsync(IEnumerable<int> productIds)
    {
        return await _promotionProductFileInfoService.GetAllForProductsAsync(productIds);
    }

    public async Task<List<PromotionProductFileInfoForProductCountData>> GetCountOfAllForProductsAsync(IEnumerable<int> productIds)
    {
        return await _promotionProductFileInfoService.GetCountOfAllForProductsAsync(productIds);
    }

    public async Task<List<PromotionProductFileInfo>> GetAllForProductAsync(int productId)
    {
        return await _promotionProductFileInfoService.GetAllForProductAsync(productId);
    }

    public async Task<int> GetCountOfAllForProductAsync(int productId)
    {
        return await _promotionProductFileInfoService.GetCountOfAllForProductAsync(productId);
    }

    public async Task<bool> DoesExistForPromotionFileAsync(int promotionFileId)
    {
        return await _promotionProductFileInfoService.DoesExistForPromotionFileAsync(promotionFileId);
    }

    public async Task<PromotionProductFileInfo?> GetByIdAsync(int id)
    {
        return await _promotionProductFileInfoService.GetByIdAsync(id);
    }

    public async Task<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(
        ServicePromotionProductFileCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        Product? product = await _productRepository.GetByIdAsync(createRequest.ProductId);

        if (product is null)
        {
            ValidationFailure validationFailure = new(nameof(createRequest.ProductId), _invalidProductIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        PromotionFileInfo? promotionFile = await _promotionFileService.GetByIdAsync(createRequest.PromotionFileInfoId);

        Stream? promotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(createRequest.PromotionFileInfoId);

        if (promotionFile is null || promotionFileDataStream is null)
        {
            ValidationFailure validationFailure = new(nameof(createRequest.PromotionFileInfoId), _invalidPromotionFileIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => InsertInternalAsync(createRequest, product, promotionFile.FileName, promotionFileDataStream),
        //    result => result.Match(
        //        id => true,
        //        validationResult => false,
        //        fileAlreadyExistsResult => false,
        //        unexpectedFailureResult => false));

        return await InsertInternalAsync(createRequest, product, promotionFile.FileName, promotionFileDataStream);
    }

    private async Task<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInternalAsync(
        ServicePromotionProductFileCreateRequest createRequest,
        Product product,
        string fileName,
        Stream fileDataStream)
    {
        int? productImageId = null;

        TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        if (createRequest.CreateInProductImagesRequest is not null)
        {
            OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> insertImageAndImageFileResult
                = await InsertImageAndImageFileAsync(product, fileName, fileDataStream, createRequest.CreateInProductImagesRequest, createRequest.CreateUserName);

            if (!insertImageAndImageFileResult.IsT0)
            {
                return insertImageAndImageFileResult.Match<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    imageAndFileIdsInfo => new UnexpectedFailureResult(),
                    validationResult => validationResult,
                    fileSaveFailureResult => fileSaveFailureResult,
                    fileAlreadyExistsResult => fileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            int newImageId = insertImageAndImageFileResult.AsT0.ImageId;

            productImageId = newImageId;
        }

        ServicePromotionProductFileInfoCreateRequest promotionProductFileInfoCreateRequest = new()
        {
            ProductId = createRequest.ProductId,
            PromotionFileInfoId = createRequest.PromotionFileInfoId,
            Active = createRequest.Active,
            ValidFrom = createRequest.ValidFrom,
            ValidTo = createRequest.ValidTo,
            ProductImageId = productImageId,
            CreateUserName = createRequest.CreateUserName,
        };

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<int, ValidationResult, UnexpectedFailureResult> createPromotionProductFileResult
            = await _promotionProductFileInfoService.InsertAsync(promotionProductFileInfoCreateRequest);

        if (!createPromotionProductFileResult.IsT0)
        {
            return createPromotionProductFileResult.Map<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                createRequest.ProductId, ProductNewStatus.WorkInProgress, createRequest.CreateUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                statusId => createPromotionProductFileResult.AsT0,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        localDBTransactionScope.Complete();

        replicationDBTransactionScope.Complete();

        return createPromotionProductFileResult.AsT0;
    }

    //private async Task<OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInternalAsync(
    //    ServicePromotionProductFileCreateRequest createRequest,
    //    Product product,
    //    string fileName,
    //    Stream fileDataStream)
    //{
    //    int? productImageId = null;

    //    if (createRequest.CreateInProductImagesRequest is not null)
    //    {
    //        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

    //        OneOf<ImageAndFileIdsInfo, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> insertImageAndImageFileResult
    //            = await InsertImageAndImageFileAsync(product, fileName, fileDataStream, createRequest.CreateInProductImagesRequest, createRequest.CreateUserName);

    //        if (!insertImageAndImageFileResult.IsT0)
    //        {
    //            return insertImageAndImageFileResult.Match<OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //                imageAndFileIdsInfo => new UnexpectedFailureResult(),
    //                validationResult => validationResult,
    //                fileAlreadyExistsResult => fileAlreadyExistsResult,
    //                unexpectedFailureResult => unexpectedFailureResult);
    //        }

    //        int newImageId = insertImageAndImageFileResult.AsT0.ImageId;

    //        productImageId = newImageId;
    //    }

    //    ServicePromotionProductFileInfoCreateRequest promotionProductFileInfoCreateRequest = new()
    //    {
    //        ProductId = createRequest.ProductId,
    //        PromotionFileInfoId = createRequest.PromotionFileInfoId,
    //        Active = createRequest.Active,
    //        ValidFrom = createRequest.ValidFrom,
    //        ValidTo = createRequest.ValidTo,
    //        ProductImageId = productImageId,
    //        CreateUserName = createRequest.CreateUserName,
    //    };

    //    OneOf<int, ValidationResult, UnexpectedFailureResult> createPromotionProductFileResult
    //        = await _promotionProductFileInfoService.InsertAsync(promotionProductFileInfoCreateRequest);

    //    if (!createPromotionProductFileResult.IsT0)
    //    {
    //        return createPromotionProductFileResult.Map<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
    //    }

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //            createRequest.ProductId, ProductNewStatus.WorkInProgress, createRequest.CreateUserName);

    //    return upsertProductStatusResult.Match<OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //        statusId => createPromotionProductFileResult.AsT0,
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    public async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateAsync(
        ServicePromotionProductFileUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => UpdateInternalAsync(updateRequest),
        //    result => result.Match(
        //        success => true,
        //        validationResult => false,
        //        fileDoesntExistResult => false,
        //        fileAlreadyExistsResult => false,
        //        unexpectedFailureResult => false));

        return await UpdateInternalAsync(updateRequest);
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateInternalAsync(
        ServicePromotionProductFileUpdateRequest updateRequest)
    {
        PromotionProductFileInfo? promotionProductFile = await _promotionProductFileInfoService.GetByIdAsync(updateRequest.Id);

        if (promotionProductFile is null)
        {
            ValidationFailure validationFailure = new(nameof(updateRequest.Id), _invalidPromotionProductFileIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        PromotionFileInfo? currentPromotionFile = await _promotionFileService.GetByIdAsync(promotionProductFile.PromotionFileInfoId);

        if (currentPromotionFile is null) return new UnexpectedFailureResult();

        Stream? currentPromotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(promotionProductFile.PromotionFileInfoId);

        PromotionFileInfo? newPromotionFile = null;

        Stream? newPromotionFileDataStream = null;

        if (updateRequest.NewPromotionFileInfoId is not null)
        {
            newPromotionFile = await _promotionFileService.GetByIdAsync(updateRequest.NewPromotionFileInfoId.Value);

            if (newPromotionFile is null)
            {
                ValidationFailure validationFailure = new(nameof(updateRequest.NewPromotionFileInfoId), _invalidPromotionFileIdErrorMessage);

                return CreateValidationResultFromErrors(validationFailure);
            }

            newPromotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(updateRequest.NewPromotionFileInfoId.Value);
        }

        int? productImageId = null;

        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        if (updateRequest.UpsertInProductImagesRequest is not null)
        {
            PromotionFileInfo promotionFileAfterUpdate;
            Stream? promotionFileAfterUpdateDataStream;

            if (newPromotionFile is not null)
            {
                promotionFileAfterUpdate = newPromotionFile;

                promotionFileAfterUpdateDataStream = newPromotionFileDataStream;
            }
            else
            {
                promotionFileAfterUpdate = currentPromotionFile;

                promotionFileAfterUpdateDataStream = currentPromotionFileDataStream;
            }

            if (promotionFileAfterUpdateDataStream is null) return new UnexpectedFailureResult();

            using MemoryStream memoryStream = new();

            promotionFileAfterUpdateDataStream.CopyTo(memoryStream);

            byte[] fileData = memoryStream.ToArray();

            Product? product = await _productRepository.GetByIdAsync(promotionProductFile.ProductId);

            if (product is null) return new UnexpectedFailureResult();

            string fileExtension = Path.GetExtension(promotionFileAfterUpdate.FileName);

            string? contentType = GetContentTypeFromExtension(fileExtension);

            if (!IsImageContentType(contentType))
            {
                ValidationFailure validationFailure = new(nameof(PromotionFileInfo.FileName), _promotionFileIsNotAnImageErrorMessage);

                return CreateValidationResultFromErrors(validationFailure);
            }

            HtmlProductsData htmlProductsData = await _productToHtmlProductService.GetHtmlProductDataFromProductsAsync(product);

            ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest = new()
            {
                ProductId = promotionProductFile.ProductId,
                ExistingImageId = promotionProductFile.ProductImageId,
                FileExtension = fileExtension,
                ImageData = fileData,
                HtmlData = updateRequest.UpsertInProductImagesRequest.HtmlData,
                FileUpsertRequest = new()
                {
                    Active = updateRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.Active ?? false,
                    CustomDisplayOrder = updateRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.CustomDisplayOrder,
                    UpsertUserName = updateRequest.UpdateUserName,
                },
            };

            OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateImageResult
                = await _productImageAndFileService.UpsertInAllImagesWithFileAsync(productImageWithFileUpsertRequest);

            if (!updateImageResult.IsT0)
            {
                return updateImageResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    imageAndFileIdsInfo => new UnexpectedFailureResult(),
                    validationResult => validationResult,
                    fileSaveFailureResult => fileSaveFailureResult,
                    fileDoesntExistResult => fileDoesntExistResult,
                    fileAlreadyExistsResult => fileAlreadyExistsResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }

            productImageId = updateImageResult.AsT0.ImageId;
        }
        else if (promotionProductFile.ProductImageId is not null)
        {
            OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult> imageAndFileDeleteResult = await _productImageAndFileService.DeleteInAllImagesByIdWithFileAsync(
                promotionProductFile.ProductImageId.Value, updateRequest.UpdateUserName);

            if (!imageAndFileDeleteResult.IsT0)
            {
                return imageAndFileDeleteResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    success => new UnexpectedFailureResult(),
                    notFound => new UnexpectedFailureResult(),
                    fileDoesntExistResult => fileDoesntExistResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            }
        }

        ServicePromotionProductFileInfoUpdateRequest promotionProductFileInfoUpdateRequest = new()
        {
            Id = updateRequest.Id,
            NewPromotionFileInfoId = updateRequest.NewPromotionFileInfoId,
            ValidFrom = updateRequest.ValidFrom,
            ValidTo = updateRequest.ValidTo,
            Active = updateRequest.Active,
            ProductImageId = productImageId,
            UpdateUserName = updateRequest.UpdateUserName,
        };

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, NotFound, ValidationResult> updatePromotionProductFileResult
            = await _promotionProductFileInfoService.UpdateAsync(promotionProductFileInfoUpdateRequest);

        if (!updatePromotionProductFileResult.IsT0)
        {
            return updatePromotionProductFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => success,
                notFound => new UnexpectedFailureResult(),
                validationResult => validationResult);
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                promotionProductFile.ProductId, ProductNewStatus.WorkInProgress, updateRequest.UpdateUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                statusId => updatePromotionProductFileResult.AsT0,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        localDBTransactionScope.Complete();

        replicationDBTransactionScope.Complete();

        return updatePromotionProductFileResult.AsT0;
    }

    //private async Task<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateInternalAsync(
    //    ServicePromotionProductFileUpdateRequest updateRequest)
    //{
    //    PromotionProductFileInfo? promotionProductFile = await _promotionProductFileInfoService.GetByIdAsync(updateRequest.Id);

    //    if (promotionProductFile is null)
    //    {
    //        ValidationFailure validationFailure = new(nameof(updateRequest.Id), _invalidPromotionProductFileIdErrorMessage);

    //        return CreateValidationResultFromErrors(validationFailure);
    //    }

    //    PromotionFileInfo? currentPromotionFile = await _promotionFileService.GetByIdAsync(promotionProductFile.PromotionFileInfoId);

    //    if (currentPromotionFile is null) return new UnexpectedFailureResult();

    //    Stream? currentPromotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(promotionProductFile.PromotionFileInfoId);

    //    PromotionFileInfo? newPromotionFile = null;

    //    Stream? newPromotionFileDataStream = null;

    //    if (updateRequest.NewPromotionFileInfoId is not null)
    //    {
    //        newPromotionFile = await _promotionFileService.GetByIdAsync(updateRequest.NewPromotionFileInfoId.Value);

    //        if (newPromotionFile is null)
    //        {
    //            ValidationFailure validationFailure = new(nameof(updateRequest.NewPromotionFileInfoId), _invalidPromotionFileIdErrorMessage);

    //            return CreateValidationResultFromErrors(validationFailure);
    //        }

    //        newPromotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(updateRequest.NewPromotionFileInfoId.Value);
    //    }

    //    int? productImageId = null;

    //    if (updateRequest.UpsertInProductImagesRequest is not null)
    //    {
    //        PromotionFileInfo promotionFileAfterUpdate;
    //        Stream? promotionFileAfterUpdateDataStream;

    //        if (newPromotionFile is not null)
    //        {
    //            promotionFileAfterUpdate = newPromotionFile;

    //            promotionFileAfterUpdateDataStream = newPromotionFileDataStream;
    //        }
    //        else
    //        {
    //            promotionFileAfterUpdate = currentPromotionFile;

    //            promotionFileAfterUpdateDataStream = currentPromotionFileDataStream;
    //        }

    //        if (promotionFileAfterUpdateDataStream is null) return new UnexpectedFailureResult();

    //        using MemoryStream memoryStream = new();

    //        promotionFileAfterUpdateDataStream.CopyTo(memoryStream);

    //        byte[] fileData = memoryStream.ToArray();

    //        Product? product = await _productRepository.GetByIdAsync(promotionProductFile.ProductId);

    //        if (product is null) return new UnexpectedFailureResult();

    //        string fileExtension = Path.GetExtension(promotionFileAfterUpdate.FileName);

    //        string? contentType = GetContentTypeFromExtension(fileExtension);

    //        if (!IsImageContentType(contentType))
    //        {
    //            ValidationFailure validationFailure = new(nameof(PromotionFileInfo.FileName), _promotionFileIsNotAnImageErrorMessage);

    //            return CreateValidationResultFromErrors(validationFailure);
    //        }

    //        HtmlProductsData htmlProductsData = await _productToHtmlProductService.GetHtmlProductDataFromProductsAsync(product);

    //        ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest = new()
    //        {
    //            ProductId = promotionProductFile.ProductId,
    //            ExistingImageId = promotionProductFile.ProductImageId,
    //            FileExtension = fileExtension,
    //            ImageData = fileData,
    //            HtmlData = updateRequest.UpsertInProductImagesRequest.HtmlData,
    //            FileUpsertRequest = new()
    //            {
    //                Active = updateRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.Active ?? false,
    //                CustomDisplayOrder = updateRequest.UpsertInProductImagesRequest.ImageFileUpsertRequest.CustomDisplayOrder,
    //                UpsertUserName = updateRequest.UpdateUserName,
    //            },
    //        };

    //        OneOf<ImageAndFileIdsInfo, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateImageResult
    //            = await _productImageAndFileService.UpsertInAllImagesWithFileAsync(productImageWithFileUpsertRequest);

    //        if (!updateImageResult.IsT0)
    //        {
    //            return updateImageResult.Match<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //                imageAndFileIdsInfo => new UnexpectedFailureResult(),
    //                validationResult => validationResult,
    //                fileDoesntExistResult => fileDoesntExistResult,
    //                fileAlreadyExistsResult => fileAlreadyExistsResult,
    //                unexpectedFailureResult => unexpectedFailureResult);
    //        }

    //        productImageId = updateImageResult.AsT0.ImageId;
    //    }
    //    else if (promotionProductFile.ProductImageId is not null)
    //    {
            //bool isImageDeleted = await _productImageAndFileService.DeleteInAllImagesByIdAsync(promotionProductFile.ProductImageId.Value);

            //if (!isImageDeleted) return new UnexpectedFailureResult();

            //OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> fileDeleteResult
            //    = await _productImageFileService.DeleteFileByProductIdAndImageIdAsync(
            //        promotionProductFile.ProductId, promotionProductFile.ProductImageId.Value, updateRequest.UpdateUserName);

            //if (!fileDeleteResult.IsT0 && !fileDeleteResult.IsT1)
            //{
            //    return fileDeleteResult.Match<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
            //        success => new UnexpectedFailureResult(),
            //        notFound => new UnexpectedFailureResult(),
            //        fileDoesntExistResult => fileDoesntExistResult,
            //        validationResult => validationResult,
            //        unexpectedFailureResult => unexpectedFailureResult);
            //}
    //    }

    //    ServicePromotionProductFileInfoUpdateRequest promotionProductFileInfoUpdateRequest = new()
    //    {
    //        Id = updateRequest.Id,
    //        NewPromotionFileInfoId = updateRequest.NewPromotionFileInfoId,
    //        ValidFrom = updateRequest.ValidFrom,
    //        ValidTo = updateRequest.ValidTo,
    //        Active = updateRequest.Active,
    //        ProductImageId = productImageId,
    //        UpdateUserName = updateRequest.UpdateUserName,
    //    };

    //    OneOf<Success, NotFound, ValidationResult> updatePromotionProductFileResult
    //        = await _promotionProductFileInfoService.UpdateAsync(promotionProductFileInfoUpdateRequest);

    //    if (!updatePromotionProductFileResult.IsT0)
    //    {
    //        return updatePromotionProductFileResult.Match<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //            success => success,
    //            notFound => new UnexpectedFailureResult(),
    //            validationResult => validationResult);
    //    }

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //            promotionProductFile.ProductId, ProductNewStatus.WorkInProgress, updateRequest.UpdateUserName);

    //    return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
    //        statusId => updatePromotionProductFileResult.AsT0,
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}

    private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertImageAndImageFileAsync(
        Product product,
        string fileName,
        Stream fileDataStream,
        ServicePromotionProductImageCreateRequest createInImagesRequest,
        string createUserName)
    {
        using MemoryStream memoryStream = new();

        fileDataStream.CopyTo(memoryStream);

        byte[] fileData = memoryStream.ToArray();

        if (fileData is null)
        {
            ValidationFailure validationFailure = new(nameof(fileData), _promotionFileDoesNotExistErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }
        
        string fileExtension = Path.GetExtension(fileName);

        string? contentType = GetContentTypeFromExtension(fileExtension);

        if (!IsImageContentType(contentType))
        {
            ValidationFailure validationFailure = new(nameof(PromotionFileInfo.FileName), _promotionFileIsNotAnImageErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        ProductImageWithFileCreateRequest productImageFileCreateRequest = new()
        {
            ProductId = product.Id,
            HtmlData = createInImagesRequest.HtmlData,
            ImageData = fileData,
            FileExtension = fileExtension,

            Active = createInImagesRequest.ImageFileCreateRequest.Active,
            CustomDisplayOrder = createInImagesRequest.ImageFileCreateRequest.CustomDisplayOrder,
            CreateUserName = createUserName,
        };

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> insertImageWithFileResult
            = await _productImageAndFileService.InsertInAllImagesWithFileAsync(productImageFileCreateRequest);

        return insertImageWithFileResult;
    }

    public async Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>> DeleteAsync(int id, string deleteUserName)
    {
        //return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
        //    () => DeleteInternalAsync(id, deleteUserName),
        //    result => result.Match(
        //        success => true,
        //        notFound => false,
        //        validationResult => false,
        //        fileDoesntExistResult => false,
        //        unexpectedFailureResult => false
        //    ));

        return await DeleteInternalAsync(id, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInternalAsync(int id, string deleteUserName)
    {
        PromotionProductFileInfo? promotionProductFile = await _promotionProductFileInfoService.GetByIdAsync(id);

        if (promotionProductFile is null) return new NotFound();

        PromotionFileInfo? promotionFile = await _promotionFileService.GetByIdAsync(promotionProductFile.PromotionFileInfoId);

        if (promotionFile is null) return new UnexpectedFailureResult();

        using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        bool promotionProductFileDeleteSuccess = await _promotionProductFileInfoService.DeleteAsync(id);

        if (!promotionProductFileDeleteSuccess) return new UnexpectedFailureResult();

        if (promotionProductFile.ProductImageId is null) return new Success();

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                promotionProductFile.ProductId, ProductNewStatus.WorkInProgress, deleteUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                statusId => new Success(),
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

        OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult> imageAndFileDeleteResult
            = await _productImageAndFileService.DeleteInAllImagesByIdWithFileAsync(
                promotionProductFile.ProductImageId.Value, deleteUserName);

        if (!imageAndFileDeleteResult.IsT0)
        {
            return imageAndFileDeleteResult.Match<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                success => new UnexpectedFailureResult(),
                notFound => notFound,
                fileDoesntExistResult => fileDoesntExistResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        replicationDBTransactionScope.Complete();

        localDBTransactionScope.Complete();

        return imageAndFileDeleteResult.AsT0;
    }

    //private async Task<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInternalAsync(int id, string deleteUserName)
    //{
    //    PromotionProductFileInfo? promotionProductFile = await _promotionProductFileInfoService.GetByIdAsync(id);

    //    if (promotionProductFile is null) return new NotFound();

    //    PromotionFileInfo? promotionFile = await _promotionFileService.GetByIdAsync(promotionProductFile.PromotionFileInfoId);

    //    if (promotionFile is null) return new UnexpectedFailureResult();

    //    using TransactionScope localDBTransactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

    //    bool promotionProductFileDeleteSuccess = await _promotionProductFileInfoService.DeleteAsync(id);

    //    if (!promotionProductFileDeleteSuccess) return new UnexpectedFailureResult();

    //    if (promotionProductFile.ProductImageId is null) return new Success();

    //    using TransactionScope replicationDBTransactionScope = new(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled);

    //    bool isImageDeleted = await _productImageAndFileService.DeleteInAllImagesByIdAsync(promotionProductFile.ProductImageId.Value);

    //    if (!isImageDeleted) return new NotFound();

    //    OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> fileDeleteResult
    //        = await _productImageFileService.DeleteFileByProductIdAndImageIdAsync(
    //            promotionProductFile.ProductId, promotionProductFile.ProductImageId.Value, deleteUserName);

    //    if (!fileDeleteResult.IsT0 && !fileDeleteResult.IsT1)
    //    {
    //        return fileDeleteResult.Match<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>>(
    //            success => new UnexpectedFailureResult(),
    //            notFound => new UnexpectedFailureResult(),
    //            fileDoesntExistResult => fileDoesntExistResult,
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);
    //    }

    //    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
    //        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
    //            promotionProductFile.ProductId, ProductNewStatus.WorkInProgress, deleteUserName);

    //    return upsertProductStatusResult.Match<OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>>(
    //        statusId => new Success(),
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);
    //}
}