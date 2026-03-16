using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;

namespace MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
public interface IProductSearchService
{
    IOrderedEnumerable<Product> OrderProductsByDisplayOrderThenById(List<Product> searchedProducts);
    Task<List<Product>> OrderProductsByLastAddedDateAsync(List<Product> filteredProducts);
    Task<List<Product>> SearchProductsAsync(ProductSearchRequest productSearchRequest);
    Task<List<Product>> SearchProductsAsync(IEnumerable<Product> productsToSearchIn, ProductSearchRequest productSearchRequest);
}