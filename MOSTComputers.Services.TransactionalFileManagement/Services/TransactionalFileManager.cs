using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using System.Transactions;

namespace MOSTComputers.Services.TransactionalFileManagement.Services;
public sealed class TransactionalFileManager : ITransactionalFileManager
{
    public TransactionalFileManager(IEnlistmentManager enlistmentManager)
    {
        _enlistmentManager = enlistmentManager;
    }

    private readonly IEnlistmentManager _enlistmentManager;

    public void EnlistOrExecuteOperation(IRollbackableOperation rollbackableOperation)
    {
        EnlistOrExecuteOperation(rollbackableOperation, Transaction.Current);
    }

    public void EnlistOrExecuteOperation(IRollbackableOperation rollbackableOperation, Transaction? transaction)
    {
        if (transaction is not null)
        {
            EnlistOperationToTransaction(transaction, rollbackableOperation);

            return;
        }

        rollbackableOperation.Execute();
    }

    public async Task EnlistOrExecuteOperationAsync(IRollbackableOperationAsync rollbackableOperation)
    {
        await EnlistOrExecuteOperationAsync(rollbackableOperation, Transaction.Current);
    }

    public async Task EnlistOrExecuteOperationAsync(IRollbackableOperationAsync rollbackableOperation, Transaction? transaction)
    {
        if (transaction is not null)
        {
            await EnlistOperationToTransactionAsync(transaction, rollbackableOperation);

            return;
        }

        await rollbackableOperation.ExecuteAsync();
    }

    public TOutput EnlistOrExecuteOperation<TOutput>(IRollbackableOperation<TOutput> rollbackableOperation)
    {
        return EnlistOrExecuteOperation(rollbackableOperation, Transaction.Current);
    }

    public TOutput EnlistOrExecuteOperation<TOutput>(IRollbackableOperation<TOutput> rollbackableOperation, Transaction? transaction)
    {
        if (transaction is not null)
        {
            (Guid _, TOutput output)
                = EnlistOperationToTransaction(transaction, rollbackableOperation);

            return output;
        }

        return rollbackableOperation.Execute();
    }

    public async Task<TOutput> EnlistOrExecuteOperationAsync<TOutput>(IRollbackableOperationAsync<TOutput> rollbackableOperation)
    {
        return await EnlistOrExecuteOperationAsync(rollbackableOperation, Transaction.Current);
    }

    public async Task<TOutput> EnlistOrExecuteOperationAsync<TOutput>(IRollbackableOperationAsync<TOutput> rollbackableOperation, Transaction? transaction)
    {
        if (transaction is not null)
        {
            (Guid _, TOutput output)
                = await EnlistOperationToTransactionAsync(transaction, rollbackableOperation);

            return output;
        }

        return await rollbackableOperation.ExecuteAsync();
    }

    private Guid EnlistOperationToTransaction(Transaction transaction, IRollbackableOperation rollbackableOperation)
    {
        (Guid enlistmentId, TransactionalFileManagementEnlistmentNotification enlistmentNotification)
            = GetOrCreateAndEnlistNotificationForTransaction(transaction);

        enlistmentNotification.EnlistOperation(rollbackableOperation);

        return enlistmentId;
    }

    private async Task<Guid> EnlistOperationToTransactionAsync(Transaction transaction, IRollbackableOperationAsync rollbackableOperation)
    {
        (Guid enlistmentId, TransactionalFileManagementEnlistmentNotification enlistmentNotification)
            = GetOrCreateAndEnlistNotificationForTransaction(transaction);

        await enlistmentNotification.EnlistOperationAsync(rollbackableOperation);

        return enlistmentId;
    }

    private (Guid enlistmentId, TOutput output) EnlistOperationToTransaction<TOutput>(
        Transaction transaction, IRollbackableOperation<TOutput> rollbackableOperation)
    {
        (Guid enlistmentId, TransactionalFileManagementEnlistmentNotification enlistmentNotification)
            = GetOrCreateAndEnlistNotificationForTransaction(transaction);

        TOutput output = enlistmentNotification.EnlistOperation(rollbackableOperation);

        return (enlistmentId, output);
    }

    private async Task<(Guid enlistmentId, TOutput output)> EnlistOperationToTransactionAsync<TOutput>(
        Transaction transaction, IRollbackableOperationAsync<TOutput> rollbackableOperation)
    {
        (Guid enlistmentId, TransactionalFileManagementEnlistmentNotification enlistmentNotification)
            = GetOrCreateAndEnlistNotificationForTransaction(transaction);

        TOutput output = await enlistmentNotification.EnlistOperationAsync(rollbackableOperation);

        return (enlistmentId, output);
    }

    private (Guid enlistmentId, TransactionalFileManagementEnlistmentNotification enlistmentNotification) GetOrCreateAndEnlistNotificationForTransaction(
        Transaction transaction)
    {
        foreach (KeyValuePair<Guid, TransactionalFileManagementEnlistmentNotification> kvp in _enlistmentManager.EnlistmentNotificationDict)
        {
            if (kvp.Value.TransactionId == transaction.TransactionInformation.LocalIdentifier)
            {
                return (kvp.Key, kvp.Value);
            }
        }

        TransactionalFileManagementEnlistmentNotification enlistmentNotification
            = new(transaction.TransactionInformation.LocalIdentifier, _enlistmentManager);

        transaction.EnlistVolatile(enlistmentNotification, EnlistmentOptions.None);

        Guid enlistmentId = _enlistmentManager.AddNotification(enlistmentNotification);

        return (enlistmentId, enlistmentNotification);
    }
}