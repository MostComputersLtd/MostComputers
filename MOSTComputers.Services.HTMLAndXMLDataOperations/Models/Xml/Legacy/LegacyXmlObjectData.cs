using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[Serializable]
[XmlRoot(ElementName = "data")]
public class LegacyXmlObjectData
{
    [XmlArray(ElementName = "productlist")]
    [XmlArrayItem(ElementName = "product")]
    public List<LegacyXmlProduct> Products { get; set; }

    [XmlElement("rates")]
    public LegacyXmlExchangeRatesEurAndUsdPerBgn ExchangeRates { get; set; }

    [XmlElement("rowstotal")]
    public int TotalNumberOfItems => Products.Count;

    [XmlElement("date_export", DataType = "date")]
    public DateTime DateOfExport { get; set; } = DateTime.Today;
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.