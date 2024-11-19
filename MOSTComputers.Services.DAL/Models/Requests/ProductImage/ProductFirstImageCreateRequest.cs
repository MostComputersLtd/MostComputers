namespace MOSTComputers.Services.DAL.Models.Requests.ProductImage;

public sealed class ProductFirstImageCreateRequest
{
    public int ProductId { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
    public DateTime? DateModified { get; set; }
}