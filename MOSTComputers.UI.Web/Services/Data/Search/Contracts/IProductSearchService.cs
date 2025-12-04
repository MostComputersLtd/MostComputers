using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models.ProductSearch;

namespace MOSTComputers.UI.Web.Services.Data.Search.Contracts;
public interface IProductSearchService
{
    Task<List<Product>> SearchProductsAsync(ProductSearchRequest productSearchRequest);
    Task<List<Product>> SearchProductsAsync(IEnumerable<Product> productsToSearchIn, ProductSearchRequest productSearchRequest);
}