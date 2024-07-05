using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface IToDoLocalChangesRepository
{
    bool DeleteById(uint id);
    bool DeleteByTableNameAndElementId(string tableName, int elementId);
    bool DeleteRangeByIds(IEnumerable<uint> ids);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
    IEnumerable<LocalChangeData> GetAll();
    IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum changeOperationType);
    IEnumerable<LocalChangeData> GetAllForTable(string tableName);
    LocalChangeData? GetById(uint id);
    LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationType);
    OneOf<int, UnexpectedFailureResult> Insert(ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest);
}