using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
public sealed class LegacySearchStringPartInfo
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlText]
    public string? Description { get; set; }
    public bool ShouldSerializeDescription()
    {
        return Description is not null;
    }
}