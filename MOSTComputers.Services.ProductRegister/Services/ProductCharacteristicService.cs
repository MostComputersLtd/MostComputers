using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductCharacteristicService : IProductCharacteristicService
{
    public ProductCharacteristicService(
        IProductCharacteristicsRepository productCharacteristicsRepository,
        IValidator<ProductCharacteristicCreateRequest>? createRequestValidator = null,
        IValidator<ProductCharacteristicByIdUpdateRequest>? updateByIdRequestValidator = null,
        IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? updateByNameAndCategoryIdRequestValidator = null)
    {
        _productCharacteristicsRepository = productCharacteristicsRepository;
        _createRequestValidator = createRequestValidator;
        _updateByIdRequestValidator = updateByIdRequestValidator;
        _updateByNameAndCategoryIdRequestValidator = updateByNameAndCategoryIdRequestValidator;
    }

    private readonly IProductCharacteristicsRepository _productCharacteristicsRepository;
    private readonly IValidator<ProductCharacteristicCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductCharacteristicByIdUpdateRequest>? _updateByIdRequestValidator;
    private readonly IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? _updateByNameAndCategoryIdRequestValidator;

    public IEnumerable<ProductCharacteristic> GetAllByCategoryId(uint categoryId)
    {
        return _productCharacteristicsRepository.GetAllByCategoryId(categoryId);
    }

    public ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name)
    {
        return _productCharacteristicsRepository.GetByCategoryIdAndName(categoryId, name);
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _productCharacteristicsRepository.Insert(createRequest);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByIdUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateByIdRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _productCharacteristicsRepository.UpdateById(updateRequest);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? validator = null)
    {

        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateByNameAndCategoryIdRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        return _productCharacteristicsRepository.UpdateByNameAndCategoryId(updateRequest);
    }

    public bool Delete(uint id)
    {
        return _productCharacteristicsRepository.Delete(id);
    }

    public bool DeleteAllForCategory(uint categoryId)
    {
        return _productCharacteristicsRepository.DeleteAllForCategory(categoryId);
    }
}