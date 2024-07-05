using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IToDoLocalChangesService
{
    bool DeleteById(uint id);
    bool DeleteByTableNameAndElementId(string tableName, int elementId);
    bool DeleteRangeByIds(IEnumerable<uint> ids);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
    IEnumerable<LocalChangeData> GetAll();
    IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum operationType);
    IEnumerable<LocalChangeData> GetAllForTable(string tableName);
    LocalChangeData? GetById(uint id);
    LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationType);
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ToDoLocalChangeCreateRequest createRequest, IValidator<ToDoLocalChangeCreateRequest>? validator = null);
}