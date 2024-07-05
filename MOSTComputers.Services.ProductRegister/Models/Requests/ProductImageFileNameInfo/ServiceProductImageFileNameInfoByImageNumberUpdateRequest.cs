namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;

public sealed class ServiceProductImageFileNameInfoByImageNumberUpdateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public int? NewDisplayOrder { get; set; }
    public int ImageNumber { get; set; }
    public bool? Active { get; set; }
}