using MOSTComputers.Models.Product.Models;
using static MOSTComputers.UI.Web.Blazor.Components.Pages.ProductEditor;

namespace MOSTComputers.UI.Web.Blazor.Services.ProductEditor.Contracts;
internal interface IProductEditorDataService
{
    Task<ProductEditorProductData> GetProductEditorProductDataAsync(Product product, bool fetchLegacyHtmlData = false);
    Task<List<ProductEditorProductData>> GetProductEditorProductDatasAsync(List<Product> products, bool fetchLegacyHtmlData = false);
}