namespace MOSTComputers.UI.Web.Models.PromotionFiles.DTOs;

public sealed class PromotionProductFileUpdateDTO
{
    public int? Id { get; init; }
    public int? PromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool Active { get; init; }
    public bool ShouldAddToImagesAll { get; init; }
}