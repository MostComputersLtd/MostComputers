namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductImages;

public sealed class ProductImageFileDisplayData
{
    public required int ProductId { get; init; }
    public required int? ExistingFileInfoId { get; init; }
    public string? FileName { get; init; }
    public int? DisplayOrder { get; init; }
    public bool? Active { get; init; }
}