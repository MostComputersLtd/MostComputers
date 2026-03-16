namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData;

public class CarouselPartialsModel
{
    public required DefaultModel.ProductImageDisplayData Image { get; set; }
    public required string CarouselId { get; set; }
    public required int ImageCarouselIndex { get; set; }
    public bool IsActive { get; set; } = false;
}
