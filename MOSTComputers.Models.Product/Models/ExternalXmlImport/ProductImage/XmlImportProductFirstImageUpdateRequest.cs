namespace MOSTComputers.Models.Product.Models.ExternalXmlImport.ProductImage;

public sealed class XmlImportProductFirstImageUpdateRequest
{
    public int ProductId { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
    public DateTime? DateModified { get; set; }
}