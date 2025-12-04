namespace MOSTComputers.UI.Web.Models.PromotionFiles.DTOs;

public sealed class PromotionProductFileInsertDTO
{
    public int? ProductId { get; init; }
    public int? PromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool Active { get; init; }
    public bool ShouldAddToImagesAll { get; init; }
}