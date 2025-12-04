namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;

public sealed class ServiceProductFirstImageUpsertRequest
{
    public int ProductId { get; set; }
    public string? HtmlData { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
}