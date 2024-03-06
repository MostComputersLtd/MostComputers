using MOSTComputers.Services.DAL.DAL;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class TransactionExecuteService : ITransactionExecuteService
{
    public TransactionExecuteService(IRelationalDataAccess relationalDataAccess)
    {
        _relationalDataAccess = relationalDataAccess;
    }

    private readonly IRelationalDataAccess _relationalDataAccess;

    public void ExecuteActionInTransaction(Action action)
    {
        _relationalDataAccess.SaveDataInTransactionScopeUsingAction(action);
    }

    public void ExecuteActionInTransaction<T>(Action<T> action, T parameter)
    {
        _relationalDataAccess.SaveDataInTransactionScopeUsingAction(action, parameter);
    }

    public TReturn ExecuteActionInTransaction<TReturn>(Func<TReturn> action)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingAction(action);
    }

    public TReturn ExecuteActionInTransaction<T, TReturn>(Func<T, TReturn> action, T parameter)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingAction(action, parameter);
    }

    public TReturn ExecuteActionInTransactionAndCommitWithCondition<TReturn>(Func<TReturn> action, Predicate<TReturn> shouldCommit)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition(action, shouldCommit);
    }

    public TReturn ExecuteActionInTransactionAndCommitWithCondition<T, TReturn>(Func<T, TReturn> action, Predicate<TReturn> shouldCommit, T parameter)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition(action, shouldCommit, parameter);
    }
}