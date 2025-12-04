using MOSTComputers.UI.Web.Models.ProductCharacteristicsComparer;

namespace MOSTComputers.UI.Web.Services.Data.PageDataStorage.Contracts;
public interface IProductCharacteristicRelationsStorageService
{
    IReadOnlyList<ExternalAndLocalCharacteristicRelationDisplayData> ProductCharacteristicRelations { get; }

    void Add(ExternalAndLocalCharacteristicRelationDisplayData relation);
    void AddRange(IEnumerable<ExternalAndLocalCharacteristicRelationDisplayData> relations);
    void Clear();
    bool Remove(ExternalAndLocalCharacteristicRelationDisplayData relation);
}