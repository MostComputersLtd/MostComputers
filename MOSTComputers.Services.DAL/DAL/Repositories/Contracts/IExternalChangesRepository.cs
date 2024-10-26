using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.External;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface IExternalChangesRepository
{
    IEnumerable<ExternalChangeData> GetAll();
    IEnumerable<ExternalChangeData> GetAllForOperationType(ChangeOperationTypeEnum changeOperationType);
    IEnumerable<ExternalChangeData> GetAllForTable(string tableName);
    IEnumerable<ExternalChangeData> GetAllByTableNameAndElementId(string tableName, int elementId);
    ExternalChangeData? GetById(int id);
    bool DeleteById(int id);
    bool DeleteAllByTableNameAndElementId(string tableName, int elementId);
    bool DeleteRangeByIds(IEnumerable<int> ids);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
    bool DeleteByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum operationType);
}