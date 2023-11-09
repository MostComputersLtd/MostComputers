namespace MOSTComputers.Services.ProductRegister.Models.Requests.Category;

public sealed class ServiceCategoryCreateRequest
{
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
    public int? ProductsUpdateCounter { get; set; }
    public int? ParentCategoryId { get; set; }
}