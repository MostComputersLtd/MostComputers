namespace MOSTComputers.Services.ProductRegister.Models.Requests.Product;

public sealed class ImageFileAndFileNameInfoUpsertRequest
{
    public required string ImageContentType { get; set; }
    public required byte[] ImageData { get; set; }
    public string? OldFileName { get; set; }
    public int? RelatedImageId { get; set; }
    public string? CustomFileNameWithoutExtension { get; set; }
    public int? DisplayOrder { get; set; }
    public bool Active { get; set; }
}