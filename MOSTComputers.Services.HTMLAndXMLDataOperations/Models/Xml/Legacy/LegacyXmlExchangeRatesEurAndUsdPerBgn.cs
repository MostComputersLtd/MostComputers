using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
public class LegacyXmlExchangeRatesEurAndUsdPerBgn
{
    [XmlElement(ElementName = "BGN-per-EUR")]
    public LegacyXmlExchangeRateEurAndUsdPerBgn? ExchangeRateEUR { get; set; }

    public bool ShouldSerializeExchangeRateEUR()
    {
        return ExchangeRateEUR is not null;
    }

    [XmlElement(ElementName = "BGN-per-USD")]
    public LegacyXmlExchangeRateEurAndUsdPerBgn? ExchangeRateUSD { get; set; }

    public bool ShouldSerializeExchangeRateUSD()
    {
        return ExchangeRateUSD is not null;
    }

    [XmlIgnore]
    public DateTime ValidTo { get; set; }

    [XmlElement(ElementName = "valid_to")]
    public string ValidToString
    {
        get => ValidTo.ToString("yyyy-MM-dd HH:mm:ss");
        set { ValidTo = DateTime.Parse(value); }
    }
}