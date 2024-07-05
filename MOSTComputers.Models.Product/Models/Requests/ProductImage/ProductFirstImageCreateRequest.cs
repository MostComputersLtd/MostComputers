namespace MOSTComputers.Models.Product.Models.Requests.ProductImage;

public sealed class ProductFirstImageCreateRequest
{
    public int? ProductId { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
    public DateTime? DateModified { get; set; }
}