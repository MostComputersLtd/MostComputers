namespace MOSTComputers.Services.DAL.Models.Requests.Manifacturer;

public sealed class ManifacturerUpdateRequest
{
    public int Id { get; set; }
    public string? BGName { get; set; }
    public string? RealCompanyName { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? Active { get; set; }
}
