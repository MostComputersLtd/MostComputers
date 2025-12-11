namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
public sealed class ProductImageUpsertRequest
{
    public required int ProductId { get; set; }
    public required int? ExistingImageId { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
}