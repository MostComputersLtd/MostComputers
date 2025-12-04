using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;

[Serializable]
public class LegacyXmlExchangeRateEurAndUsdPerBgn
{
    [XmlText]
    public decimal ExchangeRatePerBGN { get; set; }
}