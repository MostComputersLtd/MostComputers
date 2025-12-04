using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.PromotionGroupData;

public sealed class GroupPromotionsXmlFullData : IXmlAsyncSerializable
{
    public List<XmlPromotionGroup>? PromotionGroups { get; set; }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (PromotionGroups?.Count > 0)
        {
            foreach (XmlPromotionGroup promotionGroup in PromotionGroups)
            {
                await promotionGroup.WriteXmlAsync(writer, "promoGroup");
            }
        }

        await writer.WriteEndElementAsync();
    }
}
