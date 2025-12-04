using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models.ProductEditor;

namespace MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;
public interface IProductEditorProductDataService
{
    Task<ProductEditorProductData> GetProductEditorProductDataAsync(Product product);
    Task<List<ProductEditorProductData>> GetProductEditorProductDatasAsync(List<Product> products);
}