using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOSTComputers.Common.UnitOfWork.Contracts;

namespace MOSTComputers.Common.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    private IDbConnection _connection;

    private IDbTransaction? _transaction;

    public IDbConnection Connection
    {
        get 
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(ToString());
            }

            return _connection;
        }
    }

    public IDbTransaction? Transaction
    {
        get
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(ToString());
            }

            return _transaction;
        }
    }

    public void Begin()
    {
        if (_disposedValue)
        {
            throw new ObjectDisposedException(ToString());
        }

        _transaction = _connection.BeginTransaction();
    }

    public void Rollback()
    {
        if (_disposedValue)
        {
            throw new ObjectDisposedException(ToString());
        }

        if (_transaction is null)
        {
            throw new InvalidOperationException("Cannot use Rollback() before using the Begin() method to start a transaction");
        }

        _transaction.Rollback();
        Dispose();
    }

    public void Commit()
    {
        if (_disposedValue)
        {
            throw new ObjectDisposedException(ToString());
        }

        if (_transaction is null)
        {
            throw new InvalidOperationException("Cannot use Commit() before using the Begin() method to start a transaction");
        }

        _transaction.Commit();
        Dispose();
    }

#region Dispose pattern

    private bool _disposedValue;

    private void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        
        if (disposing)
        {
            if (_transaction is not null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        _connection = null!;

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }

#endregion Dispose pattern
}