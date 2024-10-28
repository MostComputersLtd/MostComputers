using System.Drawing;

namespace MOSTComputers.UI.Web.RealWorkTesting.Models.ProductCharacteristicsComparer;

public class ExternalXmlPropertyDisplayData
{
    public required int CategoryId { get; set; }

    public required string Name { get; set; }

    public required int? Order { get; set; }

    public string? Value { get; set; }

    public Color? CustomBackgroundColor { get; set; }
}