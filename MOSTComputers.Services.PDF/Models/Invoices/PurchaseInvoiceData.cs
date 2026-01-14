using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.PDF.Models.Invoices;

public class PurchaseInvoiceData
{
    public string? ProductName { get; set; }
    public required int? Quantity { get; set; }
    public required string UnitOfMeasurement { get; set; }
    public required Currency Currency { get; set; }
    public required decimal? PricePerUnit { get; set; }
    public required decimal? LineNetPrice { get; set; }
    public required decimal? LineVatPrice { get; set; }
    public required decimal? LineTotalPrice { get; set; }
}