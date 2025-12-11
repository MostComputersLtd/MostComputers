using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties;
public sealed class ProductPropertyEditorPropertyData
{
    public int ProductId { get; set; }
    public ProductCharacteristic? ProductCharacteristic { get; set; }
    public int? DisplayOrder { get; set; }
    public string? Value { get; set; }
    public List<ProductCharacteristic>? RemainingCharacteristics { get; init; }
}