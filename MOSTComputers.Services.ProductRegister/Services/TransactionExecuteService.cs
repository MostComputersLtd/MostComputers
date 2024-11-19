using MOSTComputers.Services.DAL.DAL;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using System.Transactions;

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

    public void ExecuteActionInTransaction(Action action,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption) : new(transactionScopeOption, transactionOptions.Value);

        action();

        scope.Complete();
    }

    public void ExecuteActionInTransaction<T>(Action<T> action, T parameter)
    {
        _relationalDataAccess.SaveDataInTransactionScopeUsingAction(action, parameter);
    }

    public void ExecuteActionInTransaction<T>(Action<T> action, T parameter,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption) : new(transactionScopeOption, transactionOptions.Value);

        action(parameter);

        scope.Complete();
    }

    public TReturn ExecuteActionInTransaction<TReturn>(Func<TReturn> action)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingAction(action);
    }

    public TReturn ExecuteActionInTransaction<TReturn>(Func<TReturn> action,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption) : new(transactionScopeOption, transactionOptions.Value);

        TReturn data = action();

        scope.Complete();

        return data;
    }

    public TReturn ExecuteActionInTransaction<T, TReturn>(Func<T, TReturn> action, T parameter)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingAction(action, parameter);
    }

    public TReturn ExecuteActionInTransaction<T, TReturn>(Func<T, TReturn> action, T parameter,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption) : new(transactionScopeOption, transactionOptions.Value);

        TReturn data = action(parameter);

        scope.Complete();

        return data;
    }

    public TReturn ExecuteActionInTransactionAndCommitWithCondition<TReturn>(Func<TReturn> action, Predicate<TReturn> shouldCommit)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition(action, shouldCommit);
    }

    public TReturn ExecuteActionInTransactionAndCommitWithCondition<TReturn>(Func<TReturn> action, Predicate<TReturn> shouldCommit,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption) : new(transactionScopeOption, transactionOptions.Value);

        TReturn data = action();

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
    }

    public TReturn ExecuteActionInTransactionAndCommitWithCondition<T, TReturn>(Func<T, TReturn> action, Predicate<TReturn> shouldCommit, T parameter)
    {
        return _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition(action, shouldCommit, parameter);
    }

    public TReturn ExecuteActionInTransactionAndCommitWithCondition<T, TReturn>(Func<T, TReturn> action,
        Predicate<TReturn> shouldCommit,
        T parameter,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption) : new(transactionScopeOption, transactionOptions.Value);

        TReturn data = action(parameter);

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
    }

    public async Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<TReturn>(Func<Task<TReturn>> action, Predicate<TReturn> shouldCommit)
    {
        return await _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnConditionAsync(action, shouldCommit);
    }

    public async Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<TReturn>(Func<Task<TReturn>> action,
        Predicate<TReturn> shouldCommit,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled)
            : new(transactionScopeOption, transactionOptions.Value, TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action();

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
    }

    public async Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<T, TReturn>(
        Func<T, Task<TReturn>> action, Predicate<TReturn> shouldCommit, T parameter)
    {
        return await _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnConditionAsync(action, shouldCommit, parameter);
    }

    public async Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<T, TReturn>(
        Func<T, Task<TReturn>> action,
        Predicate<TReturn> shouldCommit,
        T parameter,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null) ? new(transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled)
            : new(transactionScopeOption, transactionOptions.Value, TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action(parameter);

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
    }
}