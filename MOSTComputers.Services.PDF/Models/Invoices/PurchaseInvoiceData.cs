namespace MOSTComputers.Services.PDF.Models.Invoices;

public class PurchaseInvoiceData
{
    public string? ProductName { get; set; }
    public required int? Quantity { get; set; }
    public required string UnitOfMeasurement { get; set; }
    public required string Currency { get; set; }
    public required decimal? PricePerUnit { get; set; }
}