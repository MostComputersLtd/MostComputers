using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
public interface IProductHtmlService
{
    OneOf<string, InvalidXmlResult> TryGetHtmlFromProducts(HtmlProductsData htmlProductsData);
}