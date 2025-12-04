using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
public interface IProductXmlService
{
    ProductsXmlFullData? DeserializeProductsXml(string xml);
    OneOf<ProductsXmlFullData?, InvalidXmlResult> TryDeserializeProductsXml(string xml);
    Task TrySerializeProductsXmlAsync(Stream outputStream, ProductsXmlFullData xmlObjectData);
    Task<OneOf<string, InvalidXmlResult>> TrySerializeProductsXmlAsync(ProductsXmlFullData xmlObjectData);
}