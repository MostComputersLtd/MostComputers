namespace MOSTComputers.Models.Product.Models.Promotions.Groups;
public sealed class GroupPromotionImageFileData
{
    public required int Id { get; init; }
    public int? PromotionId { get; init; }
    public required int ImageId { get; init; }
    public required string FileName { get; init; }
}