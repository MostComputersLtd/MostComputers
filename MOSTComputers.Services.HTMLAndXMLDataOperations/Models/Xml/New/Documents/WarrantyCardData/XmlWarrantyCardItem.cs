using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.WarrantyCardData;
public sealed class XmlWarrantyCardItem : IXmlAsyncSerializable
{
    public int ExportedItemId { get; init; }
    public int? ExportId { get; init; }
    public int OrderId { get; init; }
    public int? ProductId { get; init; }
    public string? ProductName { get; init; }
    public int? Quantity { get; init; }
    public string? SerialNumber { get; init; }
    public int? WarrantyCardItemTermInMonths { get; init; }
    public int? DisplayOrder { get; init; }

    public bool ShouldDisplayExportId()
    {
        return ExportId.HasValue;
    }

    public bool ShouldDisplayProductId()
    {
        return ProductId.HasValue;
    }

    public bool ShouldDisplayProductName()
    {
        return !string.IsNullOrEmpty(ProductName);
    }

    public bool ShouldDisplayQuantity()
    {
        return Quantity.HasValue;
    }

    public bool ShouldDisplaySerialNumber()
    {
        return !string.IsNullOrEmpty(SerialNumber);
    }

    public bool ShouldDisplayWarrantyCardItemTermInMonths()
    {
        return WarrantyCardItemTermInMonths.HasValue;
    }

    public bool ShouldDisplayDisplayOrder()
    {
        return DisplayOrder.HasValue;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        //if (ShouldDisplayDisplayOrder())
        //{
        //    await writer.WriteAttributeStringAsync(null, "order", null, DisplayOrder!.Value.ToString());
        //}

        await writer.WriteStartElementAsync(null, "product", null);

        if (ShouldDisplayProductId())
        {
            await writer.WriteAttributeStringAsync(null, "id", null, ProductId!.Value.ToString());
        }

        if (ShouldDisplayProductName())
        {
            await writer.WriteStringAsync(ProductName!);
        }

        await writer.WriteEndElementAsync();

        if (ShouldDisplayQuantity())
        {
            await writer.WriteElementStringAsync(null, "quantity", null, Quantity!.Value.ToString());
        }

        if (ShouldDisplaySerialNumber())
        {
            await writer.WriteElementStringAsync(null, "serialNumber", null, SerialNumber!);
        }

        if (ShouldDisplayWarrantyCardItemTermInMonths())
        {
            await writer.WriteElementStringAsync(null, "warrantyTermInMonths", null, WarrantyCardItemTermInMonths!.Value.ToString());
        }

        await writer.WriteEndElementAsync();
    }
}