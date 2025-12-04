using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductSerialNumber;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers.Contracts;
public interface IProductSerialNumberService
{
    Task<List<IGrouping<int, ProductSerialNumber>>> GetAllForProductsAsync(List<int> productIds);
    Task<List<ProductSerialNumber>> GetAllForProductAsync(int productId);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ProductSerialNumberCreateRequest createRequest);
    Task<OneOf<Success, NotFound>> DeleteAsync(int productId, string serialNumber);
}