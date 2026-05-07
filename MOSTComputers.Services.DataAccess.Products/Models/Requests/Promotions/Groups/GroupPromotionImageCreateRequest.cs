namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;

public sealed class GroupPromotionImageCreateRequest
{
    public int? PromotionId { get; init; }
	public byte[]? Image { get; init; }
	public string? ContentType { get; init; }
}
