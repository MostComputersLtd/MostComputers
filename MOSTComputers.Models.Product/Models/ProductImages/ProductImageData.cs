namespace MOSTComputers.Models.Product.Models.ProductImages;
public sealed class ProductImageData
{
    public int Id { get; init; }
    public int? ProductId { get; init; }
    public string? HtmlData { get; init; }
    public string? ImageContentType { get; init; }
    public DateTime? DateModified { get; init; }
}