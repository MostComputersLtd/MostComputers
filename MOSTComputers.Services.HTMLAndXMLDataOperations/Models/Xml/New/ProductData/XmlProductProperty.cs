using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
[Serializable]
public class XmlProductProperty : IXmlAsyncSerializable
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "s")]
    public string Order { get; set; }

    [XmlText]
    public string Value { get; set; }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (!string.IsNullOrEmpty(Name))
        {
            await writer.WriteAttributeStringAsync(null, "name", null, Name);
        }

        if (!string.IsNullOrEmpty(Order))
        {
            await writer.WriteAttributeStringAsync(null, "s", null, Order);
        }

        if (!string.IsNullOrEmpty(Value))
        {
            await writer.WriteStringAsync(Value);
        }

        await writer.WriteEndElementAsync();
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
