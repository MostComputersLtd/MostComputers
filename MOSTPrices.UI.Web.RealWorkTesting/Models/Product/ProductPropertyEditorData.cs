using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

public class ProductPropertyEditorData
{
    public int ProductCharacteristicId { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
    public bool IsNew { get; set; }
}