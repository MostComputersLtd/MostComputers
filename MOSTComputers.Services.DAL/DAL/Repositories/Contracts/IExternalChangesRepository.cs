using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.External;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface IExternalChangesRepository
{
    IEnumerable<ExternalChangeData> GetAll();
    IEnumerable<ExternalChangeData> GetAllForOperationType(ChangeOperationTypeEnum changeOperationType);
    IEnumerable<ExternalChangeData> GetAllForTable(string tableName);
    ExternalChangeData? GetById(int id);
    ExternalChangeData? GetByTableNameAndElementId(string tableName, int elementId);
    bool DeleteById(int id);
    bool DeleteByTableNameAndElementId(string tableName, int elementId);
    bool DeleteRangeByIds(IEnumerable<int> ids);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
}