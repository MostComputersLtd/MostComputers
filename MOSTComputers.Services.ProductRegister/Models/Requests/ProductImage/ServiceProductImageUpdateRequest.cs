namespace MOSTComputers.Models.Product.Models.Requests.ProductImage;

public sealed class ServiceProductImageUpdateRequest
{
    public int Id { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
}