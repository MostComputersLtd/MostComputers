namespace MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;

public sealed class XmlImportServiceProductImageFileNameInfoCreateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? Active { get; set; }
    public int? ImagesInImagesAllForProductCount { get; set; }
    public bool IsProductFirstImageInImages { get; set; }
}