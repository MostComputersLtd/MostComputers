namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;

public sealed class GroupPromotionImageUpdateRequest
{
    public required int Id { get; init; }
    public int? PromotionId { get; init; }
	public byte[]? Image { get; init; }
	public string? ContentType { get; init; }
}