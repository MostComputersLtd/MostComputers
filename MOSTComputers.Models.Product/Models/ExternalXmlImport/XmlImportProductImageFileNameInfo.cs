namespace MOSTComputers.Models.Product.Models.ExternalXmlImport;

public sealed class XmlImportProductImageFileNameInfo
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public int? DisplayOrder { get; set; }
    public int ImageNumber { get; set; }
    public bool Active { get; set; }
    public int? ImagesInImagesAllForProductCount { get; set; }
    public bool IsProductFirstImageInImages { get; set; }
}