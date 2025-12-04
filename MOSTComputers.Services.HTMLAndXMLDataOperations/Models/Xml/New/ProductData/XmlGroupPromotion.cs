using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

public sealed class XmlGroupPromotion : IXmlAsyncSerializable
{
    [XmlAttribute(AttributeName = "vendor")]
    public string? VendorName { get; set; }

    [XmlText]
    public string? GroupPromotionsUrl { get; set; }

    public bool ShouldSerializeVendorName()
    {
        return !string.IsNullOrEmpty(VendorName);
    }

    public bool ShouldSerializeGroupPromotionsUrl()
    {
        return !string.IsNullOrEmpty(GroupPromotionsUrl);
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldSerializeVendorName())
        {
            await writer.WriteAttributeStringAsync(null, "vendor", null, VendorName);
        }

        if (ShouldSerializeGroupPromotionsUrl())
        {
            await writer.WriteStringAsync(GroupPromotionsUrl);
        }

        await writer.WriteEndElementAsync();
    }
}