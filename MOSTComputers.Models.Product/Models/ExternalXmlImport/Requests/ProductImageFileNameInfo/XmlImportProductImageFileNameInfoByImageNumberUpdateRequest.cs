namespace MOSTComputers.Models.Product.Models.ExternalXmlImport.Requests.ProductImageFileNameInfo;

public sealed class XmlImportProductImageFileNameInfoByImageNumberUpdateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public bool ShouldUpdateDisplayOrder { get; set; }
    public int? NewDisplayOrder { get; set; }
    public int ImageNumber { get; set; }
    public bool Active { get; set; }
    public int? ImagesInImagesAllForProductCount { get; set; }
    public bool IsProductFirstImageInImages { get; set; }
}