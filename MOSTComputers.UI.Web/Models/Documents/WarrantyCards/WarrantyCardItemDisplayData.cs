namespace MOSTComputers.UI.Web.Models.Documents.WarrantyCards;

public sealed class WarrantyCardItemDisplayData
{
    public int ExportedItemId { get; init; }
    public int? ExportId { get; init; }
    public int OrderId { get; init; }
    public int? ProductId { get; init; }
    public string? ProductName { get; init; }
    public decimal? PriceInLeva { get; init; }
    public int? Quantity { get; init; }
    public string? SerialNumber { get; init; }
    public int? WarrantyCardItemTermInMonths { get; init; }
    public int? DisplayOrder { get; init; }
}
