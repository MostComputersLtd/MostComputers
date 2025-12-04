namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductImages.DTOs;

public sealed class ProductImageUpsertDTO
{
    public bool? IncludeHtmlDataView { get; init; }
    public int? ProductId { get; init; }
    public int? ExistingImageId { get; init; }
    public IFormFile? File { get; init; }
    public FileForImageUpsertDTO? FileUpsertData { get; init; }
}