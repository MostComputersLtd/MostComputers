using MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductCharacteristicsComparer;
public class LocalCharacteristicsListPartialModel
{
    public required IEnumerable<LocalCharacteristicDisplayData> Characteristics { get; init; }
    public required IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> ExternalAndLocalCharacteristicRelations { get; init; }
}