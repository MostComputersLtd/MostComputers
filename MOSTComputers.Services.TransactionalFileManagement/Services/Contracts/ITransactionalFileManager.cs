using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using System.Transactions;

namespace MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
public interface ITransactionalFileManager
{
    void EnlistOrExecuteOperation(IRollbackableOperation rollbackableOperation);
    void EnlistOrExecuteOperation(IRollbackableOperation rollbackableOperation, Transaction? transaction);
    TOutput EnlistOrExecuteOperation<TOutput>(IRollbackableOperation<TOutput> rollbackableOperation);
    TOutput EnlistOrExecuteOperation<TOutput>(IRollbackableOperation<TOutput> rollbackableOperation, Transaction? transaction);
    Task EnlistOrExecuteOperationAsync(IRollbackableOperationAsync rollbackableOperation);
    Task EnlistOrExecuteOperationAsync(IRollbackableOperationAsync rollbackableOperation, Transaction? transaction);
    Task<TOutput> EnlistOrExecuteOperationAsync<TOutput>(IRollbackableOperationAsync<TOutput> rollbackableOperation);
    Task<TOutput> EnlistOrExecuteOperationAsync<TOutput>(IRollbackableOperationAsync<TOutput> rollbackableOperation, Transaction? transaction);
}