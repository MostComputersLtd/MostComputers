using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface ILocalChangesRepository
{
    IEnumerable<LocalChangeData> GetAll();
    IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum changeOperationType);
    IEnumerable<LocalChangeData> GetAllForTable(string tableName);
    LocalChangeData? GetById(int id);
    LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationTypeEnum);
    bool DeleteById(int id);
    bool DeleteByTableNameAndElementId(string tableName, int elementId);
    bool DeleteRangeByIds(IEnumerable<int> ids);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
}