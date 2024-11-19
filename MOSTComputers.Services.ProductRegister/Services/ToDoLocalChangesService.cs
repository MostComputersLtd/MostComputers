using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using FluentValidation;
using FluentValidation.Results;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Services.DAL.Models.Requests.ToDoLocalChanges;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ToDoLocalChangesService : IToDoLocalChangesService
{
    public ToDoLocalChangesService(
        IToDoLocalChangesRepository toDoLocalChangesRepository,
        ProductMapper productMapper,
        IValidator<ServiceToDoLocalChangeCreateRequest>? createRequestValidator = null)
    {
        _toDoLocalChangesRepository = toDoLocalChangesRepository;
        _productMapper = productMapper;
        _createRequestValidator = createRequestValidator;
    }

    private readonly IToDoLocalChangesRepository _toDoLocalChangesRepository;
    private readonly ProductMapper _productMapper;
    private readonly IValidator<ServiceToDoLocalChangeCreateRequest>? _createRequestValidator;

    public IEnumerable<LocalChangeData> GetAll()
    {
        return _toDoLocalChangesRepository.GetAll();
    }

    public IEnumerable<LocalChangeData> GetAllForTable(string tableName)
    {
        return _toDoLocalChangesRepository.GetAllForTable(tableName);
    }

    public IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum operationType)
    {
        return _toDoLocalChangesRepository.GetAllForOperationType(operationType);
    }

    public LocalChangeData? GetById(int id)
    {
        if (id <= 0) return null;

        return _toDoLocalChangesRepository.GetById(id);
    }

    public LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationType)
    {
        return _toDoLocalChangesRepository.GetByTableNameAndElementIdAndOperationType(tableName, elementId, changeOperationType);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ServiceToDoLocalChangeCreateRequest createRequest,
        IValidator<ServiceToDoLocalChangeCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest = _productMapper.Map(createRequest);

        toDoLocalChangeCreateRequest.TimeStamp = DateTime.Now;

        OneOf<int, UnexpectedFailureResult> toDoLocalChangeInsertResult = _toDoLocalChangesRepository.Insert(toDoLocalChangeCreateRequest);

        return toDoLocalChangeInsertResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public bool DeleteById(int id)
    {
        if (id <= 0) return false;

        return _toDoLocalChangesRepository.DeleteById(id);
    }

    public bool DeleteRangeByIds(IEnumerable<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _toDoLocalChangesRepository.DeleteRangeByIds(ids);
    }

    public bool DeleteAllByTableNameAndElementId(string tableName, int elementId)
    {
        return _toDoLocalChangesRepository.DeleteByTableNameAndElementId(tableName, elementId);
    }

    public bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds)
    {
        return _toDoLocalChangesRepository.DeleteRangeByTableNameAndElementIds(tableName, elementIds);
    }
}