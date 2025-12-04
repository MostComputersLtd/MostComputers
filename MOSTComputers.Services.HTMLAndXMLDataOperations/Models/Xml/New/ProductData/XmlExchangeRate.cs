using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

[Serializable]
public class XmlExchangeRate
{
    [XmlText]
    public decimal ExchangeRatePerEUR { get; set; }
}