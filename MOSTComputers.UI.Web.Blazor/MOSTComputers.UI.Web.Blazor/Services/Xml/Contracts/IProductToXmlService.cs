using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using OneOf;
using static MOSTComputers.UI.Web.Blazor.Services.Xml.ProductToXmlService;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
public interface IProductToXmlService
{
    Task<ProductsXmlFullData> GetXmlObjectDataForProductsAsync(List<XmlProduct> xmlProducts);
    Task TryGetXmlForAllPublicProductsAsync(Stream outputStream, ProductXmlOptions? productXmlOptions = null);
    Task TryGetXmlForProductsAsync(Stream outputStream, List<Product> products, ProductXmlOptions? productXmlOptions = null);
    Task<OneOf<string, InvalidXmlResult>> TryGetXmlForProductsAsync(List<Product> products, ProductXmlOptions? productXmlOptions = null);
}