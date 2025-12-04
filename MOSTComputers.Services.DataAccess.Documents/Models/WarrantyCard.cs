namespace MOSTComputers.Services.DataAccess.Documents.Models;
public sealed class WarrantyCard
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

    public List<WarrantyCardItem>? WarrantyCardItems { get; init; }
}