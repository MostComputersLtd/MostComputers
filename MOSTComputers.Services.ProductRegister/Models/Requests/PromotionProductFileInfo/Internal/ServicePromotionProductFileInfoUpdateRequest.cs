namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;

internal sealed class ServicePromotionProductFileInfoUpdateRequest
{
    public required int Id { get; init; }
    public int? NewPromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required bool Active { get; init; }
    public int? ProductImageId { get; init; }
    public required string UpdateUserName { get; init; }
}