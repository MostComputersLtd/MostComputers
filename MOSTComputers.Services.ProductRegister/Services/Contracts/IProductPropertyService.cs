using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductPropertyService
{
    IEnumerable<ProductProperty> GetAllInProduct(uint productId);
    ProductProperty? GetByNameAndProductId(string name, uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(ProductPropertyByCharacteristicIdCreateRequest createRequest, IValidator<ProductPropertyByCharacteristicIdCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest, IValidator<ProductPropertyUpdateRequest>? validator = null);
    bool Delete(uint productId, uint characteristicId);
    bool DeleteAllForCharacteristic(uint characteristicId);
    bool DeleteAllForProduct(uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(ProductPropertyByCharacteristicNameCreateRequest createRequest, IValidator<ProductPropertyByCharacteristicNameCreateRequest>? validator = null);
}