namespace MOSTComputers.Services.DataAccess.Products.Models.DAOs.ProductImage;

internal sealed class ProductFirstImageDAO
{
    public int Id { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
    public DateTime? DateModified { get; set; }
}