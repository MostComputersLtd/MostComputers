namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionProductFiles;

public sealed class PromotionProductFileInfoUpdateRequest
{
    public required int Id { get; init; }
    public int? NewPromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required bool Active { get; init; }
    public int? ProductImageId { get; init; }
    public required string LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
}