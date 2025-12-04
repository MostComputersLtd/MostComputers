using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
[Serializable]
public class XmlManufacturer : IXmlAsyncSerializable
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [XmlAttribute(AttributeName = "id")]
    public string IdAsString { get; set; }

    public int? Id
    {
        get
        {
            if (string.IsNullOrEmpty(IdAsString)) return null;

            bool parseSuccess = int.TryParse(IdAsString, out int output);

            return parseSuccess ? output : null;
        }
    }

    [XmlText]
    public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (!string.IsNullOrEmpty(IdAsString))
        {
            await writer.WriteAttributeStringAsync(null, "id", null, IdAsString);
        }

        if (!string.IsNullOrEmpty(Name))
        {
            await writer.WriteStringAsync(Name);
        }

        await writer.WriteEndElementAsync();
    }
}