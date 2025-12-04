using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductSerialNumber;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers.Contracts;
public interface IProductSerialNumberRepository
{
    Task<List<ProductSerialNumber>> GetAllForProductAsync(int productId);
    Task<List<IGrouping<int, ProductSerialNumber>>> GetAllForProductsAsync(List<int> productIds);
    Task<OneOf<Success, UnexpectedFailureResult>> InsertAsync(ProductSerialNumberCreateRequest createRequest);
    Task<OneOf<Success, NotFound>> DeleteAsync(int productId, string serialNumber);
}