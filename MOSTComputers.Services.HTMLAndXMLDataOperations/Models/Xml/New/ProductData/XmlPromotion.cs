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

    [XmlElement(ElementName = "name")]
    public string? Name { get; set; }

    [XmlElement(ElementName = "pictureUrl")]
    public string? PictureUrl { get; set; }

    [XmlElement(ElementName = "Discount_BGN")]
    public decimal? PromotionBGN { get; set; }

    [XmlElement(ElementName = "Discount_EUR")]
    public decimal? PromotionEUR { get; set; }

    [XmlElement(ElementName = "Discount_USD")]
    public decimal? PromotionUSD { get; set; }

    [XmlElement(ElementName = "minQuantity")]
    public int? MinimumQuantity { get; set; }

    [XmlElement(ElementName = "maxQuantity")]
    public int? MaximumQuantity { get; set; }

    [XmlElement(ElementName = "quantityIncrement")]
    public int? QuantityIncrement { get; set; }

    [XmlElement(ElementName = "expireQuantity")]
    public int? ExpireQuantity { get; set; }

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

    public bool ShouldSerializeName()
    {
        return !string.IsNullOrWhiteSpace(Name);
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
        return ValidFromString != null;
    }

    public bool ShouldSerializeValidToString()
    {
        return ValidToString != null;
    }

    public bool ShouldSerializePromotionBGN()
    {
        return PromotionBGN != null && PromotionBGN > 0M;
    }

    public bool ShouldSerializePromotionEUR()
    {
        return PromotionEUR != null && PromotionEUR > 0M;
    }

    public bool ShouldSerializePromotionUSD()
    {
        return PromotionUSD != null && PromotionUSD > 0M;
    }

    public bool ShouldSerializeMinimumQuantity()
    {
        return MinimumQuantity != null && MinimumQuantity > 0;
    }

    public bool ShouldSerializeMaximumQuantity()
    {
        return MaximumQuantity != null && MaximumQuantity > 0;
    }

    public bool ShouldSerializeQuantityIncrement()
    {
        return QuantityIncrement != null && QuantityIncrement > 0;
    }

    public bool ShouldSerializeExpireQuantity()
    {
        return ExpireQuantity != null && ExpireQuantity > 0;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldSerializeType())
        {
            await writer.WriteElementStringAsync(null, "type", null, Type!);
        }

        if (ShouldSerializeName())
        {
            await writer.WriteElementStringAsync(null, "name", null, Name!);
        }

        if (ShouldSerializeInfo())
        {
            await writer.WriteElementStringAsync(null, "info", null, Info!);
        }

        if (ShouldSerializePictureUrl())
        {
            await writer.WriteElementStringAsync(null, "pictureUrl", null, PictureUrl!);
        }

        if (ShouldSerializeMinimumQuantity())
        {
            await writer.WriteElementStringAsync(null, "minQuantity", null, MinimumQuantity!.Value.ToString());
        }

        if (ShouldSerializeMaximumQuantity())
        {
            await writer.WriteElementStringAsync(null, "maxQuantity", null, MaximumQuantity!.Value.ToString());
        }

        if (ShouldSerializeQuantityIncrement())
        {
            await writer.WriteElementStringAsync(null, "quantityIncrement", null, QuantityIncrement!.Value.ToString());
        }

        if (ShouldSerializeExpireQuantity())
        {
            await writer.WriteElementStringAsync(null, "expireQuantity", null, ExpireQuantity!.Value.ToString());
        } 

        if (ShouldSerializeValidFromString())
        {
            await writer.WriteElementStringAsync(null, "validFrom", null, ValidFromString!);
        }

        if (ShouldSerializeValidToString())
        {
            await writer.WriteElementStringAsync(null, "validTo", null, ValidToString!);
        } 

        if (ShouldSerializePromotionBGN())
        {
            await writer.WriteElementStringAsync(null, "Discount_BGN", null, PromotionBGN!.Value.ToString("F2"));
        }

        if (ShouldSerializePromotionEUR())
        {
            await writer.WriteElementStringAsync(null, "Discount_EUR", null, PromotionEUR!.Value.ToString("F2"));
        }

        if (ShouldSerializePromotionUSD())
        {
            await writer.WriteElementStringAsync(null, "Discount_USD", null, PromotionUSD!.Value.ToString("F2"));
        }

        await writer.WriteEndElementAsync();
    }
}
