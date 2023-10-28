namespace MOSTComputers.Services.DAL.Models.Requests.Category;

public class CategoryCreateRequest
{
    public string? Description { get; set; }
    public bool? IsLeaf => ParentCategoryId != null;
    public int? DisplayOrder { get; set; }
    public Guid? RowGuid { get; set; }
    public int? ProductsUpdateCounter { get; set; }
    public int? ParentCategoryId { get; set; }
}