namespace MOSTComputers.Models.Product.Models.Requests.ProductProperty;

public sealed class ProductPropertyByCharacteristicIdCreateRequest
{
    public int ProductId { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}