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
    IEnumerable<ProductCharacteristic> GetAllByCategoryId(uint categoryId);
    IEnumerable<IGrouping<uint, ProductCharacteristic>> GetAllForSelectionOfCategoryIds(IEnumerable<uint> categoryIds);
    ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name);
    IEnumerable<ProductCharacteristic> GetSelectionByCategoryIdAndNames(uint categoryId, List<string> names);
    OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByIdUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? validator = null);
    bool Delete(uint id);
    bool DeleteAllForCategory(uint categoryId);
}