using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties.DTOs;
public sealed class ProductPropertyEditorPropertyDTO
{
    public int PropertyIndex { get; set; }
    public int PropertyGroup { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public string? Value { get; set; }
    public XMLPlacementEnum? XmlPlacement { get; set; }
}