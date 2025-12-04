using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
public sealed class XmlFile : IXmlAsyncSerializable
{
    [XmlElement(ElementName = "url")]
    public string? Url { get; set; }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (!string.IsNullOrEmpty(Url))
        {
            await writer.WriteElementStringAsync(null, "url", null, Url);
        }

        await writer.WriteEndElementAsync();
    }
}