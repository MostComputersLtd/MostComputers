namespace MOSTComputers.Models.Product.Models;

public sealed class Category
{
    public int Id { get; init; }
    public string? Description { get; init; }
    public bool? IsLeaf { get; init; }
    public int? DisplayOrder { get; init; }
    public Guid? RowGuid { get; init; }
    public int? ProductsUpdateCounter { get; init; }
    public int? ParentCategoryId { get; init; }
}