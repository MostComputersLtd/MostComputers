using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductPropertyRepository
{
    IEnumerable<ProductProperty> GetAllInProduct(int productId);
    ProductProperty? GetByNameAndProductId(string name, int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(ProductPropertyByCharacteristicIdCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(ProductPropertyByCharacteristicNameCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest);
    bool Delete(int productId, int characteristicId);
    bool DeleteAllForProduct(int productId);
    bool DeleteAllForCharacteristic(int characteristicId);
}