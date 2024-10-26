namespace MOSTComputers.Services.TransactionalFileManagement.Contracts;

public interface IRollbackableOperationAsync : IRollbackable
{
    Task ExecuteAsync();
}

public interface IRollbackableOperationAsync<TOutput> : IRollbackable
{
    Task<TOutput> ExecuteAsync();
}