using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.PromotionGroupData;

public sealed class XmlPromotionGroup : IXmlAsyncSerializable
{
    public string? Name { get; set; }
    public string? LogoImageUrl { get; set; }
    public List<XmlGroupPromotion>? Promotions { get; set; }

    public bool ShouldSerializeName()
    {
        return !string.IsNullOrEmpty(Name);
    }

    public bool ShouldSerializeLogoImageUrl()
    {
        return !string.IsNullOrEmpty(LogoImageUrl);
    }

    public bool ShouldSerializePromotions()
    {
        return Promotions?.Count > 0;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldSerializeName())
        {
            await writer.WriteAttributeStringAsync(null, "name", null, Name);
        }

        if (ShouldSerializeLogoImageUrl())
        {
            await writer.WriteElementStringAsync(null, "logo", null, LogoImageUrl!);
        }

        if (ShouldSerializePromotions())
        {
            foreach (XmlGroupPromotion promotion in Promotions!)
            {
                await promotion.WriteXmlAsync(writer, "promo");
            }
        }

        await writer.WriteEndElementAsync();
    }
}