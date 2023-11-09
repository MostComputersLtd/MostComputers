namespace MOSTComputers.Models.Product.Models;

public sealed class Category
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public bool? IsLeaf { get; set; }
    public int? DisplayOrder { get; set; }
    public Guid? RowGuid { get; set; }
    public int? ProductsUpdateCounter { get; set; }
    public int? ParentCategoryId { get; set; }
}