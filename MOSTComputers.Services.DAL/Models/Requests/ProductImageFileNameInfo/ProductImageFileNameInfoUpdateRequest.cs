namespace MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;

public sealed class ProductImageFileNameInfoUpdateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public int? DisplayOrder { get; set; }
    public int? NewDisplayOrder { get; set; }
}