using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;

public interface IProductHtmlService
{
    string GetHtmlFromProduct(Product product);
    OneOf<string, InvalidXmlResult> TryGetHtmlFromProduct(Product product);
}