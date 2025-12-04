namespace MOSTComputers.UI.Web.Models.Documents.WarrantyCards;

public sealed class WarrantyCardDisplayData
{
    public int ExportId { get; init; }
    public DateTime? ExportDate { get; init; }
    public int? ExportUserId { get; init; }
    public string? ExportUser { get; init; }
    public int OrderId { get; init; }
    public int? CustomerBID { get; init; }
    public string? CustomerName { get; init; }
    public DateTime? WarrantyCardDate { get; init; }
    public int? WarrantyCardTerm { get; init; }

    public List<WarrantyCardItemDisplayData>? WarrantyCardItems { get; init; }
}
