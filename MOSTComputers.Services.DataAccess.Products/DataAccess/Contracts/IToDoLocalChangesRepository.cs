using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ToDoLocalChanges;
using OneOf;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IToDoLocalChangesRepository
{
    Task<List<LocalChangeData>> GetAllAsync();
    Task<List<LocalChangeData>> GetAllForOperationTypeAsync(ChangeOperationType changeOperationType);
    Task<List<LocalChangeData>> GetAllForTableAsync(string tableName);
    Task<LocalChangeData?> GetByIdAsync(int id);
    Task<LocalChangeData?> GetByTableNameAndElementIdAndOperationTypeAsync(string tableName, int elementId, ChangeOperationType changeOperationType);
    Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest);
    Task<bool> DeleteRangeByIdsAsync(IEnumerable<int> ids);
    Task<bool> DeleteRangeByTableNameAndElementIdsAsync(string tableName, IEnumerable<int> elementIds);
    Task<bool> DeleteByIdAsync(int id);
    Task<bool> DeleteByTableNameAndElementIdAsync(string tableName, int elementId);
}