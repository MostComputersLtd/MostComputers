using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.Promotions;
using MOSTComputers.Services.ProductRegister.Mapping;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class PromotionService : IPromotionService
{
    public PromotionService(
        IPromotionRepository promotionRepository,
        ProductMapper productMapper,
        IValidator<ServicePromotionCreateRequest>? createRequestValidator = null,
        IValidator<ServicePromotionUpdateRequest>? updateRequestValidator = null)
    {
        _promotionRepository = promotionRepository;
        _productMapper = productMapper;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IPromotionRepository _promotionRepository;
    private readonly ProductMapper _productMapper;
    private readonly IValidator<ServicePromotionCreateRequest>? _createRequestValidator;
    private readonly IValidator<ServicePromotionUpdateRequest>? _updateRequestValidator;

    public IEnumerable<Promotion> GetAll()
    {
        return _promotionRepository.GetAll();
    }

    public IEnumerable<Promotion> GetAllActive()
    {
        return _promotionRepository.GetAllActive();
    }

    public IEnumerable<Promotion> GetAllForProduct(uint productId)
    {
        return _promotionRepository.GetAllForProduct(productId);
    }

    public IEnumerable<Promotion> GetAllForSelectionOfProducts(List<uint> productIds)
    {
        return _promotionRepository.GetAllForSelectionOfProducts(productIds);
    }

    public IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<uint> productIds)
    {
        return _promotionRepository.GetAllActiveForSelectionOfProducts(productIds);
    }

    public Promotion? GetActiveForProduct(uint productId)
    {
        return _promotionRepository.GetActiveForProduct(productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ServicePromotionCreateRequest createRequest, IValidator<ServicePromotionCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        PromotionCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.PromotionAddedDate = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _promotionRepository.Insert(createRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServicePromotionUpdateRequest updateRequest, IValidator<ServicePromotionUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        PromotionUpdateRequest updateRequestInternal = _productMapper.Map(updateRequest);

        updateRequestInternal.PromotionAddedDate = DateTime.Today;

        OneOf<Success, UnexpectedFailureResult> result = _promotionRepository.Update(updateRequestInternal);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }
    public bool Delete(uint id)
    {
        return _promotionRepository.Delete(id);
    }

    public bool DeleteAllByProductId(uint productId)
    {
        return _promotionRepository.DeleteAllByProductId(productId);
    }
}