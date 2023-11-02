using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MOSTComputers.Services.XMLDataOperations.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[Serializable]
[XmlRoot(ElementName = "data")]
public class XmlObjectData
{
    [XmlArray(ElementName = "productlist")]
    [XmlArrayItem(ElementName = "product")]
    public List<XmlProduct> Products { get; set; }

    [XmlElement("rates")]
    public XmlExchangeRatesEurAndUsdPerBgn ExchangeRates { get; set; }

    [XmlElement("rowstotal")]
    public int TotalNumberOfItems { get => Products.Count; set { } }

    [XmlElement("date_export", DataType = "date")]
    public DateTime DateOfExport { get; set; } = DateTime.Today;
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.