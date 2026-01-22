using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;
public sealed class XmlInvoiceItem : IXmlAsyncSerializable
{
    public int ExportedItemId { get; init; }
    public int? ExportId { get; init; }
    public int IEID { get; init; }
    public string? Name { get; init; }
    public decimal? Price { get; init; }
    public int? Quantity { get; init; }
    public int? DisplayOrder { get; init; }

    public bool ShouldDisplayName()
    {
        return !string.IsNullOrEmpty(Name);
    }

    public bool ShouldDisplayPriceInLeva()
    {
        return Price is not null;
    }

    public bool ShouldDisplayQuantity()
    {
        return Quantity is not null;
    }

    public bool ShouldDisplayDisplayOrder()
    {
        return DisplayOrder is not null;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldDisplayDisplayOrder())
        {
            await writer.WriteAttributeStringAsync(null, "order", null, DisplayOrder!.Value.ToString());
        }

        if (ShouldDisplayName())
        {
            await writer.WriteElementStringAsync(null, "name", null, Name!.ToString());
        }

        if (ShouldDisplayPriceInLeva())
        {
            await writer.WriteElementStringAsync(null, "price", null, Price!.Value.ToString());
        }

        if (ShouldDisplayQuantity())
        {
            await writer.WriteElementStringAsync(null, "quantity", null, Quantity!.Value.ToString());
        }

        await writer.WriteEndElementAsync();
    }
}