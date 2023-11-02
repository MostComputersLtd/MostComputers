using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductCharacteristicService
{
    IEnumerable<ProductCharacteristic> GetAllByCategoryId(uint categoryId);
    ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByIdUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? validator = null);
    bool Delete(uint id);
    bool DeleteAllForCategory(uint productId);
}