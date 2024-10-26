using MOSTComputers.Services.TransactionalFileManagement.Contracts;

namespace MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
public interface IOperationRollbackDataManager<T>
{
    IReadOnlyDictionary<IRollbackable, T> RollbackData { get; }

    T? TryGetRollbackData(IRollbackableOperation rollbackableOperation);
    T? TryGetRollbackData<U>(IRollbackableOperation<U> rollbackableOperation);
    bool TryAddRollbackData(IRollbackableOperation rollbackableOperation, T data);
    bool TryAddRollbackData<U>(IRollbackableOperation<U> rollbackableOperation, T data);
    bool TryRemoveRollbackData(IRollbackableOperation rollbackableOperation);
    bool TryRemoveRollbackData<U>(IRollbackableOperation<U> rollbackableOperation);
    T? TryGetRollbackData(IRollbackableOperationAsync rollbackableOperation);
    T? TryGetRollbackData<U>(IRollbackableOperationAsync<U> rollbackableOperation);
    bool TryAddRollbackData(IRollbackableOperationAsync rollbackableOperation, T data);
    bool TryAddRollbackData<U>(IRollbackableOperationAsync<U> rollbackableOperation, T data);
    bool TryRemoveRollbackData<U>(IRollbackableOperationAsync<U> rollbackableOperation);
    bool TryRemoveRollbackData(IRollbackableOperationAsync rollbackableOperation);
}