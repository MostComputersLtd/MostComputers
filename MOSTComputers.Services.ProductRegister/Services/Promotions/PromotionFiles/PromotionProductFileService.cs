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
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
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
using static MOSTComputers.Utils.OneOf.AsyncMatchingExtensions;
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
        IProductHtmlService productHtmlService,
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
        _productHtmlService = productHtmlService;
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
    private readonly IProductHtmlService _productHtmlService;
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

        using Stream? promotionFileDataStream = await _promotionFileService.GetFileDataByIdAsync(createRequest.PromotionFileInfoId);

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

    private async Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertImageAndImageFileAsync(
        Product product,
        string fileName,
        Stream fileDataStream,
        ServicePromotionProductImageCreateRequest createInImagesRequest,
        string createUserName)
    {
        using MemoryStream memoryStream = new();

        await fileDataStream.CopyToAsync(memoryStream);

        byte[] fileData = memoryStream.ToArray();

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
}
