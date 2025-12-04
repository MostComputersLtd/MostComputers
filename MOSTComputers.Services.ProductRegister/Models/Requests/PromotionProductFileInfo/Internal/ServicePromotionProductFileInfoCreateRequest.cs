namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo.Internal;

internal sealed class ServicePromotionProductFileInfoCreateRequest
{
    public required int ProductId { get; init; }
    public required int PromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required bool Active { get; init; }
    public int? ProductImageId { get; init; }
    public required string CreateUserName { get; init; }
}