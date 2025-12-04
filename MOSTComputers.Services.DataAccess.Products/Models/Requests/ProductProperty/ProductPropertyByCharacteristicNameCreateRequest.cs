using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
public sealed class ProductPropertyByCharacteristicNameCreateRequest
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public required string ProductCharacteristicName { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}