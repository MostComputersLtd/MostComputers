using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductPropertyRepository
{
    IEnumerable<ProductProperty> GetAllInProduct(uint productId);
    ProductProperty? GetByNameAndProductId(string name, uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductPropertyCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest);
    bool Delete(uint productId, uint characteristicId);
    bool DeleteAllForProduct(uint productId);
    bool DeleteAllForCharacteristic(uint characteristicId);
}