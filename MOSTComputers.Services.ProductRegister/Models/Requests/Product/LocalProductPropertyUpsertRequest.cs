using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.Product;

public sealed class LocalProductPropertyUpsertRequest
{
    public int ProductCharacteristicId { get; set; }
    public int? CustomDisplayOrder { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}