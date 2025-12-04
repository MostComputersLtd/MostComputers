namespace MOSTComputers.Services.PDF.Models.WarrantyCards;
public sealed class WarrantyCardItemWithPricesData
{
    public string? ProductName { get; set; }
    public string? SerialNumber { get; set; }
    public int? WarrantyCardItemTermInMonths { get; set; }
    public decimal? PriceInLeva { get; set; }
    public int? Quantity { get; set; }
    public int? DisplayOrder { get; set; }
}