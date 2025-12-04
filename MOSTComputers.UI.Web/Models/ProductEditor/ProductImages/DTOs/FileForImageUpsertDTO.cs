namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductImages.DTOs;

public sealed class FileForImageUpsertDTO
{
    public required int? ExistingFileInfoId { get; init; }
    public int? DisplayOrder { get; init; }
    public bool? Active { get; init; }
}