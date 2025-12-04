using MOSTComputers.Services.ProductRegister.Services.Contracts;
using System.Transactions;

namespace MOSTComputers.Services.ProductRegister.Services;
internal sealed class TransactionExecuteService : ITransactionExecuteService
{
    public void ExecuteActionInTransaction(Action action)
    {
        using TransactionScope scope = new();

        action();

        scope.Complete();
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
        using TransactionScope scope = new();

        action(parameter);

        scope.Complete();
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
        using TransactionScope scope = new();

        TReturn data = action();

        scope.Complete();

        return data;
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
        using TransactionScope scope = new();

        TReturn data = action(parameter);

        scope.Complete();

        return data;
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

    public async Task ExecuteActionInTransactionAsync(Func<Task> action)
    {
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

        await action();

        scope.Complete();
    }

    public async Task<TReturn> ExecuteActionInTransactionAsync<TReturn>(Func<Task<TReturn>> action)
    {
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action();

        scope.Complete();

        return data;
    }

    public async Task<TReturn> ExecuteActionInTransactionAsync<TReturn>(Func<Task<TReturn>> action,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null)
            ? new(transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled)
            : new(transactionScopeOption, transactionOptions.Value, TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action();

        scope.Complete();

        return data;
    }

    public async Task<TReturn> ExecuteActionInTransactionAsync<T, TReturn>(Func<T, Task<TReturn>> action, T parameter)
    {
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action(parameter);

        scope.Complete();

        return data;
    }

    public async Task<TReturn> ExecuteActionInTransactionAsync<T, TReturn>(Func<T, Task<TReturn>> action, T parameter,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null)
            ? new(transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled)
            : new(transactionScopeOption, transactionOptions.Value, TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action(parameter);

        scope.Complete();

        return data;
    }

    public TReturn ExecuteActionInTransactionAndCommitWithCondition<TReturn>(Func<TReturn> action, Predicate<TReturn> shouldCommit)
    {
        using TransactionScope scope = new();

        TReturn data = action();

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
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
        using TransactionScope scope = new();

        TReturn data = action(parameter);

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
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
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action();

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
    }

    public async Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<TReturn>(Func<Task<TReturn>> action,
        Predicate<TReturn> shouldCommit,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null)
            ? new(transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled)
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
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action(parameter);

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
    }

    public async Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<T, TReturn>(
        Func<T, Task<TReturn>> action,
        Predicate<TReturn> shouldCommit,
        T parameter,
        TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
        TransactionOptions? transactionOptions = null)
    {
        using TransactionScope scope = (transactionOptions is null)
            ? new(transactionScopeOption, TransactionScopeAsyncFlowOption.Enabled)
            : new(transactionScopeOption, transactionOptions.Value, TransactionScopeAsyncFlowOption.Enabled);

        TReturn data = await action(parameter);

        if (shouldCommit(data))
        {
            scope.Complete();
        }

        return data;
    }
}