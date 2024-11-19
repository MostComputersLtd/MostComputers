using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using OneOf;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductFullHtmlBasedDisplayPopupPartialModel
{
    public required IProductHtmlService ProductHtmlService { get; init; }
    public Product? Product { get; init; }
    public List<Tuple<string, List<SearchStringPartOriginData>?>>? ProductSearchStringPartsAndDataAboutTheirOrigin { get; init; }

    public OneOf<string?, InvalidXmlResult> GetProductHtml()
    {
        if (Product is null) return null;

        OneOf<string, InvalidXmlResult> getProductHtmlResult = ProductHtmlService.TryGetHtmlFromProduct(Product);

        return getProductHtmlResult!;
    }
}
