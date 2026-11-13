namespace MOSTComputers.Services.PDF.Models.HomeOffers;

public sealed class HomeOfferData
{
    public int OfferNumber { get; set; }
    public string? AdditionalInfo { get; init; }
    public DateTime? Date { get; set; }
    public List<HomeOfferItemData> Items { get; set; } = new();
}
