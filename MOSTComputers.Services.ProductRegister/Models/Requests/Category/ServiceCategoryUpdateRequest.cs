namespace MOSTComputers.Services.ProductRegister.Models.Requests.Category;

public sealed class ServiceCategoryUpdateRequest
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public int? DisplayOrder { get; set; }
    public int? ProductsUpdateCounter { get; set; }
}