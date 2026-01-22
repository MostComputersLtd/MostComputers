using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.WarrantyCardData;
public sealed class XmlWarrantyCard : IXmlAsyncSerializable
{
    private const string _dateFormat = "MMM dd yyyy T:HH:mm:ss";

    public int ExportId { get; init; }
    public DateTime? ExportDate { get; init; }
    public int? ExportUserId { get; init; }
    public string? ExportUser { get; init; }
    public int OrderId { get; init; }
    public int? CustomerBID { get; init; }
    public string? CustomerName { get; init; }
    public DateTime? WarrantyCardDate { get; init; }
    public int? WarrantyCardTerm { get; init; }

    public List<XmlWarrantyCardItem>? WarrantyCardItems { get; init; }

    public bool ShouldDisplayExportDate()
    {
        return ExportDate.HasValue;
    }

    public bool ShouldDisplayExportUserId()
    {
        return ExportUserId.HasValue;
    }

    public bool ShouldDisplayExportUser()
    {
        return !string.IsNullOrEmpty(ExportUser);
    }

    public bool ShouldDisplayCustomerBID()
    {
        return CustomerBID.HasValue;
    }

    public bool ShouldDisplayCustomerName()
    {
        return !string.IsNullOrEmpty(CustomerName);
    }

    public bool ShouldDisplayWarrantyCardDate()
    {
        return WarrantyCardDate.HasValue;
    }

    public bool ShouldDisplayWarrantyCardTerm()
    {
        return WarrantyCardTerm.HasValue;
    }

    public bool ShouldDisplayWarrantyCardItems()
    {
        return WarrantyCardItems?.Count > 0;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        await writer.WriteAttributeStringAsync(null, "orderId", null, OrderId.ToString());

        if (ShouldDisplayCustomerBID())
        {
            await writer.WriteElementStringAsync(null, "customerBID", null, CustomerBID!.Value.ToString());
        }

        if (ShouldDisplayCustomerName())
        {
            await writer.WriteElementStringAsync(null, "customerName", null, CustomerName!.ToString());
        }

        if (ShouldDisplayWarrantyCardDate())
        {
            await writer.WriteElementStringAsync(null, "date", null, WarrantyCardDate!.Value.ToString(_dateFormat));
        }

        if (ShouldDisplayWarrantyCardTerm())
        {
            await writer.WriteElementStringAsync(null, "term", null, WarrantyCardTerm!.Value.ToString());
        }

        if (ShouldDisplayWarrantyCardItems())
        {
            await writer.WriteStartElementAsync(null, "items", null);

            foreach (XmlWarrantyCardItem item in WarrantyCardItems!)
            {
                await item.WriteXmlAsync(writer, "item");
            }

            await writer.WriteEndElementAsync();
        }

        await writer.WriteEndElementAsync();
    }
}