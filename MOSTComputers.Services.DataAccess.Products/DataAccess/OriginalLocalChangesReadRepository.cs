using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class OriginalLocalChangesReadRepository : IOriginalLocalChangesReadRepository
{
    public OriginalLocalChangesReadRepository(
        [FromKeyedServices(ConfigureServices.ReadOnlyDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<LocalChangeData>> GetAllAsync()
    {
        const string getAllQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp 
        FROM {LocalChangesTableName} WITH (NOLOCK)
        ORDER BY TimeStamp;
        """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<LocalChangeData> localChanges = await dbConnection.QueryAsync<LocalChangeData>(
            getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return localChanges.AsList();
    }

    public async Task<List<LocalChangeData>> GetAllForTableAsync(string tableName)
    {
        const string getAllForTableQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {LocalChangesTableName} WITH (NOLOCK)
        WHERE TableName = @tableName
        ORDER BY TimeStamp;
        """;

        var parameters = new
        {
            tableName
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<LocalChangeData> localChanges = await dbConnection.QueryAsync<LocalChangeData>(
            getAllForTableQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return localChanges.AsList();
    }

    public async Task<List<LocalChangeData>> GetAllForOperationTypeAsync(ChangeOperationType changeOperationType)
    {
        const string getAllForOperationTypeQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {LocalChangesTableName} WITH (NOLOCK)
        WHERE Operation = @changeOperationType
        ORDER BY TimeStamp;
        """;

        var parameters = new
        {
            changeOperationType = (int)changeOperationType
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<LocalChangeData> localChanges = await dbConnection.QueryAsync<LocalChangeData>(
            getAllForOperationTypeQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return localChanges.AsList();
    }

    public async Task<List<LocalChangeData>> GetAllForTableNameAndOperationTypeAsync(string tableName, ChangeOperationType changeOperationType)
    {
        const string getAllForOperationTypeQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {LocalChangesTableName} WITH (NOLOCK)
        WHERE Operation = @changeOperationType
        AND TableName = @tableName
        ORDER BY TimeStamp;
        """;

        var parameters = new
        {
            tableName = tableName,
            changeOperationType = (int)changeOperationType,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<LocalChangeData> localChanges = await dbConnection.QueryAsync<LocalChangeData>(
            getAllForOperationTypeQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return localChanges.AsList();
    }

    public async Task<LocalChangeData?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {LocalChangesTableName} WITH (NOLOCK)
        WHERE PK = @id;
        """;

        var parameters = new
        {
            id
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        LocalChangeData? localChange = await dbConnection.QueryFirstOrDefaultAsync<LocalChangeData>(
            getByIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return localChange;
    }

    public async Task<LocalChangeData?> GetByTableNameAndElementIdAndOperationTypeAsync(string tableName, int elementId, ChangeOperationType changeOperationType)
    {
        const string getByTableNameAndElementIdQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {LocalChangesTableName} WITH (NOLOCK)
        WHERE TableName = @tableName
        AND ID = @elementId
        AND Operation = @changeOperationType;
        """;

        var parameters = new
        {
            tableName,
            elementId,
            changeOperationType = (int)changeOperationType
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        LocalChangeData? localChange = await dbConnection.QueryFirstOrDefaultAsync<LocalChangeData>(
            getByTableNameAndElementIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return localChange;
    }
}