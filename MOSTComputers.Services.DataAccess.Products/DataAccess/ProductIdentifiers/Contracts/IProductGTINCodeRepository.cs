using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductGTINCode;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers.Contracts;
public interface IProductGTINCodeRepository
{
    Task<List<IGrouping<int, ProductGTINCode>>> GetAllForProductsAsync(List<int> productIds);
    Task<List<ProductGTINCode>> GetAllForProductAsync(int productId);
    Task<ProductGTINCode?> GetByProductIdAndTypeAsync(int productId, ProductGTINCodeType productGTINCodeType);
    Task<OneOf<Success, UnexpectedFailureResult>> InsertAsync(ProductGTINCodeCreateRequest createRequest);
    Task<OneOf<Success, NotFound>> UpdateAsync(ProductGTINCodeUpdateRequest updateRequest);
    Task<OneOf<Success, UnexpectedFailureResult>> UpsertAsync(ProductGTINCodeUpsertRequest upsertRequest);
    Task<OneOf<Success, NotFound>> DeleteAsync(int productId, ProductGTINCodeType productGTINCodeType);
}