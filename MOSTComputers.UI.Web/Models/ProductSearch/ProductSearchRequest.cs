using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.UI.Web.Models.ProductSearch;

public class ProductSearchRequest
{
    public string? UserInputString { get; init; }
    public int? CategoryId { get; init; }
    public ProductStatusSearchOptions? ProductStatus { get; init; }
    public List<ProductNewStatusSearchOptions>? ProductNewStatuses { get; init; }
    public PromotionSearchOptions? PromotionSearchOptions { get; init; }
    public int? MaxResultCount { get; init; }
}