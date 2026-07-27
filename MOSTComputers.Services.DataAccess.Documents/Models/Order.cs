using MOSTComputers.Models.Common;

namespace MOSTComputers.Services.DataAccess.Documents.Models;
public class Order
{
	public required int Id { get; init; }
	public byte? Status { get; init; }
	public bool? AutoReply { get; init; }
	public int? QuoteId { get; init; }
	public int? UserId { get; init; }
	public DateTime? OrderDate { get; init; }
	public string? OrderName { get; init; }
	public DateTime? ReadTime { get; init; }
	public int? ReadUserId { get; init; }
	public int? BusinessId { get; init; }
	public int? DealId { get; init; }
	public int? MOST3DealId { get; init; }
	public bool? IsConfiguration { get; init; }
	public decimal? CfgProfit { get; init; }
	public Currency? Currency { get; init; }
	public int? OriginalInternetOrderId { get; init; }
	public int? RepliedInternetOrderId { get; init; }
	public string? Info { get; init; }

    public List<OrderItem> Items { get; init; } = new();
}
