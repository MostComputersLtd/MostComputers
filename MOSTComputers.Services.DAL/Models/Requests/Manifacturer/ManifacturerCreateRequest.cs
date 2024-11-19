namespace MOSTComputers.Services.DAL.Models.Requests.Manifacturer;

public sealed class ManifacturerCreateRequest
{
    public string? BGName { get; set; }
    public string? RealCompanyName { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? Active { get; set; }
}
