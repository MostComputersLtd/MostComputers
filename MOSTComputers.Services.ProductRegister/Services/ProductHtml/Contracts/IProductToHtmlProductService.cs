using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;

namespace MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
internal interface IProductToHtmlProductService
{
    Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(IEnumerable<Product> products);
    Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(List<int> productIds);
    Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(params Product[] products);
}