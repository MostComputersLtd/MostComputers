namespace MOSTComputers.Models.Product.Models.Requests.ProductImage;

public sealed class ProductImageUpdateRequest
{
    public int Id { get; set; }
    public string? XML { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
    public DateTime? DateModified { get; set; }
}