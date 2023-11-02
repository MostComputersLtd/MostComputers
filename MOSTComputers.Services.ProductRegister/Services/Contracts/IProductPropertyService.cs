using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductPropertyService
{
    IEnumerable<ProductProperty> GetAllInProduct(uint productId);
    ProductProperty? GetByNameAndProductId(string name, uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductPropertyCreateRequest createRequest, IValidator<ProductPropertyCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest, IValidator<ProductPropertyUpdateRequest>? validator = null);
    bool Delete(uint productId, uint characteristicId);
    bool DeleteAllForCharacteristic(uint characteristicId);
    bool DeleteAllForProduct(uint productId);
}