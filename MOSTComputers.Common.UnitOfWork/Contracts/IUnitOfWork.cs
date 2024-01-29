using System.Data;

namespace MOSTComputers.Common.UnitOfWork.Contracts;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }

    void Begin();
    void Commit();
    void Rollback();
}