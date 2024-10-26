namespace MOSTComputers.Services.TransactionalFileManagement.Contracts;

public interface IRollbackableOperation : IRollbackable
{
    void Execute();
}

public interface IRollbackableOperation<TOutput> : IRollbackable
{
    TOutput Execute();
}