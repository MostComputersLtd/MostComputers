using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IToDoLocalChangesService
{
    Task<List<LocalChangeData>> GetAllAsync();
    Task<List<LocalChangeData>> GetAllForOperationTypeAsync(ChangeOperationType operationType);
    Task<List<LocalChangeData>> GetAllForTableAsync(string tableName);
    Task<LocalChangeData?> GetByIdAsync(int id);
    Task<LocalChangeData?> GetByTableNameAndElementIdAndOperationTypeAsync(string tableName, int elementId, ChangeOperationType changeOperationType);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceToDoLocalChangeCreateRequest createRequest);
    Task<bool> DeleteByIdAsync(int id);
    Task<bool> DeleteAllByTableNameAndElementIdAsync(string tableName, int elementId);
    Task<bool> DeleteRangeByIdsAsync(IEnumerable<int> ids);
    Task<bool> DeleteRangeByTableNameAndElementIdsAsync(string tableName, IEnumerable<int> elementIds);
}