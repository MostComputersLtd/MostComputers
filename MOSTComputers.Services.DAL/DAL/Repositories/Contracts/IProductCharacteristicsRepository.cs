using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

internal interface IProductCharacteristicsRepository
{
    bool Delete(uint id);
    IEnumerable<ProductCharacteristic> GetAllByCategoryId(uint categoryId);
    ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name);
    OneOf<Success, ValidationResult> Insert(ProductCharacteristicCreateRequest createRequest, IValidator<ProductCharacteristicCreateRequest>? validator = null);
    OneOf<Success, ValidationResult> UpdateById(ProductCharacteristicByIdUpdateRequest createRequest, IValidator<ProductCharacteristicByIdUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, IValidator<ProductCharacteristicByNameAndCategoryIdUpdateRequest>? validator = null);
}