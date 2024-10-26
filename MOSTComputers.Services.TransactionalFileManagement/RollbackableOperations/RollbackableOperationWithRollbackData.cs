using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.TransactionalFileManagement.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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