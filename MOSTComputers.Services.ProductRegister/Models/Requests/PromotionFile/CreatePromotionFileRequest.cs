namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;

public sealed class CreatePromotionFileRequest
{
    public string? Name { get; init; }
    public required bool Active { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required string FileName { get; init; }
    public required byte[] FileData { get; init; }
    public string? Description { get; init; }
    public string? RelatedProductsString { get; init; }
    public required string CreateUserName { get; init; }
}