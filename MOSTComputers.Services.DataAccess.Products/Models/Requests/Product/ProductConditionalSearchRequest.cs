using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Product;
public sealed class ProductConditionalSearchRequest
{
    public ProductStatus? Status { get; set; }
    public string? NameSubstring { get; set; }
    public string? SearchStringSubstring { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsProcessed { get; set; }
    public bool? NeedsToBeUpdated { get; set; }
}