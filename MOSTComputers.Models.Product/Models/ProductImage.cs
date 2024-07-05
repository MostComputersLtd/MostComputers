namespace MOSTComputers.Models.Product.Models;

public sealed class ProductImage
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
    public DateTime? DateModified { get; set; }
}