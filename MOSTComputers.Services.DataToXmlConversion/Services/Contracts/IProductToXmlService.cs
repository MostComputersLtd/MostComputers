using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataToXmlConversion.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using OneOf;

namespace MOSTComputers.Services.DataToXmlConversion.Services.Contracts;
public interface IProductToXmlService
{
    Task<ProductsXmlFullData> GetXmlObjectDataForProductsAsync(List<XmlProduct> xmlProducts);
    Task TryGetXmlForAllPublicProductsAsync(Stream outputStream, ProductXmlOptions? productXmlOptions = null);
    Task TryGetXmlForProductsAsync(Stream outputStream, List<Product> products, ProductXmlOptions? productXmlOptions = null);
    Task<OneOf<string, InvalidXmlResult>> TryGetXmlForProductsAsync(List<Product> products, ProductXmlOptions? productXmlOptions = null);
}