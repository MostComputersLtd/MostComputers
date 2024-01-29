namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface ITransactionExecuteService
{
    void ExecuteActionInTransaction(Action action);
    void ExecuteActionInTransaction<T>(Action<T> action, T parameter);
    TReturn ExecuteActionInTransaction<TReturn>(Func<TReturn> action);
    TReturn ExecuteActionInTransaction<T, TReturn>(Func<T, TReturn> action, T parameter);
}