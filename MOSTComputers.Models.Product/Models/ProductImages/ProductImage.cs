namespace MOSTComputers.Models.Product.Models.ProductImages;

public sealed class ProductImage
{
    public int Id { get; init; }
    public int? ProductId { get; init; }
    public string? HtmlData { get; init; }
    public byte[]? ImageData { get; init; }
    public string? ImageContentType { get; init; }
    public DateTime? DateModified { get; init; }
}