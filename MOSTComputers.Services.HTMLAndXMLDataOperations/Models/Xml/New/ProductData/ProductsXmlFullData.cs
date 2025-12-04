using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[Serializable]
[XmlRoot(ElementName = "data")]
public class ProductsXmlFullData : IXmlAsyncSerializable
{
    [XmlArray(ElementName = "productlist")]
    [XmlArrayItem(ElementName = "product")]
    public List<XmlProduct> Products { get; set; }

    [XmlElement("rates")]
    public XmlExchangeRates ExchangeRates { get; set; }

    [XmlElement("rowstotal")]
    public int TotalNumberOfItems => Products.Count;

    [XmlElement("date_export", DataType = "date")]
    public DateTime DateOfExport { get; set; } = DateTime.Now;

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        await writer.WriteStartElementAsync(null, "productList", null);

        foreach (XmlProduct product in Products)
        {
            await product.WriteXmlAsync(writer, "product");
        }

        await writer.WriteFullEndElementAsync();

        await ExchangeRates.WriteXmlAsync(writer, "rates");

        await writer.WriteElementStringAsync(null, "rowstotal", null, TotalNumberOfItems.ToString());

        await writer.WriteElementStringAsync(null, "date_export", null, DateOfExport.ToString("yyyy-MM-dd HH:mm:ss"));

        await writer.WriteFullEndElementAsync();
    }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.