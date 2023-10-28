namespace MOSTComputers.Services.DAL.Models.Requests.Manifacturer;

public class ManifacturerInsertRequest
{
    public string? BGName { get; set; }
    public string? RealCompanyName { get; set; }
    public int? DisplayOrder { get; set; }
    public float? Active { get; set; }
}
