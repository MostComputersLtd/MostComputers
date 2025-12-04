namespace MOSTComputers.Models.Product.Models.Promotions.Groups;
public sealed class GroupPromotionImage
{
    public int Id { get; init; }
    public int? PromotionId { get; init; }
	public byte[]? Image { get; init; }
	public string? ContentType { get; init; }
}