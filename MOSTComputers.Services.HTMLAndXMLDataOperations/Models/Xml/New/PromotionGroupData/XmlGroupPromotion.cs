using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.PromotionGroupData;

public sealed class XmlGroupPromotion : IXmlAsyncSerializable
{
    [XmlText]
    public string? PromotionPictureUrl { get; set; }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        await writer.WriteStringAsync(PromotionPictureUrl);

        await writer.WriteEndElementAsync();
    }
}