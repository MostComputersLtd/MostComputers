using MOSTComputers.UI.Web.RealWorkTesting.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductCharacteristicsComparer;

public class ExternalPropertiesListPartialModel
{
    public required IEnumerable<ExternalXmlPropertyDisplayData> Properties { get; init; }
    public required IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> ExternalAndLocalCharacteristicRelations { get; init; }
}