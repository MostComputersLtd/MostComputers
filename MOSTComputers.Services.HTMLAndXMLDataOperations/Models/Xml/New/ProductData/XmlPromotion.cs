using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
public sealed class XmlPromotion : IXmlAsyncSerializable
{
    [XmlElement(ElementName = "type")]
    public string? Type { get; set; }

    [XmlElement(ElementName = "info")]
    public string? Info { get; set; }

    [XmlElement(ElementName = "pictureUrl")]
    public string? PictureUrl { get; set; }

    [XmlElement(ElementName = "Discount_BGN")]
    public decimal? PromotionBGN { get; set; }

    [XmlElement(ElementName = "Discount_EUR")]
    public decimal? PromotionEUR { get; set; }

    [XmlElement(ElementName = "Discount_USD")]
    public decimal? PromotionUSD { get; set; }

    [XmlIgnore]
    public DateTime? ValidFrom { get; set; }

    [XmlElement(ElementName = "validFrom")]
    public string? ValidFromString
    {
        get
        {
            if (ValidFrom is null) return null;

            if (ValidFrom.Value.TimeOfDay == TimeSpan.Zero)
            {
                return ValidFrom.Value.ToString("yyyy-MM-dd");
            }

            return ValidFrom.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }
        set
        {
            if (value == null)
            {
                ValidFrom = null;

                return;
            }

            bool isDateTime = DateTime.TryParse(value, out DateTime validFrom);

            ValidFrom = isDateTime ? validFrom : null;
        }
    }

    [XmlIgnore]
    public DateTime? ValidTo { get; set; }

    [XmlElement(ElementName = "validTo")]
    public string? ValidToString
    {
        get
        {
            if (ValidTo is null) return null;

            if (ValidTo.Value.TimeOfDay == TimeSpan.Zero)
            {
                return ValidTo.Value.ToString("yyyy-MM-dd");
            }

            return ValidTo.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }
        set
        {
            if (value == null)
            {
                ValidTo = null;

                return;
            }

            bool isDateTime = DateTime.TryParse(value, out DateTime validTo);

            ValidTo = isDateTime ? validTo : null;
        }
    }

    public bool ShouldSerializeType()
    {
        return !string.IsNullOrEmpty(Type);
    }

    public bool ShouldSerializeInfo()
    {
        return !string.IsNullOrEmpty(Info);
    }

    public bool ShouldSerializePictureUrl()
    {
        return !string.IsNullOrEmpty(PictureUrl);
    }

    public bool ShouldSerializeValidFromString()
    {
        return ValidFromString is not null;
    }

    public bool ShouldSerializeValidToString()
    {
        return ValidToString is not null;
    }

    public bool ShouldSerializePromotionBGN()
    {
        return PromotionBGN is not null && PromotionBGN > 0M;
    }

    public bool ShouldSerializePromotionEUR()
    {
        return PromotionEUR is not null && PromotionEUR > 0M;
    }

    public bool ShouldSerializePromotionUSD()
    {
        return PromotionUSD is not null && PromotionUSD > 0M;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldSerializeType())
        {
            await writer.WriteStartElementAsync(null, "type", null);
            await writer.WriteStringAsync(Type);
            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializeInfo())
        {
            await writer.WriteStartElementAsync(null, "info", null);
            await writer.WriteStringAsync(Info);
            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializePictureUrl())
        {
            await writer.WriteStartElementAsync(null, "pictureUrl", null);
            await writer.WriteStringAsync(PictureUrl);
            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializePromotionBGN())
        {
            await writer.WriteStartElementAsync(null, "Discount_BGN", null);
            await writer.WriteStringAsync(PromotionBGN!.ToString());
            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializePromotionEUR())
        {
            await writer.WriteStartElementAsync(null, "Discount_EUR", null);
            await writer.WriteStringAsync(PromotionEUR!.ToString());
            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializePromotionUSD())
        {
            await writer.WriteStartElementAsync(null, "Discount_USD", null);
            await writer.WriteStringAsync(PromotionUSD!.ToString());
            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializeValidFromString())
        {
            await writer.WriteStartElementAsync(null, "validFrom", null);
            await writer.WriteStringAsync(ValidFromString);
            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializeValidToString())
        {
            await writer.WriteStartElementAsync(null, "validTo", null);
            await writer.WriteStringAsync(ValidToString);
            await writer.WriteEndElementAsync();
        }

        await writer.WriteEndElementAsync();
    }
}