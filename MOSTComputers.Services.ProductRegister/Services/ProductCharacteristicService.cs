using FluentValidation;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;

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

    public IEnumerable<ProductCharacteristic> GetAllByCategoryId(int categoryId)
    {
        return _productCharacteristicsRepository.GetAllCharacteristicsAndSearchStringAbbreviationsByCategoryId(categoryId);
    }

    public IEnumerable<ProductCharacteristic> GetCharacteristicsOnlyByCategoryId(int categoryId)
    {
        return _productCharacteristicsRepository.GetAllCharacteristicsByCategoryId(categoryId);
    }

    public IEnumerable<ProductCharacteristic> GetSearchStringAbbreviationsOnlyByCategoryId(int categoryId)
    {
        return _productCharacteristicsRepository.GetAllSearchStringAbbreviationsByCategoryId(categoryId);
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetAllForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        return _productCharacteristicsRepository.GetCharacteristicsAndSearchStringAbbreviationsForSelectionOfCategoryIds(categoryIds);
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetCharacteristicsOnlyForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        return _productCharacteristicsRepository.GetCharacteristicsForSelectionOfCategoryIds(categoryIds);
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        return _productCharacteristicsRepository.GetSearchStringAbbreviationsForSelectionOfCategoryIds(categoryIds);
    }

    public ProductCharacteristic? GetById(int id)
    {
        if (id <= 0) return null;

        return _productCharacteristicsRepository.GetById(id);
    }

    public ProductCharacteristic? GetByCategoryIdAndName(int categoryId, string name)
    {
        return _productCharacteristicsRepository.GetByCategoryIdAndName(categoryId, name);
    }

    public IEnumerable<ProductCharacteristic> GetSelectionByCategoryIdAndNames(int categoryId, List<string> names)
    {
        return _productCharacteristicsRepository.GetSelectionByCategoryIdAndNames(categoryId, names);
    }

    public IEnumerable<ProductCharacteristic> GetSelectionByCharacteristicIds(IEnumerable<int> ids)
    {
        if (!ids.Any()) return Enumerable.Empty<ProductCharacteristic>();

        List<int> uniqueIds = new();

        foreach (int id in ids)
        {
            if (id <= 0 || uniqueIds.Contains(id)) continue;

            uniqueIds.Add(id);
        }

        return _productCharacteristicsRepository.GetSelectionByIds(uniqueIds);
    }

    public ProductCharacteristic? GetByCategoryIdAndNameAndCharacteristicType(
        int categoryId,
        string name,
        ProductCharacteristicTypeEnum productCharacteristicType)
    {
        return _productCharacteristicsRepository.GetByCategoryIdAndNameAndCharacteristicType(categoryId, name, productCharacteristicType);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null)
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

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        return _productCharacteristicsRepository.Delete(id);
    }

    public bool DeleteAllForCategory(int categoryId)
    {
        return _productCharacteristicsRepository.DeleteAllForCategory(categoryId);
    }
}