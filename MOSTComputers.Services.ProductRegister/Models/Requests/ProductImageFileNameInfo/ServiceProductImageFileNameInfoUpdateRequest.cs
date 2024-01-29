namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;

public sealed class ServiceProductImageFileNameInfoUpdateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public int? DisplayOrder { get; set; }
    public int? NewDisplayOrder { get; set; }
    public bool? Active { get; set; }
}