using MOSTComputers.Models.Product.Models;
using System.Drawing;

namespace MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;
public class LocalCharacteristicDisplayData
{
    public int? Id { get; set; }
    public int? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Meaning { get; set; }
    public int? DisplayOrder { get; set; }
    public ProductCharacteristicType? KWPrCh { get; set; }
    public Color? CustomBackgroundColor { get; set; }
}