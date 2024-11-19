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
    IEnumerable<LocalChangeData> GetAll();
    IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum operationType);
    IEnumerable<LocalChangeData> GetAllForTable(string tableName);
    LocalChangeData? GetById(int id);
    LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationType);
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ServiceToDoLocalChangeCreateRequest createRequest, IValidator<ServiceToDoLocalChangeCreateRequest>? validator = null);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
    bool DeleteRangeByIds(IEnumerable<int> ids);
    bool DeleteAllByTableNameAndElementId(string tableName, int elementId);
    bool DeleteById(int id);
}