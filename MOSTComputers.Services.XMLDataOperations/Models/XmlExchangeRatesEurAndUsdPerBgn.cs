using System.Xml.Serialization;

namespace MOSTComputers.Services.XMLDataOperations.Models;

public class XmlExchangeRatesEurAndUsdPerBgn
{
    [XmlElement(ElementName = "BGN-per-EUR")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public XmlExchangeRateEurAndUsdPerBgn ExchangeRateEUR { get; set; }

    [XmlElement(ElementName = "BGN-per-USD")]
    public XmlExchangeRateEurAndUsdPerBgn ExchangeRateUSD { get; set; }

    [XmlIgnore]
    public DateTime ValidTo { get; set; }

    [XmlElement(ElementName = "valid_to")]
    public string ValidToString
    {
        get => ValidTo.ToString("yyyy-MM-dd HH:mm:ss");
        set { ValidTo = DateTime.Parse(value); }
    }
}

[Serializable]
public class XmlExchangeRateEurAndUsdPerBgn
{
    [XmlIgnore]
    public string CurrencyCode { get; set; }

    [XmlText]
    public decimal ExchangeRatePerBGN { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.