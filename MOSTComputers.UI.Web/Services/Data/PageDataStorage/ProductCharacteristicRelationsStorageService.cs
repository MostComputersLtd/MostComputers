using MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;
using MOSTComputers.UI.Web.Services.Data.PageDataStorage.Contracts;

namespace MOSTComputers.UI.Web.Services.Data.PageDataStorage;
public class ProductCharacteristicRelationsStorageService : IProductCharacteristicRelationsStorageService
{
    private readonly List<ExternalAndLocalCharacteristicRelationDisplayData> _productCharacteristicRelations = new();
    public IReadOnlyList<ExternalAndLocalCharacteristicRelationDisplayData> ProductCharacteristicRelations => _productCharacteristicRelations;

    public void Add(ExternalAndLocalCharacteristicRelationDisplayData relation)
    {
        _productCharacteristicRelations.Add(relation);
    }

    public void AddRange(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> relations)
    {
        _productCharacteristicRelations.AddRange(relations);
    }

    public bool Remove(ExternalAndLocalCharacteristicRelationDisplayData relation)
    {
        return _productCharacteristicRelations.Remove(relation);
    }

    public void Clear()
    {
        _productCharacteristicRelations.Clear();
    }
}