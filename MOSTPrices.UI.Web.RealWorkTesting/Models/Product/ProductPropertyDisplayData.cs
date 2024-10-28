using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

public class ProductPropertyDisplayData
{
    public int? ProductCharacteristicId { get; set; }
    public int? DisplayOrder { get; set; }
    public int? CustomDisplayOrderOnInsertOrUpdate { get; set; }
    public string? Characteristic { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}