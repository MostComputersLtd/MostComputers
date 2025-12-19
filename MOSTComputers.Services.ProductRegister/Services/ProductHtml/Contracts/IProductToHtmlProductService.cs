using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductHtml;

namespace MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
internal interface IProductToHtmlProductService
{
    HtmlProductsData GetHtmlProductDataFromProducts(List<GetHtmlDataForProductRequest> requests);
    Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(IEnumerable<Product> products);
    Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(List<int> productIds);
    Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(params Product[] products);
}