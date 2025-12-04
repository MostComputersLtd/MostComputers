namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImageFileNameInfo;

public sealed class ProductImageFileNameInfoByIdUpdateRequest
{
    public int Id { get; set; }
    public int? ImageId { get; set; }
    public string? FileName { get; set; }
    public bool ShouldUpdateDisplayOrder { get; set; }
    public int? NewDisplayOrder { get; set; }
    public bool Active { get; set; }
    public required string LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
}