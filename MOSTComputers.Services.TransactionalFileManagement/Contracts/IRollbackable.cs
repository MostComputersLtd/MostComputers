namespace MOSTComputers.Services.TransactionalFileManagement.Contracts;

public interface IRollbackable
{
    void Rollback();
}