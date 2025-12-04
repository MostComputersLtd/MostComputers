namespace MOSTComputers.Models.Product.Models;

public sealed class Manufacturer
{
    public int Id { get; init; }
    public string? BGName { get; init; }
    public string? RealCompanyName { get; init; }
    public int? DisplayOrder { get; init; }
    public bool? Active { get; init; }
}