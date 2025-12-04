namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionProductFiles;

public sealed class PromotionProductFileInfoCreateRequest
{
    public required int ProductId { get; init; }
    public required int PromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required bool Active { get; init; }
    public int? ProductImageId { get; init; }
    public required string CreateUserName { get; init; }
    public required DateTime CreateDate { get; init; }
    public required string LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
}