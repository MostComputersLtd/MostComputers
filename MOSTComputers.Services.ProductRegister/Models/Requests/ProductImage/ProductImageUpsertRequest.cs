namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
public sealed class ProductImageUpsertRequest
{
    public required int ProductId { get; init; }
    public required int? ExistingImageId { get; init; }
    public string? HtmlData { get; init; }
    public byte[]? ImageData { get; init; }
    public string? ImageContentType { get; init; }
}