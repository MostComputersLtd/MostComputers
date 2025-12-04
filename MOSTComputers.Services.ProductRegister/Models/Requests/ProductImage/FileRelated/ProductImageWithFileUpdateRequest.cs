namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

public sealed class ProductImageWithFileUpdateRequest
{
    public required int ImageId { get; init; }
    public required string FileExtension { get; init; }
    public required byte[] ImageData { get; init; }
    public string? HtmlData { get; init; }
    public int? CustomDisplayOrder { get; init; }
    public bool? Active { get; init; }
    public required string UpdateUserName { get; init; }
}