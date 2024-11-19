using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.DAL.Models.Requests.ProductProperty;

public sealed class ProductPropertyByCharacteristicIdCreateRequest
{
    public int ProductId { get; set; }
    public int ProductCharacteristicId { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}