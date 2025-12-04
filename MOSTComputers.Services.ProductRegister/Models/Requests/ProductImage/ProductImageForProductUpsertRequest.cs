namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;

public class ProductImageForProductUpsertRequest
{
    public int? ExistingImageId { get; set; }
    public string? HtmlData { get; init; }
    public required string ImageContentType { get; init; }
    public byte[]? ImageData { get; init; }
}