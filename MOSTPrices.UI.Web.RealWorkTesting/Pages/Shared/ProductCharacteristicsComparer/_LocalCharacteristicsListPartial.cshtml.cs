using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductCharacteristicsComparer;

public class LocalCharacteristicsListPartialModel
{
    public required IEnumerable<LocalCharacteristicDisplayData> Characteristics { get; init; }
    public required IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> ExternalAndLocalCharacteristicRelations { get; init; }
}