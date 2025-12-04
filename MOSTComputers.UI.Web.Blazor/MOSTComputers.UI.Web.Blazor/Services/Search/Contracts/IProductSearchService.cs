using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Blazor.Models.Search.Product;

namespace MOSTComputers.UI.Web.Blazor.Services.Search.Contracts;
public interface IProductSearchService
{
    Task<List<Product>> SearchProductsAsync(ProductSearchRequest productSearchRequest);
    Task<List<Product>> SearchProductsAsync(IEnumerable<Product> productsToSearchIn, ProductSearchRequest productSearchRequest);
}