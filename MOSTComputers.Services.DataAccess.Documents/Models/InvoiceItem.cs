namespace MOSTComputers.Services.DataAccess.Documents.Models;
public sealed class InvoiceItem
{
    public int ExportedItemId { get; init; }
    public int? ExportId { get; init; }
    public int IEID { get; init; }
    public int? InvoiceId { get; init; }
    public string? Name { get; init; }
    public decimal? PriceInLeva { get; init; }
    public int? Quantity { get; init; }
    public int? DisplayOrder { get; init; }
}