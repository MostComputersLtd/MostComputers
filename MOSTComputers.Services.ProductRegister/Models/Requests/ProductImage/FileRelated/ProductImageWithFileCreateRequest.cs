namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

public sealed class ProductImageWithFileCreateRequest
{
    public required int ProductId { get; init; }
    public required string FileExtension { get; init; }
    public required byte[] ImageData { get; init; }
    public string? HtmlData { get; init; }
    public int? CustomDisplayOrder { get; init; }
    public bool? Active { get; init; }
    public required string CreateUserName { get; init; }
}