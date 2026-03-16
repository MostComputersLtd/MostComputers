namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.GroupPromotionCarousel;

public class GroupPromotionsCarouselItemModel
{
    public required List<string> ImageUrls { get; set; }
    public required string CarouselId { get; set; }
    public required int PromotionImageFileId { get; set; }
    public required int PromotionGroupId { get; set; }
    public required int Index { get; set; }
    public required bool IsActive { get; set; }
}