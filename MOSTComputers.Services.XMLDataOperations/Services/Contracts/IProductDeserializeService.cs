using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.XMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.Services.XMLDataOperations.Services.Contracts;
public interface IProductDeserializeService
{
    XmlObjectData? DeserializeProductsXml(string xml);
    OneOf<XmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml);
    string SerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
    string SerializeProductsXml(Product product, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
    OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
    OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(Product product, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
}