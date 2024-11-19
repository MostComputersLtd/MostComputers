using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using FluentValidation;
using FluentValidation.Results;
using OneOf;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class FailedPropertyNameOfProductService : IFailedPropertyNameOfProductService
{
    public FailedPropertyNameOfProductService(
        IFailedPropertyNameOfProductRepository failedPropertyNameOfProductRepository,
        IValidator<FailedPropertyNameOfProductCreateRequest> createRequestValidator,
        IValidator<FailedPropertyNameOfProductMultiCreateRequest> multiCreateRequestValidator,
        IValidator<FailedPropertyNameOfProductUpdateRequest> updateRequestValidator)
    {
        _failedPropertyNameOfProductRepository = failedPropertyNameOfProductRepository;
        _createRequestValidator = createRequestValidator;
        _multiCreateRequestValidator = multiCreateRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IFailedPropertyNameOfProductRepository _failedPropertyNameOfProductRepository;
    private readonly IValidator<FailedPropertyNameOfProductCreateRequest> _createRequestValidator;
    private readonly IValidator<FailedPropertyNameOfProductMultiCreateRequest> _multiCreateRequestValidator;
    private readonly IValidator<FailedPropertyNameOfProductUpdateRequest> _updateRequestValidator;

    public IEnumerable<FailedPropertyNameOfProduct> GetAll()
    {
        return _failedPropertyNameOfProductRepository.GetAll();
    }

    public IEnumerable<FailedPropertyNameOfProduct> GetAllForProduct(int productId)
    {
        if (productId <= 0) return Enumerable.Empty<FailedPropertyNameOfProduct>();

        return _failedPropertyNameOfProductRepository.GetAllForProduct(productId);
    }

    public IEnumerable<FailedPropertyNameOfProduct> GetAllForSelectionOfProducts(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return _failedPropertyNameOfProductRepository.GetAllForSelectionOfProducts(productIds);
    }

    public OneOf<bool, ValidationResult> Insert(FailedPropertyNameOfProductCreateRequest createRequest)
    {
        ValidationResult validationResult = _createRequestValidator.Validate(createRequest);

        if (!validationResult.IsValid) return validationResult;

        return _failedPropertyNameOfProductRepository.Insert(createRequest);
    }

    public OneOf<bool, ValidationResult> MultiInsert(FailedPropertyNameOfProductMultiCreateRequest createRequest)
    {
        ValidationResult validationResult = _multiCreateRequestValidator.Validate(createRequest);

        if (!validationResult.IsValid) return validationResult;

        return _failedPropertyNameOfProductRepository.MultiInsert(createRequest);
    }

    public OneOf<bool, ValidationResult> Update(FailedPropertyNameOfProductUpdateRequest updateRequest)
    {
        ValidationResult validationResult = _updateRequestValidator.Validate(updateRequest);

        if (!validationResult.IsValid) return validationResult;

        return _failedPropertyNameOfProductRepository.Update(updateRequest);
    }

    public bool Delete(int productId, string propertyName)
    {
        if (productId <= 0) return false;

        return _failedPropertyNameOfProductRepository.Delete(productId, propertyName);
    }

    public bool DeleteAllForProduct(int productId)
    {
        if (productId <= 0) return false;

        return _failedPropertyNameOfProductRepository.DeleteAllForProduct(productId);
    }

    public bool DeleteAllForSelectionOfProducts(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return _failedPropertyNameOfProductRepository.DeleteAllForSelectionOfProducts(productIds);
    }
}