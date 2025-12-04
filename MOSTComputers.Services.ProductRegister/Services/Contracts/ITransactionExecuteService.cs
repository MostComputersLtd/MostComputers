using System.Transactions;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface ITransactionExecuteService
{
    void ExecuteActionInTransaction(Action action);
    void ExecuteActionInTransaction(Action action, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    TReturn ExecuteActionInTransaction<T, TReturn>(Func<T, TReturn> action, T parameter);
    TReturn ExecuteActionInTransaction<T, TReturn>(Func<T, TReturn> action, T parameter, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    void ExecuteActionInTransaction<T>(Action<T> action, T parameter);
    void ExecuteActionInTransaction<T>(Action<T> action, T parameter, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    TReturn ExecuteActionInTransaction<TReturn>(Func<TReturn> action);
    TReturn ExecuteActionInTransaction<TReturn>(Func<TReturn> action, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    TReturn ExecuteActionInTransactionAndCommitWithCondition<T, TReturn>(Func<T, TReturn> action, Predicate<TReturn> shouldCommit, T parameter);
    TReturn ExecuteActionInTransactionAndCommitWithCondition<T, TReturn>(Func<T, TReturn> action, Predicate<TReturn> shouldCommit, T parameter, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    TReturn ExecuteActionInTransactionAndCommitWithCondition<TReturn>(Func<TReturn> action, Predicate<TReturn> shouldCommit);
    TReturn ExecuteActionInTransactionAndCommitWithCondition<TReturn>(Func<TReturn> action, Predicate<TReturn> shouldCommit, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<T, TReturn>(Func<T, Task<TReturn>> action, Predicate<TReturn> shouldCommit, T parameter);
    Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<T, TReturn>(Func<T, Task<TReturn>> action, Predicate<TReturn> shouldCommit, T parameter, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<TReturn>(Func<Task<TReturn>> action, Predicate<TReturn> shouldCommit);
    Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<TReturn>(Func<Task<TReturn>> action, Predicate<TReturn> shouldCommit, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    Task<TReturn> ExecuteActionInTransactionAsync<TReturn>(Func<Task<TReturn>> action);
    Task<TReturn> ExecuteActionInTransactionAsync<TReturn>(Func<Task<TReturn>> action, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    Task<TReturn> ExecuteActionInTransactionAsync<T, TReturn>(Func<T, Task<TReturn>> action, T parameter);
    Task<TReturn> ExecuteActionInTransactionAsync<T, TReturn>(Func<T, Task<TReturn>> action, T parameter, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TransactionOptions? transactionOptions = null);
    Task ExecuteActionInTransactionAsync(Func<Task> action);
}