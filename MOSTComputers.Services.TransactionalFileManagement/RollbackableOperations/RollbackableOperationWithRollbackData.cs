using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;

namespace MOSTComputers.Services.TransactionalFileManagement.RollbackableOperations;

public abstract class RollbackableOperationWithRollbackData<TOutput, TRollbackData>
    : IRollbackableOperation<TOutput>
{
    public RollbackableOperationWithRollbackData(
        IOperationRollbackDataManager<TRollbackData> operationRollbackDataManager)
    {
        this.operationRollbackDataManager = operationRollbackDataManager;
    }

    protected readonly IOperationRollbackDataManager<TRollbackData> operationRollbackDataManager;

    public abstract TOutput Execute();

    public abstract void Rollback();
}