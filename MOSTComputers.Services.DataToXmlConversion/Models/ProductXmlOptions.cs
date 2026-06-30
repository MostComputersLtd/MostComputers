using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataToXmlConversion.Models;

public sealed class ProductXmlOptions
{
    public string? ImageFilesBasePath { get; set; }
    public string? GroupPromotionsBasePath { get; set; }
    public string? PromotionGroupsBasePath { get; set; }
    public Func<int, string>? GetPromotionPictureSourceUrlById { get; set; }
    public Currency? PrefferedPriceCurrency { get; set; }
}
