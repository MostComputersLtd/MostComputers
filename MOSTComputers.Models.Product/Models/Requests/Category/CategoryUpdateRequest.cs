namespace MOSTComputers.Models.Product.Models.Requests.Category;

public sealed class CategoryUpdateRequest
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
    public Guid? RowGuid { get; set; }
    public int? ProductsUpdateCounter { get; set; }
}