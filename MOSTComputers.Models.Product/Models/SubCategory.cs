namespace MOSTComputers.Models.Product.Models;
public sealed class SubCategory
{
    public int? Id { get; init; }
    public int? CategoryId { get; init; }
    public string? Name { get; init; }
    public int? DisplayOrder { get; init; }
    public bool? Active { get; init; }
}