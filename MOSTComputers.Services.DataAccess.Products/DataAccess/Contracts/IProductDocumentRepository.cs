using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductDocument;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IProductDocumentRepository
{
    Task<List<ProductDocument>> GetAllForProductAsync(int productId);
    Task<ProductDocument?> GetByIdAsync(int id);
    Task<OneOf<ProductDocument, UnexpectedFailureResult>> InsertAsync(ProductDocumentCreateRequest createRequest);
}