namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;

public sealed class UpdatePromotionFileRequest
{
    public required int Id { get; init; }
    public string? Name { get; init; }
    public required bool Active { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public FileData? NewFileData { get; init; }
    public string? Description { get; init; }
    public string? RelatedProductsString { get; init; }
    public required string UpdateUserName { get; init; }
}