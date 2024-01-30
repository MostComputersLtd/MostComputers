using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.XMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.Services.XMLDataOperations.Services.Contracts;
public interface IProductDeserializeService
{
    XmlObjectData? DeserializeProductsXml(string xml);
    OneOf<XmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml);
    string SerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes);
    string SerializeProductsXml(Product product, bool shouldRemoveXmlSerializeEscapes);
    OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes);
    OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(Product product, bool shouldRemoveXmlSerializeEscapes);
}