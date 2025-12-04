using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
public class XmlExchangeRates : IXmlAsyncSerializable
{
    [XmlElement(ElementName = "EUR-per-USD")]
    public XmlExchangeRate? ExchangeRateUSD { get; set; }

    public bool ShouldSerializeExchangeRateUSD()
    {
        return ExchangeRateUSD is not null;
    }

    [XmlIgnore]
    public DateTime ValidTo { get; set; }

    [XmlElement(ElementName = "valid-to")]
    public string ValidToString
    {
        get => ValidTo.ToString("yyyy-MM-dd HH:mm:ss");
        set { ValidTo = DateTime.Parse(value); }
    }

    [XmlElement(ElementName = "valid-to-info")]
    public string ValidToInfo { get; set; } = "Цените зависят от курсовете и са валидни само до часа на валидност";

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldSerializeExchangeRateUSD())
        {
            await writer.WriteStartElementAsync(null, "EUR-per-USD", null);
            await writer.WriteStringAsync(ExchangeRateUSD!.ExchangeRatePerEUR.ToString());
            await writer.WriteEndElementAsync();
        }

        await writer.WriteStartElementAsync(null, "valid-to", null);
        await writer.WriteStringAsync(ValidToString);
        await writer.WriteEndElementAsync();

        await writer.WriteElementStringAsync(null, "valid-to-info", null, ValidToInfo);
        
        await writer.WriteEndElementAsync();
    }
}