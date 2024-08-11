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
    IEnumerable<ProductProperty> GetAllInProduct(int productId);
    ProductProperty? GetByNameAndProductId(string name, int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(ProductPropertyByCharacteristicIdCreateRequest createRequest, IValidator<ProductPropertyByCharacteristicIdCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest, IValidator<ProductPropertyUpdateRequest>? validator = null);
    bool Delete(int productId, int characteristicId);
    bool DeleteAllForCharacteristic(int characteristicId);
    bool DeleteAllForProduct(int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(ProductPropertyByCharacteristicNameCreateRequest createRequest, IValidator<ProductPropertyByCharacteristicNameCreateRequest>? validator = null);
}