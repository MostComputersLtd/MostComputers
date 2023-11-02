using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductCharacteristicsRepository
{
    IEnumerable<ProductCharacteristic> GetAllByCategoryId(uint categoryId);
    ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest);
    bool Delete(uint id);
    bool DeleteAllForCategory(uint productId);
}