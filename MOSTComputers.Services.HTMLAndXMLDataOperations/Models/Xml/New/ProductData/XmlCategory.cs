using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
[Serializable]
public class XmlCategory : IXmlAsyncSerializable
{
    [XmlAttribute("id")]
    public int Id { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [XmlText]
    public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [XmlIgnore]
    public int? ParentCategoryId { get; set; }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        await writer.WriteAttributeStringAsync(null, "id", null, Id.ToString(CultureInfo.InvariantCulture));

        if (!string.IsNullOrEmpty(Name))
        {
            await writer.WriteStringAsync(Name);
        }

        await writer.WriteEndElementAsync();
    }
}