namespace MOSTComputers.Services.DAL.Models.Requests.Category;

public sealed class CategoryUpdateRequest
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public bool? IsLeaf => ParentCategoryId is not null;
    public int? DisplayOrder { get; set; }
    public Guid? RowGuid { get; set; }
    public int? ProductsUpdateCounter { get; set; }
    public int? ParentCategoryId { get; set; }
}