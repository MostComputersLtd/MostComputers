namespace MOSTComputers.Models.Product.Models;

public sealed class ProductFirstImage
{
    public int Id { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
    public DateTime? DateModified { get; set; }
}