using MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductCharacteristicsComparer;
public class ExternalPropertiesListPartialModel
{
    public required IEnumerable<ExternalXmlPropertyDisplayData> Properties { get; init; }
    public required IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> ExternalAndLocalCharacteristicRelations { get; init; }
}