using FluentValidation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductPropertyService : IProductPropertyService
{
    public ProductPropertyService(
        IProductPropertyRepository productPropertyRepository,
        IValidator<ProductPropertyCreateRequest>? createRequestValidator = null,
        IValidator<ProductPropertyUpdateRequest>? updateRequestValidator = null)
    {
        _productPropertyRepository = productPropertyRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IProductPropertyRepository _productPropertyRepository;
    private readonly IValidator<ProductPropertyCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductPropertyUpdateRequest>? _updateRequestValidator;

    public IEnumerable<ProductProperty> GetAllInProduct(uint productId)
    {
        return _productPropertyRepository.GetAllInProduct(productId);
    }

    public ProductProperty? GetByNameAndProductId(string name, uint productId)
    {
        return _productPropertyRepository.GetByNameAndProductId(name, productId);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductPropertyCreateRequest createRequest,
        IValidator<ProductPropertyCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyRepository.Insert(createRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, validationResult => validationResult, unexpectedResult => unexpectedResult);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest,
        IValidator<ProductPropertyUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _productPropertyRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, validationResult => validationResult, unexpectedResult => unexpectedResult);
    }

    public bool Delete(uint productId, uint characteristicId)
    {
        return _productPropertyRepository.Delete(productId, characteristicId);
    }

    public bool DeleteAllForProduct(uint productId)
    {
        return _productPropertyRepository.DeleteAllForProduct(productId);
    }

    public bool DeleteAllForCharacteristic(uint characteristicId)
    {
        return _productPropertyRepository.DeleteAllForCharacteristic(characteristicId);
    }
}