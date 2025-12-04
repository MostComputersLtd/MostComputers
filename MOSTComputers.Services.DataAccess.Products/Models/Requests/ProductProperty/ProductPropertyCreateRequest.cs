using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
public sealed class ProductPropertyCreateRequest
{
    public int ProductId { get; set; }
    public int ProductCharacteristicId { get; set; }
    public string? Name { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}