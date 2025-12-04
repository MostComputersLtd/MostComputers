using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
public sealed class XmlSearchStringPartInfo : IXmlAsyncSerializable
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlText]
    public string? Description { get; set; }
    public bool ShouldSerializeDescription()
    {
        return Description is not null;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (Name is not null)
        {
            await writer.WriteAttributeStringAsync(null, "name", null, Name);
        }

        if (ShouldSerializeDescription())
        {
            await writer.WriteStringAsync(Description);
        }

        await writer.WriteEndElementAsync();
    }
}