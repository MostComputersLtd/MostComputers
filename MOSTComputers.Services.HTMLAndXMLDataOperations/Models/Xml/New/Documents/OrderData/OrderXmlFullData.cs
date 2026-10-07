using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.OrderData;
public sealed class OrderXmlFullData : IXmlAsyncSerializable
{
    public List<XmlOrder>? Orders { get; set; }

    public bool ShouldDisplayOrders()
    {
        return Orders?.Count > 0;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldDisplayOrders())
        {
            foreach (XmlOrder order in Orders!)
            {
                await order.WriteXmlAsync(writer, "order");
            }
        }

        await writer.WriteEndElementAsync();
    }
}
