using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Mapping;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.ProductRegister.Models.Requests.Promotion;
using MOSTComputers.Services.DAL.Models.Requests.Promotion;

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

    public IEnumerable<Promotion> GetAllForProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<Promotion>();

        return _promotionRepository.GetAllForProduct(productId);
    }

    public IEnumerable<Promotion> GetAllForSelectionOfProducts(List<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return _promotionRepository.GetAllForSelectionOfProducts(productIds);
    }

    public IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return _promotionRepository.GetAllActiveForSelectionOfProducts(productIds);
    }

    public Promotion? GetActiveForProduct(int productId)
    {
        if (productId <= 0) return null;

        return _promotionRepository.GetActiveForProduct(productId);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ServicePromotionCreateRequest createRequest, IValidator<ServicePromotionCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        PromotionCreateRequest createRequestInternal = _productMapper.Map(createRequest);

        createRequestInternal.PromotionAddedDate = DateTime.Today;

        OneOf<int, UnexpectedFailureResult> result = _promotionRepository.Insert(createRequestInternal);

        return result.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id, unexpectedFailure => unexpectedFailure);
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
    public bool Delete(int id)
    {
        if (id <= 0) return false;

        return _promotionRepository.Delete(id);
    }

    public bool DeleteAllByProductId(int productId)
    {
        if (productId <= 0) return false;

        return _promotionRepository.DeleteAllByProductId(productId);
    }
}