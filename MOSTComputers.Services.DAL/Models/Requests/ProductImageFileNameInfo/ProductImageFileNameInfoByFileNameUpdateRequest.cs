namespace MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;

public sealed class ProductImageFileNameInfoByFileNameUpdateRequest
{
    public int ProductId { get; set; }
    public required string FileName { get; set; }
    public string? NewFileName { get; set; }
    public bool ShouldUpdateDisplayOrder { get; set; }
    public int? NewDisplayOrder { get; set; }
    public bool Active { get; set; }
}