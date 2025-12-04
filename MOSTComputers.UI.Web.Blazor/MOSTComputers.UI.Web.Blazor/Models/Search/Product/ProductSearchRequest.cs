namespace MOSTComputers.UI.Web.Blazor.Models.Search.Product;

public class ProductSearchRequest
{
    public bool OnlyVisibleByEndUsers { get; init; } = true;
    public string? UserInputString { get; init; }
    public int? CategoryId { get; init; }
    public int? ManufacturerId { get; init; }
    public ProductStatusSearchOptions? ProductStatus { get; init; }
    public List<ProductNewStatusSearchOptions>? ProductNewStatuses { get; init; }
    public PromotionSearchOptions? PromotionSearchOptions { get; init; }
    public int? MaxResultCount { get; init; }
}