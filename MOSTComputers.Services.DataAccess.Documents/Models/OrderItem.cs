namespace MOSTComputers.Services.DataAccess.Documents.Models;
public class OrderItem
{
	public required int Id { get; init; }
	public int OrderId { get; init; }
	public int? ProductId { get; init; }
	public int? Quantity { get; init; }
	public decimal? Price { get; init; }
	public decimal? AdditionalWarranty { get; init; }
	public decimal? FDDC { get; init; }
	public byte PriceGroup { get; init; }
	public decimal? ProfitPercent { get; init; }
	public int? PromotionPId { get; init; }
	public int? PromotionRId { get; init; }
	public decimal? PromotionPAmount { get; init; }
	public decimal? PromotionRAmount { get; init; }
	public string? STInfo { get; init; }
	public string? STInfoW { get; init; }
	public string? ExternalInfo { get; init; }
	public string? InternalInfo { get; init; }
}
