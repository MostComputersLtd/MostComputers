using MOSTComputers.Services.XMLDataOperations.Models;
using OneOf;

namespace MOSTComputers.Services.XMLDataOperations.Services.Contracts;
public interface IProductDeserializeService
{
    XmlObjectData? DeserializeProductsXml(string xml);
    string SerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes);
    OneOf<XmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml);
    OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes);
}