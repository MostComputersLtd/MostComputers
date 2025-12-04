using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
public interface ILegacyProductXmlService
{
    LegacyXmlObjectData? DeserializeProductsXml(string xml);
    OneOf<LegacyXmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml);
    string SerializeProductsXml(LegacyXmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
    OneOf<string, InvalidXmlResult> TrySerializeProductsXml(LegacyXmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false);
}