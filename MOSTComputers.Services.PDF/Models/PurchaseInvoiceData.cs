namespace MOSTComputers.Services.PDF.Models;

public class PurchaseInvoiceData
{
    public required string ProductName { get; set; }
    public required int Quantity { get; set; }
    public required string UnitOfMeasurement { get; set; }
    public required string Currency { get; set; }
    public required double PricePerUnit { get; set; }
}