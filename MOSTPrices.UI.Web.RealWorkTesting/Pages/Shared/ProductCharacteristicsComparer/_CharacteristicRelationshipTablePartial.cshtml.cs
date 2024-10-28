using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductCharacteristicsComparer;

public class CharacteristicRelationshipTablePartialModel
{
    public required List<ExternalAndLocalCharacteristicRelationDisplayData> ExternalAndLocalCharacteristicRelations { get; init; }
    public required List<Category> CategoriesForItems { get; init; }
    public required string RelationshipTableViewContainerId { get; init; }
    public required string RelationshipXmlViewContainerId { get; init; }

    public bool AreAllItemsInTheSameCategory()
    {
        return CategoriesForItems.Count == 1;
    }
}