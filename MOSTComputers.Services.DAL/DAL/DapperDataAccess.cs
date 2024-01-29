using System.Data.SqlClient;
using System.Data;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using System.Transactions;
using System.Data.Common;

namespace MOSTComputers.Services.DAL.DAL;

public interface IRelationalDataAccess
{
    bool AddCustomEntityMap<T>(EntityMap<T> entityMap) where T : class;
    IEnumerable<T> GetData<T, U>(string sqlStatement, U parameters, bool doInTransaction = false);
    IEnumerable<T1> GetData<T1, T2, U>(string sqlStatement, Func<T1, T2, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, U>(string sqlStatement, Func<T1, T2, T3, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, U>(string sqlStatement, Func<T1, T2, T3, T4, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, T5, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, T5, T6, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, T5, T6, T7, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T6, T7, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    T? GetDataFirstOrDefault<T, U>(string sqlStatement, U parameters, bool doInTransaction = false);
    T? GetDataFirstOrDefault<T, U>(string sqlStatement, U parameters, IDbConnection dbConnection, IDbTransaction transaction);
    T GetDataFirst<T, U>(string sqlStatement, U parameters, bool doInTransaction = false);
    T GetDataFirst<T, U>(string sqlStatement, U parameters, IDbConnection dbConnection, IDbTransaction transaction);
    T GetDataSingle<T, U>(string sqlStatement, U parameters, bool doInTransaction = false);
    T GetDataSingle<T, U>(string sqlStatement, U parameters, IDbConnection dbConnection, IDbTransaction transaction);
    int SaveData<T, U>(string sqlStatement, U parameters, bool doInTransaction = false);
    void SaveDataInTransactionUsingAction<T, U>(Action<IDbConnection, IDbTransaction, U> actionInTransaction, U parameters);
    TReturn SaveDataInTransactionUsingAction<T, U, TReturn>(Func<IDbConnection, IDbTransaction, U, TReturn> actionInTransaction, U parameters);
    IEnumerable<T> GetDataStoredProcedure<T, U>(string storedProcedureName, U parameters, bool doInTransaction = false);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, U>(string storedProcedureName, Func<T1, T2, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, U>(string storedProcedureName, Func<T1, T2, T3, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, U>(string storedProcedureName, Func<T1, T2, T3, T4, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, T6, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, T6, T7, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T6, T7, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    int SaveDataStoredProcedure<T, U>(string storedProcedureName, U parameters, bool doInTransaction = false);
    T? SaveDataAndReturnValue<T, U>(string sqlStatement, U parameters, bool doInTransaction = false);
    T? SaveDataAndReturnValueStoredProcedure<T, U>(string storedProcedureName, U parameters, bool doInTransaction = false);
    T GetDataFirst<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction);
    T GetDataSingle<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction);
    T? GetDataFirstOrDefault<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction);
    IEnumerable<T> GetData<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction);
    void SaveDataInTransactionScopeUsingAction(Action actionInTransaction);
    void SaveDataInTransactionScopeUsingAction<U>(Action<U> actionInTransaction, U parameters);
    void SaveDataInTransactionScopeUsingAction<U>(Action<IDbConnection, U> actionInTransaction, U parameters);
    TReturn SaveDataInTransactionScopeUsingAction<U, TReturn>(Func<IDbConnection, U, TReturn> actionInTransaction, U parameters);
    TReturn SaveDataInTransactionScopeUsingAction<U, TReturn>(Func<U, TReturn> actionInTransaction, U parameters);
    TReturn SaveDataInTransactionScopeUsingAction<TReturn>(Func<TReturn> actionInTransaction);
}

internal class DapperDataAccess : IRelationalDataAccess
{
    private readonly string _connectionString;

    public DapperDataAccess(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool AddCustomEntityMap<T>(EntityMap<T> entityMap)
        where T : class
    {
        FluentMapper.EntityMaps.AddOrUpdate(typeof(T), x => entityMap, (type, entityMapToUpdate) => entityMapToUpdate = entityMap);

        return true;
    }

    public IEnumerable<T> GetData<T, U>(string sqlStatement, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            return connection.Query<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
        }

        return connection.Query<T>(sqlStatement, parameters, commandType: CommandType.Text);
    }

    public IEnumerable<T> GetData<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        return connection.Query<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
    }


    public IEnumerable<T1> GetData<T1, T2, U>(string sqlStatement, Func<T1, T2, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(sqlStatement, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.Text);
    }

    public IEnumerable<T1> GetData<T1, T2, T3, U>(string sqlStatement, Func<T1, T2, T3, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(sqlStatement, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.Text);
    }

    public IEnumerable<T1> GetData<T1, T2, T3, T4, U>(string sqlStatement, Func<T1, T2, T3, T4, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(sqlStatement, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.Text);
    }

    public IEnumerable<T1> GetData<T1, T2, T3, T4, T5, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(sqlStatement, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.Text);
    }

