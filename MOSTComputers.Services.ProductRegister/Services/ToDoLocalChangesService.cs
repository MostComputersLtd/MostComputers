using FluentValidation;
using FluentValidation.Results;
using OneOf;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;

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

    public async Task<List<LocalChangeData>> GetAllAsync()
    {
        return await _toDoLocalChangesRepository.GetAllAsync();
    }

    public async Task<List<LocalChangeData>> GetAllForTableAsync(string tableName)
    {
        return await _toDoLocalChangesRepository.GetAllForTableAsync(tableName);
    }

    public async Task<List<LocalChangeData>> GetAllForOperationTypeAsync(ChangeOperationType operationType)
    {
        return await _toDoLocalChangesRepository.GetAllForOperationTypeAsync(operationType);
    }

    public async Task<LocalChangeData?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _toDoLocalChangesRepository.GetByIdAsync(id);
    }

    public async Task<LocalChangeData?> GetByTableNameAndElementIdAndOperationTypeAsync(string tableName, int elementId, ChangeOperationType changeOperationType)
    {
        return await _toDoLocalChangesRepository.GetByTableNameAndElementIdAndOperationTypeAsync(tableName, elementId, changeOperationType);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceToDoLocalChangeCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest = _productMapper.Map(createRequest);

        toDoLocalChangeCreateRequest.TimeStamp = DateTime.Now;

        OneOf<int, UnexpectedFailureResult> toDoLocalChangeInsertResult = await _toDoLocalChangesRepository.InsertAsync(toDoLocalChangeCreateRequest);

        return toDoLocalChangeInsertResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            id => id,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        if (id <= 0) return false;

        return await _toDoLocalChangesRepository.DeleteByIdAsync(id);
    }

    public async Task<bool> DeleteRangeByIdsAsync(IEnumerable<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return await _toDoLocalChangesRepository.DeleteRangeByIdsAsync(ids);
    }

    public async Task<bool> DeleteAllByTableNameAndElementIdAsync(string tableName, int elementId)
    {
        return await _toDoLocalChangesRepository.DeleteByTableNameAndElementIdAsync(tableName, elementId);
    }

    public async Task<bool> DeleteRangeByTableNameAndElementIdsAsync(string tableName, IEnumerable<int> elementIds)
    {
        return await _toDoLocalChangesRepository.DeleteRangeByTableNameAndElementIdsAsync(tableName, elementIds);
    }
}