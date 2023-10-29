namespace MOSTComputers.Services.DAL.Models.Requests.ProductImage;

public class ProductImageCreateRequest
{
    public int? ProductId { get; set; }
    public string? XML { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
    public DateTime? DateModified { get; } = DateTime.Now;
}