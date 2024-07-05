namespace MOSTComputers.Models.Product.Models.Requests.ProductImage;

public sealed class ServiceProductFirstImageCreateRequest
{
    public int? ProductId { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
}