using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
public sealed class LegacyXmlFile
{
    [XmlElement(ElementName = "url")]
    public string? Url { get; set; }
}