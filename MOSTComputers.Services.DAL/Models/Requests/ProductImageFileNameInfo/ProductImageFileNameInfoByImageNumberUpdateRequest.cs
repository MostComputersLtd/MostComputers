namespace MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;

public sealed class ProductImageFileNameInfoByImageNumberUpdateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public bool ShouldUpdateDisplayOrder { get; set; }
    public int? NewDisplayOrder { get; set; }
    public int ImageNumber { get; set; }
    public bool Active { get; set; }
}