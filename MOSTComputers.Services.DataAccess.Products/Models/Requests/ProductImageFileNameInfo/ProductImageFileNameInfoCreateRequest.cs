namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImageFileNameInfo;

public sealed class ProductImageFileNameInfoCreateRequest
{
    public required int ProductId { get; set; }
    public int? ImageId { get; set; }
    public string? FileName { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public bool Active { get; set; }
    public required string CreateUserName { get; init; }
    public required DateTime CreateDate { get; init; }
    public required string LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
}