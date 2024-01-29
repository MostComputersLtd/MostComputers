using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductCharacteristicService
{
    IEnumerable<ProductCharacteristic> GetAllByCategoryId(int categoryId);
    IEnumerable<ProductCharacteristic> GetCharacteristicsOnlyByCategoryId(int categoryId);
    IEnumerable<ProductCharacteristic> GetSearchStringAbbreviationsOnlyByCategoryId(int categoryId);
    IEnumerable<IGrouping<int, ProductCharacteristic>> GetAllForSelectionOfCategoryIds(IEnumerable<int> categoryIds);
    IEnumerable<IGrouping<int, ProductCharacteristic>> GetCharacteristicsOnlyForSelectionOfCategoryIds(IEnumerable<int> categoryIds);
    IEnumerable<IGrouping<int, ProductCharacteristic>> GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds(IEnumerable<int> categoryIds);
    ProductCharacteristic? GetById(uint id);
    ProductCharacteristic? GetByCategoryIdAndName(int categoryId, string name);
    IEnumerable<ProductCharacteristic> GetSelectionByCategoryIdAndNames(int categoryId, List<string> names);
    OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByIdUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? validator = null);
    bool Delete(uint id);
    bool DeleteAllForCategory(int categoryId);
}