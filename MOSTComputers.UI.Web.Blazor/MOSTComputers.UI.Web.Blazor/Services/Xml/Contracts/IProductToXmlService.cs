using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using OneOf;
using static MOSTComputers.UI.Web.Blazor.Services.Xml.ProductToXmlService;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
public interface IProductToXmlService
{
    Task<ProductsXmlFullData> GetXmlObjectDataForProductsAsync(List<XmlProduct> xmlProducts);
    Task<OneOf<string, InvalidXmlResult>> TryGetXmlForAllProductsAsync(ProductXmlOptions? productXmlOptions = null);
    Task TryGetXmlForAllProductsAsync(Stream outputStream, ProductXmlOptions? productXmlOptions = null);
    Task<OneOf<string, InvalidXmlResult>> TryGetXmlForProductsAsync(List<int> productIds, ProductXmlOptions? productXmlOptions = null);
    Task TryGetXmlForProductsAsync(Stream outputStream, List<int> productIds, ProductXmlOptions? productXmlOptions = null);
}