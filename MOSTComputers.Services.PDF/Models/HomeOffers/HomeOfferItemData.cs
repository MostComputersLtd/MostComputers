namespace MOSTComputers.Services.PDF.Models.HomeOffers;

public sealed class HomeOfferItemData
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public required decimal LineNetPrice { get; set; }
    public required decimal LineVatPrice { get; set; }
    public required decimal LineTotalPrice { get; set; }
}
