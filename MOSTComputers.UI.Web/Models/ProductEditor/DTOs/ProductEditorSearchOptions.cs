using MOSTComputers.UI.Web.Models.ProductSearch;

namespace MOSTComputers.UI.Web.Models.ProductEditor.DTOs;
public sealed class ProductEditorSearchOptions
{
    public string? UserInputString { get; init; }
    public int? CategoryId { get; init; }
    public ProductStatusSearchOptions? ProductStatusSearchOptions { get; init; }
    public ProductEditorProductNewStatusSearchOptions? ProductNewStatusSearchOptions { get; init; }
    public PromotionSearchOptions? PromotionSearchOptions { get; init; }
}