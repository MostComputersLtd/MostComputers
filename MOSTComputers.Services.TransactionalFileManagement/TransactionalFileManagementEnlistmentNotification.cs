using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MOSTComputers.Services.TransactionalFileManagement;

public class TransactionalFileManagementEnlistmentNotification : IEnlistmentNotification
{
    internal TransactionalFileManagementEnlistmentNotification(string transactionId, IEnlistmentManager enlistmentManager)
    {
        TransactionId = transactionId;
        _enlistmentManager = enlistmentManager;
    }

    internal string TransactionId { get; init; }
    private readonly IEnlistmentManager _enlistmentManager;
    private readonly List<IRollbackable> _rollbackableOperations = new();

    public virtual void EnlistOperation(IRollbackableOperation rollbackableOperation)
    {
        _rollbackableOperations.Add(rollbackableOperation);

        rollbackableOperation.Execute();
    }

    public virtual async Task EnlistOperationAsync(IRollbackableOperationAsync rollbackableOperation)
    {
        _rollbackableOperations.Add(rollbackableOperation);

        await rollbackableOperation.ExecuteAsync();
    }

    public virtual T EnlistOperation<T>(IRollbackableOperation<T> rollbackableOperation)
    {
        _rollbackableOperations.Add(rollbackableOperation);

        return rollbackableOperation.Execute();
    }

    public virtual async Task<T> EnlistOperationAsync<T>(IRollbackableOperationAsync<T> rollbackableOperation)
    {
        _rollbackableOperations.Add(rollbackableOperation);

        return await rollbackableOperation.ExecuteAsync();
    }

    public virtual void Prepare(PreparingEnlistment preparingEnlistment)
    {
        preparingEnlistment.Prepared();
    }

    public virtual void Commit(Enlistment enlistment)
    {
        enlistment.Done();

        _enlistmentManager.TryRemoveNotification(this);
    }

    public virtual void InDoubt(Enlistment enlistment)
    {
        Rollback(enlistment);
    }

    public virtual void Rollback(Enlistment enlistment)
    {
        try
        {
            for (int i = _rollbackableOperations.Count - 1; i >= 0; i--)
            {
                IRollbackable operation = _rollbackableOperations[i];

                operation.Rollback();
            }
        }
        catch (Exception ex)
        {
            throw new TransactionException("Failed to roll back.", ex);
        }

        _enlistmentManager.TryRemoveNotification(this);
    }
}