namespace MOSTComputers.Services.PDF.Models.WarrantyCards;
public sealed class WarrantyCardWithPricesData
{
    public string? ExportUser { get; init; }
    public int OrderId { get; init; }
    public int? CustomerBID { get; init; }
    public string? CustomerName { get; init; }
    public DateTime? WarrantyCardDate { get; init; }
    public int? WarrantyCardTermInMonths { get; init; }

    public List<WarrantyCardItemWithPricesData>? WarrantyCardItems { get; init; }
}