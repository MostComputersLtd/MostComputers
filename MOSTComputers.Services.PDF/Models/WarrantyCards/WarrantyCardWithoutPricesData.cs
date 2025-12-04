namespace MOSTComputers.Services.PDF.Models.WarrantyCards;
public sealed class WarrantyCardWithoutPricesData
{
    public int OrderId { get; init; }
    public DateTime? ExportDate { get; init; }
    public string? CustomerName { get; init; }
    public DateTime? WarrantyCardDate { get; init; }
    public int? WarrantyCardTermInMonths { get; init; }

    public List<WarrantyCardItemWithoutPricesData>? WarrantyCardItems { get; init; }
}