namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

public sealed class ProductImageWithFileUpsertRequest
{
    public required int ProductId { get; init; }
    public required int? ExistingImageId { get; init; }
    public required string FileExtension { get; init; }
    public required byte[] ImageData { get; init; }
    public string? HtmlData { get; init; }
    public FileForImageUpsertRequest? FileUpsertRequest { get; init; }
}