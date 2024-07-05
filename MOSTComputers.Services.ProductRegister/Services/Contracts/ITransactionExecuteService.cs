
namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface ITransactionExecuteService
{
    void ExecuteActionInTransaction(Action action);
    void ExecuteActionInTransaction<T>(Action<T> action, T parameter);
    TReturn ExecuteActionInTransaction<TReturn>(Func<TReturn> action);
    TReturn ExecuteActionInTransaction<T, TReturn>(Func<T, TReturn> action, T parameter);
    TReturn ExecuteActionInTransactionAndCommitWithCondition<TReturn>(Func<TReturn> action, Predicate<TReturn> shouldCommit);
    TReturn ExecuteActionInTransactionAndCommitWithCondition<T, TReturn>(Func<T, TReturn> action, Predicate<TReturn> shouldCommit, T parameter);
    Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<TReturn>(Func<Task<TReturn>> action, Predicate<TReturn> shouldCommit);
    Task<TReturn> ExecuteActionInTransactionAndCommitWithConditionAsync<T, TReturn>(Func<T, Task<TReturn>> action, Predicate<TReturn> shouldCommit, T parameter);
}