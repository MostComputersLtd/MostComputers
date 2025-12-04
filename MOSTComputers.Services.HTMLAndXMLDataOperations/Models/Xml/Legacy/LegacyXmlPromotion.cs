using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
public sealed class LegacyXmlPromotion
{
    [XmlElement(ElementName = "type")]
    public string? Type { get; set; }

    [XmlIgnore]
    public DateTime? ValidFrom { get; set; }

    [XmlElement(ElementName = "valid_from")]
    public string? ValidFromString
    {
        get
        {
            return ValidFrom?.ToString("yyyy-MM-dd HH:mm:ss");
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

    [XmlElement(ElementName = "valid_to")]
    public string? ValidToString
    {
        get
        {
            return ValidTo?.ToString("yyyy-MM-dd HH:mm:ss");
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

    [XmlElement(ElementName = "PromotionUSD")]
    public decimal? PromotionUSD { get; set; }

    [XmlElement(ElementName = "PromotionEUR")]
    public decimal? PromotionEUR { get; set; }
}