using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ToDoLocalChanges;
using OneOf;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ToDoLocalChangesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class ToDoLocalChangesRepository : IToDoLocalChangesRepository
{
    public ToDoLocalChangesRepository(
        [FromKeyedServices(ConfigureServices.ConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<LocalChangeData>> GetAllAsync()
    {
        const string getAllQuery =
        $"""
        SELECT {IdColumnName} AS {IdColumnAlias},
            {TableElementIdColumnName} AS {TableElementIdColumnAlias},
            {OperationTypeColumnName} AS {OperationTypeColumnAlias}, 
            {TableNameColumnName} AS {TableNameColumnAlias},
            {TimeStampColumnName} AS {TimeStampColumnAlias} 
        FROM {ToDoLocalChangesTableName}
        ORDER BY {TimeStampColumnName};
        """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<LocalChangeData> data = await dbConnection.QueryAsync<LocalChangeData>(getAllQuery, new { });

        return data.AsList();
    }

    public async Task<List<LocalChangeData>> GetAllForTableAsync(string tableName)
    {
        const string getAllForTableQuery =
        $"""
        SELECT {IdColumnName} AS {IdColumnAlias},
            {TableElementIdColumnName} AS {TableElementIdColumnAlias},
            {OperationTypeColumnName} AS {OperationTypeColumnAlias}, 
            {TableNameColumnName} AS {TableNameColumnAlias},
            {TimeStampColumnName} AS {TimeStampColumnAlias} 
        FROM {ToDoLocalChangesTableName}
        WHERE {TableNameColumnName} = @tableName
        ORDER BY {TimeStampColumnName};
        """;

        var parameters = new
        {
            tableName = tableName
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<LocalChangeData> data = await dbConnection.QueryAsync<LocalChangeData>(getAllForTableQuery, parameters);

        return data.AsList();
    }

    public async Task<List<LocalChangeData>> GetAllForOperationTypeAsync(ChangeOperationType changeOperationType)
    {
        const string getAllForOperationTypeQuery =
        $"""
        SELECT {IdColumnName} AS {IdColumnAlias},
            {TableElementIdColumnName} AS {TableElementIdColumnAlias},
            {OperationTypeColumnName} AS {OperationTypeColumnAlias}, 
            {TableNameColumnName} AS {TableNameColumnAlias},
            {TimeStampColumnName} AS {TimeStampColumnAlias} 
        FROM {ToDoLocalChangesTableName}
        WHERE {OperationTypeColumnName} = @changeOperationType
        ORDER BY {TimeStampColumnName};
        """;

        var parameters = new
        {
            changeOperationType = (int)changeOperationType
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<LocalChangeData> data = await dbConnection.QueryAsync<LocalChangeData>(getAllForOperationTypeQuery, parameters);

        return data.AsList();
    }

    public async Task<LocalChangeData?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        const string getByIdQuery =
        $"""
        SELECT {IdColumnName} AS {IdColumnAlias},
            {TableElementIdColumnName} AS {TableElementIdColumnAlias},
            {OperationTypeColumnName} AS {OperationTypeColumnAlias}, 
            {TableNameColumnName} AS {TableNameColumnAlias},
            {TimeStampColumnName} AS {TimeStampColumnAlias} 
        FROM {ToDoLocalChangesTableName}
        WHERE {IdColumnName} = @id;
        """;

        var parameters = new
        {
            id = id
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<LocalChangeData>(getByIdQuery, parameters);
    }

    public async Task<LocalChangeData?> GetByTableNameAndElementIdAndOperationTypeAsync(string tableName, int elementId, ChangeOperationType changeOperationType)
    {
        const string getByTableNameAndElementIdQuery =
        $"""
        SELECT {IdColumnName} AS {IdColumnAlias},
            {TableElementIdColumnName} AS {TableElementIdColumnAlias},
            {OperationTypeColumnName} AS {OperationTypeColumnAlias}, 
            {TableNameColumnName} AS {TableNameColumnAlias},
            {TimeStampColumnName} AS {TimeStampColumnAlias} 
        FROM {ToDoLocalChangesTableName}
        WHERE {TableNameColumnName} = @tableName
        AND {TableElementIdColumnName} = @elementId
        AND {OperationTypeColumnName} = @changeOperationType;
        """;

        var parameters = new
        {
            tableName = tableName,
            elementId = elementId,
            changeOperationType = (int)changeOperationType
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<LocalChangeData>(getByTableNameAndElementIdQuery, parameters);
    }

    public async Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);

            INSERT INTO {ToDoLocalChangesTableName} ({TableElementIdColumnName}, {OperationTypeColumnName},
                {TableNameColumnName}, {TimeStampColumnName})
            OUTPUT INSERTED.{IdColumnName} INTO @InsertedIdTable
            VALUES(@TableElementId, @OperationType, @TableName, @TimeStamp)

            SELECT TOP 1 Id FROM @InsertedIdTable;
            """;

        var parameters = new
        {
            TableElementId = toDoLocalChangeCreateRequest.TableElementId,
            OperationType = toDoLocalChangeCreateRequest.OperationType,
            TableName = toDoLocalChangeCreateRequest.TableName,
            TimeStamp = toDoLocalChangeCreateRequest.TimeStamp,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? toDoLocalChangeId = await dbConnection.ExecuteScalarAsync<int?>(insertQuery, parameters);

        return toDoLocalChangeId is not null
            && toDoLocalChangeId > 0 ? toDoLocalChangeId.Value : new UnexpectedFailureResult();
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        if (id <= 0) return false;

        const string deleteByIdQuery =
            $"""
            DELETE FROM {ToDoLocalChangesTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new
        {
            id = id
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteByIdQuery, parameters);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteRangeByIdsAsync(IEnumerable<int> ids)
    {
        const string deleteByIdQuery =
            $"""
            DELETE FROM {ToDoLocalChangesTableName}
            WHERE {IdColumnName} IN @ids;
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<int> rowsAffectedPerChunk = await ExecuteQueryWithParametersInChunksAsync(
            idsChunk =>
            {
                var parameters = new
                {
                    ids = idsChunk
                };

                return dbConnection.ExecuteAsync(deleteByIdQuery, parameters);
            },
            ids.AsList());

        int rowsAffected = rowsAffectedPerChunk.Sum();

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByTableNameAndElementIdAsync(string tableName, int elementId)
    {
        const string deleteByTableNameAndElementIdQuery =
            $"""
            DELETE FROM {ToDoLocalChangesTableName}
            WHERE {TableNameColumnName} = @tableName
            AND {TableElementIdColumnName} = @elementId;
            """;

        var parameters = new
        {
            tableName = tableName,
            elementId = elementId
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteByTableNameAndElementIdQuery, parameters);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteRangeByTableNameAndElementIdsAsync(string tableName, IEnumerable<int> elementIds)
    {
        const string deleteByTableNameAndElementIdsQuery =
            $"""
            DELETE FROM {ToDoLocalChangesTableName}
            WHERE {TableNameColumnName} = @tableName
            AND {TableElementIdColumnName} IN @elementIds;
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<int> rowsAffectedPerChunk = await ExecuteQueryWithParametersInChunksAsync(
            async elementIdsChunk =>
            {
                var parameters = new
                {
                    tableName = tableName,
                    elementIds = elementIdsChunk
                };

                return await dbConnection.ExecuteAsync(deleteByTableNameAndElementIdsQuery, parameters);
            },
            elementIds.AsList());

        int rowsAffected = rowsAffectedPerChunk.Sum();

        return rowsAffected > 0;
    }
}