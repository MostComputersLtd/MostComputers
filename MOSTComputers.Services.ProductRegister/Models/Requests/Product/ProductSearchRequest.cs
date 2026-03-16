namespace MOSTComputers.Services.ProductRegister.Models.Requests.Product;

public class ProductSearchRequest
{
    public bool OnlyVisibleByEndUsers { get; init; } = false;
    public string? UserInputString { get; init; }
    public int? CategoryId { get; init; }
    public int? ManufacturerId { get; init; }
    public ProductStatusSearchOptions? ProductStatus { get; init; }
    public List<ProductNewStatusSearchOptions>? ProductNewStatuses { get; init; }
    public PromotionSearchOptions? PromotionSearchOptions { get; init; }
    public int? MaxResultCount { get; init; }
}