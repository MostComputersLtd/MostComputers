using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Utils.OneOf;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionProductFiles;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles;
internal sealed class PromotionProductFileInfoService : IPromotionProductFileInfoService
{
    public PromotionProductFileInfoService(IPromotionProductFileInfoRepository promotionProductFileInfoRepository,
        IPromotionFileService promotionFileService,
        IProductRepository productRepository,
        IProductWorkStatusesWorkflowService productWorkStatusesWorkflowService,
        IValidator<ServicePromotionProductFileInfoCreateRequest>? createRequestValidator = null,
        IValidator<ServicePromotionProductFileInfoUpdateRequest>? updateRequestValidator = null)
    {
        _promotionProductFileInfoRepository = promotionProductFileInfoRepository;
        _promotionFileService = promotionFileService;
        _productRepository = productRepository;
        _productWorkStatusesWorkflowService = productWorkStatusesWorkflowService;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private const string _invalidProductIdErrorMessage = "Id does not correspond to any known product id";
    private const string _invalidPromotionFileIdErrorMessage = "Id does not correspond to any known promotion file id";
    private const string _invalidPromotionProductFileIdErrorMessage = "Id does not correspond to any known promotion product file id";

    private readonly IPromotionProductFileInfoRepository _promotionProductFileInfoRepository;

    private readonly IPromotionFileService _promotionFileService;
    private readonly IProductRepository _productRepository;
    private readonly IProductWorkStatusesWorkflowService _productWorkStatusesWorkflowService;
    private readonly IValidator<ServicePromotionProductFileInfoCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServicePromotionProductFileInfoUpdateRequest>? _updateRequestValidator;

    public async Task<List<IGrouping<int, PromotionProductFileInfo>>> GetAllForProductsAsync(IEnumerable<int> productIds)
    {
        return await _promotionProductFileInfoRepository.GetAllForProductsAsync(productIds);
    }

    public async Task<List<PromotionProductFileInfoForProductCountData>> GetCountOfAllForProductsAsync(IEnumerable<int> productIds)
    {
        return await _promotionProductFileInfoRepository.GetCountOfAllForProductsAsync(productIds);
    }

    public async Task<List<PromotionProductFileInfo>> GetAllForProductAsync(int productId)
    {
        return await _promotionProductFileInfoRepository.GetAllForProductAsync(productId);
    }

    public async Task<int> GetCountOfAllForProductAsync(int productId)
    {
        return await _promotionProductFileInfoRepository.GetCountOfAllForProductAsync(productId);
    }

    public async Task<PromotionProductFileInfo?> GetByIdAsync(int id)
    {
        return await _promotionProductFileInfoRepository.GetByIdAsync(id);
    }

    public async Task<bool> DoesExistForPromotionFileAsync(int promotionFileId)
    {
        return await _promotionProductFileInfoRepository.DoesExistForPromotionFileAsync(promotionFileId);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServicePromotionProductFileInfoCreateRequest createRequest)
    {
        if (_createRequestValidator is not null)
        {
            ValidationResult validationResult = _createRequestValidator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        Product? product = await _productRepository.GetByIdAsync(createRequest.ProductId);

        if (product is null)
        {
            ValidationFailure validationFailure = new(nameof(createRequest.ProductId), _invalidProductIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        PromotionFileInfo? promotionFile = await _promotionFileService.GetByIdAsync(createRequest.PromotionFileInfoId);

        if (promotionFile is null)
        {
            ValidationFailure validationFailure = new(nameof(createRequest.PromotionFileInfoId), _invalidPromotionFileIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        DateTime createDate = DateTime.Now;

        PromotionProductFileInfoCreateRequest promotionProductFileInfoCreateRequest = new()
        {
            ProductId = createRequest.ProductId,
            PromotionFileInfoId = createRequest.PromotionFileInfoId,
            Active = createRequest.Active,
            ValidFrom = createRequest.ValidFrom,
            ValidTo = createRequest.ValidTo,
            ProductImageId = createRequest.ProductImageId,
            CreateUserName = createRequest.CreateUserName,
            CreateDate = createDate,
            LastUpdateUserName = createRequest.CreateUserName,
            LastUpdateDate = createDate,
        };

        OneOf<int, UnexpectedFailureResult> createPromotionProductFileResult
            = await _promotionProductFileInfoRepository.InsertAsync(promotionProductFileInfoCreateRequest);

        
        return createPromotionProductFileResult.Map<int, ValidationResult, UnexpectedFailureResult>();
    }

    public async Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServicePromotionProductFileInfoUpdateRequest updateRequest)
    {
        if (_updateRequestValidator is not null)
        {
            ValidationResult validationResult = _updateRequestValidator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        PromotionProductFileInfo? promotionProductFile = await _promotionProductFileInfoRepository.GetByIdAsync(updateRequest.Id);

        if (promotionProductFile is null)
        {
            ValidationFailure validationFailure = new(nameof(updateRequest.Id), _invalidPromotionProductFileIdErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        if (updateRequest.NewPromotionFileInfoId is not null)
        {
            PromotionFileInfo? newPromotionFile = await _promotionFileService.GetByIdAsync(updateRequest.NewPromotionFileInfoId.Value);

            if (newPromotionFile is null)
            {
                ValidationFailure validationFailure = new(nameof(updateRequest.NewPromotionFileInfoId), _invalidPromotionFileIdErrorMessage);

                return CreateValidationResultFromErrors(validationFailure);
            }
        }

        DateTime updateDate = DateTime.Now;

        PromotionProductFileInfoUpdateRequest promotionProductFileInfoUpdateRequest = new()
        {
            Id = updateRequest.Id,
            NewPromotionFileInfoId = updateRequest.NewPromotionFileInfoId,
            ValidFrom = updateRequest.ValidFrom,
            ValidTo = updateRequest.ValidTo,
            Active = updateRequest.Active,
            ProductImageId = updateRequest.ProductImageId,
            LastUpdateUserName = updateRequest.UpdateUserName,
            LastUpdateDate = updateDate,
        };

        OneOf<Success, NotFound> updatePromotionProductFileResult = await _promotionProductFileInfoRepository.UpdateAsync(promotionProductFileInfoUpdateRequest);

        return updatePromotionProductFileResult.Map<Success, NotFound, ValidationResult>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _promotionProductFileInfoRepository.DeleteAsync(id);
    }
}