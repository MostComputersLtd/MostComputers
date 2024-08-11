using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
public interface IProductDeserializeService
{
    XmlObjectData? DeserializeProductsXml(string xml);
    OneOf<XmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml);
    string SerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
    string SerializeProductXml(Product product, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
    OneOf<string, InvalidXmlResult> TrySerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
    OneOf<string, InvalidXmlResult> TrySerializeProductXml(Product product, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
}