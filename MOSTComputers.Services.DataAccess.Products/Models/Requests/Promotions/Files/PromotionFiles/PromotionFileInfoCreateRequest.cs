namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionFiles;
public sealed class PromotionFileInfoCreateRequest
{
    public string? Name { get; init; }
    public required bool Active { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required string FileName { get; init; }
    public string? Description { get; init; }
    public string? RelatedProductsString { get; init; }
    public required string CreateUserName { get; init; }
    public required DateTime CreateDate { get; init; }
    public required string LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
}