    public IEnumerable<T1> GetData<T1, T2, T3, T4, T5, T6, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(sqlStatement, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.Text);
    }

    public IEnumerable<T1> GetData<T1, T2, T3, T4, T5, T6, T7, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T6, T7, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(sqlStatement, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.Text);
    }

    public T? GetDataFirstOrDefault<T, U>(string sqlStatement, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            return connection.QueryFirstOrDefault<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
        }

        return connection.QueryFirstOrDefault<T>(sqlStatement, parameters, commandType: CommandType.Text);
    }

    public T GetDataFirst<T, U>(string sqlStatement, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            return connection.QueryFirst<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
        }

        return connection.QueryFirst<T>(sqlStatement, parameters, commandType: CommandType.Text);
    }

    public T GetDataSingle<T, U>(string sqlStatement, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            return connection.QuerySingle<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
        }

        return connection.QuerySingle<T>(sqlStatement, parameters, commandType: CommandType.Text);
    }

    public T? GetDataFirstOrDefault<T, U>(string sqlStatement, U parameters, IDbConnection dbConnection, IDbTransaction transaction)
    {
        return dbConnection.QueryFirstOrDefault<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
    }

    public T GetDataFirst<T, U>(string sqlStatement, U parameters, IDbConnection dbConnection, IDbTransaction transaction)
    {
        return dbConnection.QueryFirst<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
    }

    public T GetDataSingle<T, U>(string sqlStatement, U parameters, IDbConnection dbConnection, IDbTransaction transaction)
    {
        return dbConnection.QuerySingle<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
    }

    public T? GetDataFirstOrDefault<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);

        return dbConnection.QueryFirstOrDefault<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
    }

    public T GetDataFirst<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);

        return dbConnection.QueryFirst<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
    }

    public T GetDataSingle<T, U>(string sqlStatement, U parameters, IDbTransaction? transaction)
    {
        using IDbConnection dbConnection = new SqlConnection(_connectionString);

        return dbConnection.QuerySingle<T>(sqlStatement, parameters, transaction, commandType: CommandType.Text);
    }

    public int SaveData<T, U>(string sqlStatement, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            int rowsAffected = connection.Execute(sqlStatement, parameters, transaction, commandType: CommandType.Text);

            transaction.Commit();

            return rowsAffected;
        }

        return connection.Execute(sqlStatement, parameters, commandType: CommandType.Text);
    }

    public T? SaveDataAndReturnValue<T, U>(string sqlStatement, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            T? data = (T?)connection.ExecuteScalar(sqlStatement, parameters, transaction, commandType: CommandType.Text);

            transaction.Commit();

            return data;
        }

        return (T?)connection.ExecuteScalar(sqlStatement, parameters, commandType: CommandType.Text);
    }

    public void SaveDataInTransactionUsingAction<T, U>(Action<IDbConnection, IDbTransaction, U> actionInTransaction, U parameters)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        actionInTransaction(connection, transaction, parameters);

        transaction.Commit();
    }

    public TReturn SaveDataInTransactionUsingAction<T, U, TReturn>(Func<IDbConnection, IDbTransaction, U, TReturn> actionInTransaction, U parameters)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        TReturn data = actionInTransaction(connection, transaction, parameters);

        transaction.Commit();

        return data;
    }

    public void SaveDataInTransactionScopeUsingAction(Action actionInTransaction)
    {
        using TransactionScope scope = new();

        actionInTransaction();

        scope.Complete();
    }

    public void SaveDataInTransactionScopeUsingAction<U>(Action<U> actionInTransaction, U parameter)
    {
        using TransactionScope scope = new();

        actionInTransaction(parameter);

        scope.Complete();
    }

    public void SaveDataInTransactionScopeUsingAction<U>(Action<IDbConnection, U> actionInTransaction, U parameters)
    {
        using TransactionScope scope = new();

        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        actionInTransaction(connection, parameters);

        scope.Complete();
    }

    public TReturn SaveDataInTransactionScopeUsingAction<TReturn>(Func<TReturn> actionInTransaction)
    {
        using TransactionScope scope = new();

        TReturn data = actionInTransaction();

        scope.Complete();

        return data;
    }

    public TReturn SaveDataInTransactionScopeUsingAction<U, TReturn>(Func<U, TReturn> actionInTransaction, U parameters)
    {
        using TransactionScope scope = new();

        TReturn data = actionInTransaction(parameters);

        scope.Complete();

        return data;
    }

    public TReturn SaveDataInTransactionScopeUsingAction<U, TReturn>(Func<IDbConnection, U, TReturn> actionInTransaction, U parameters)
    {
        using TransactionScope scope = new();

        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        TReturn data = actionInTransaction(connection, parameters);

        scope.Complete();

        return data;
    }

    public IEnumerable<T> GetDataStoredProcedure<T, U>(string storedProcedureName, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            return connection.Query<T>(storedProcedureName, parameters, transaction, commandType: CommandType.StoredProcedure);
        }

        return connection.Query<T>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T1> GetDataStoredProcedure<T1, T2, U>(string storedProcedureName, Func<T1, T2, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(storedProcedureName, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, U>(string storedProcedureName, Func<T1, T2, T3, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(storedProcedureName, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, U>(string storedProcedureName, Func<T1, T2, T3, T4, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(storedProcedureName, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(storedProcedureName, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, T6, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(storedProcedureName, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.StoredProcedure);
    }

    public IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, T6, T7, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T6, T7, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Open();

        using IDbTransaction transaction = connection.BeginTransaction();

        return connection.Query(storedProcedureName, map, parameters, transaction, buffered, splitOn, commandTimeout, commandType: CommandType.StoredProcedure);
    }

    public int SaveDataStoredProcedure<T, U>(string storedProcedureName, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            int rowsAffected = connection.Execute(storedProcedureName, parameters, transaction, commandType: CommandType.StoredProcedure);

            transaction.Commit();

            return rowsAffected;
        }

        return connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
    }

    public T? SaveDataAndReturnValueStoredProcedure<T, U>(string storedProcedureName, U parameters, bool doInTransaction = false)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        if (doInTransaction)
        {
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction();

            T? data = (T?)connection.ExecuteScalar(storedProcedureName, parameters, transaction, commandType: CommandType.StoredProcedure);

            transaction.Commit();

            return data;
        }

        return (T?)connection.ExecuteScalar(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
    }
}