using System.Data.SqlClient;
using System.Data;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;

namespace MOSTComputers.Services.DAL.DAL;

public interface IRelationalDataAccess
{
    bool AddCustomEntityMap<T>(EntityMap<T> entityMap) where T : class;
    IEnumerable<T> GetData<T, U>(string sqlStatement, U parameters);
    IEnumerable<T1> GetData<T1, T2, U>(string sqlStatement, Func<T1, T2, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, U>(string sqlStatement, Func<T1, T2, T3, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, U>(string sqlStatement, Func<T1, T2, T3, T4, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, T5, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, T5, T6, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetData<T1, T2, T3, T4, T5, T6, T7, U>(string sqlStatement, Func<T1, T2, T3, T4, T5, T6, T7, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    void SaveData<T, U>(string sqlStatement, U parameters);
    IEnumerable<T> GetDataStoredProcedure<T, U>(string storedProcedureName, U parameters);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, U>(string storedProcedureName, Func<T1, T2, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, U>(string storedProcedureName, Func<T1, T2, T3, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, U>(string storedProcedureName, Func<T1, T2, T3, T4, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, T6, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T6, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    IEnumerable<T1> GetDataStoredProcedure<T1, T2, T3, T4, T5, T6, T7, U>(string storedProcedureName, Func<T1, T2, T3, T4, T5, T6, T7, T1> map, string splitOn, U parameters, bool buffered = true, int? commandTimeout = null);
    void SaveDataStoredProcedure<T, U>(string storedProcedureName, U parameters);
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

    public IEnumerable<T> GetData<T, U>(string sqlStatement, U parameters)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        return connection.Query<T>(sqlStatement, parameters, commandType: CommandType.Text);
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

    public void SaveData<T, U>(string sqlStatement, U parameters)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Execute(sqlStatement, parameters, commandType: CommandType.Text);
    }

    public IEnumerable<T> GetDataStoredProcedure<T, U>(string storedProcedureName, U parameters)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

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

    public void SaveDataStoredProcedure<T, U>(string storedProcedureName, U parameters)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);

        connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
    }
}
