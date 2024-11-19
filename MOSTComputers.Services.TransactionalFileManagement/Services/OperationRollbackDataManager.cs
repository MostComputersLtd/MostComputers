using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using System.Collections.Concurrent;

namespace MOSTComputers.Services.TransactionalFileManagement.Services;

public class OperationRollbackDataManager<T> : IOperationRollbackDataManager<T>
{
    private readonly ConcurrentDictionary<IRollbackable, T> _rollbackData = new();

    public IReadOnlyDictionary<IRollbackable, T> RollbackData => _rollbackData;

    public T? TryGetRollbackData(IRollbackableOperation rollbackableOperation)
    {
        bool success = _rollbackData.TryGetValue(rollbackableOperation, out T? result);

        return success ? result : default;
    }

    public T? TryGetRollbackData(IRollbackableOperationAsync rollbackableOperation)
    {
        bool success = _rollbackData.TryGetValue(rollbackableOperation, out T? result);

        return success ? result : default;
    }

    public T? TryGetRollbackData<U>(IRollbackableOperation<U> rollbackableOperation)
    {
        bool success = _rollbackData.TryGetValue(rollbackableOperation, out T? result);

        return success ? result : default;
    }

    public T? TryGetRollbackData<U>(IRollbackableOperationAsync<U> rollbackableOperation)
    {
        bool success = _rollbackData.TryGetValue(rollbackableOperation, out T? result);

        return success ? result : default;
    }

    public bool TryAddRollbackData(IRollbackableOperation rollbackableOperation, T data)
    {
        return _rollbackData.TryAdd(rollbackableOperation, data);
    }

    public bool TryAddRollbackData(IRollbackableOperationAsync rollbackableOperation, T data)
    {
        return _rollbackData.TryAdd(rollbackableOperation, data);
    }

    public bool TryAddRollbackData<U>(IRollbackableOperation<U> rollbackableOperation, T data)
    {
        return _rollbackData.TryAdd(rollbackableOperation, data);
    }

    public bool TryAddRollbackData<U>(IRollbackableOperationAsync<U> rollbackableOperation, T data)
    {
        return _rollbackData.TryAdd(rollbackableOperation, data);
    }

    public bool TryRemoveRollbackData(IRollbackableOperation rollbackableOperation)
    {
        return TryRemoveRollbackDataInternal(rollbackableOperation);
    }

    public bool TryRemoveRollbackData(IRollbackableOperationAsync rollbackableOperation)
    {
        return TryRemoveRollbackDataInternal(rollbackableOperation);
    }

    public bool TryRemoveRollbackData<U>(IRollbackableOperation<U> rollbackableOperation)
    {
        return TryRemoveRollbackDataInternal(rollbackableOperation);
    }

    public bool TryRemoveRollbackData<U>(IRollbackableOperationAsync<U> rollbackableOperation)
    {
        return TryRemoveRollbackDataInternal(rollbackableOperation);
    }

    private bool TryRemoveRollbackDataInternal(IRollbackable rollbackableOperation)
    {
        foreach (KeyValuePair<IRollbackable, T> kvp in _rollbackData)
        {
            if (kvp.Key == rollbackableOperation)
            {
                return _rollbackData.TryRemove(kvp);
            }
        }

        return false;
    }
}