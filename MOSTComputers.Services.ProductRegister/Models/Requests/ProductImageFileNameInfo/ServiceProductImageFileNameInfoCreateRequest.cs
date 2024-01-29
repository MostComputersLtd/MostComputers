namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;

public sealed class ServiceProductImageFileNameInfoCreateRequest
{
    public int ProductId { get; set; }
    public string? FileName { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? Active { get; set; }
}