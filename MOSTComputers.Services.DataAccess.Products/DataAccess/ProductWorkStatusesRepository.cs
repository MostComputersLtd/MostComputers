using Dapper;
using FluentValidation.Results;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
using OneOf;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductWorkStatusesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class ProductWorkStatusesRepository : IProductWorkStatusesRepository
{
    public ProductWorkStatusesRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ProductWorkStatuses>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductNewStatusColumnName},
                {ProductXmlStatusColumnName},
                {ReadyForImageInsertColumnName},
                {CreateUserNameColumnName},
                {CreateDateColumnName},
                {LastUpdateUserNameColumnName},
                {LastUpdateDateColumnName}
            FROM {ProductWorkStatusesTableName}
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductWorkStatuses> data = await dbConnection.QueryAsync<ProductWorkStatuses>(getAllQuery, new { }, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<ProductWorkStatuses>> GetAllForProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductNewStatusColumnName},
                {ProductXmlStatusColumnName},
                {ReadyForImageInsertColumnName},
                {CreateUserNameColumnName},
                {CreateDateColumnName},
                {LastUpdateUserNameColumnName},
                {LastUpdateDateColumnName}
            FROM {ProductWorkStatusesTableName}
            WHERE {ProductIdColumnName} IN @productIds
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductWorkStatuses> productWorkStatuses = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return dbConnection.QueryAsync<ProductWorkStatuses>(getAllQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        return productWorkStatuses;
    }

    public async Task<List<ProductWorkStatuses>> GetAllWithProductNewStatusAsync(ProductNewStatus productNewStatusEnum)
    {
        const string getAllWithProductNewStatusQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductNewStatusColumnName},
                {ProductXmlStatusColumnName},
                {ReadyForImageInsertColumnName},
                {CreateUserNameColumnName},
                {CreateDateColumnName},
                {LastUpdateUserNameColumnName},
                {LastUpdateDateColumnName}
            FROM {ProductWorkStatusesTableName}
            WHERE {ProductNewStatusColumnName} = @productNewStatus
            """;

        var parameters = new { productNewStatus = (int)productNewStatusEnum };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductWorkStatuses> data = await dbConnection.QueryAsync<ProductWorkStatuses>(
            getAllWithProductNewStatusQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<ProductWorkStatuses>> GetAllWithProductXmlStatusAsync(ProductXmlStatus productXmlStatusEnum)
    {
        const string getAllWithProductXmlStatusQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductNewStatusColumnName},
                {ProductXmlStatusColumnName},
                {ReadyForImageInsertColumnName},
                {CreateUserNameColumnName},
                {CreateDateColumnName},
                {LastUpdateUserNameColumnName},
                {LastUpdateDateColumnName}
            FROM {ProductWorkStatusesTableName}
            WHERE {ProductXmlStatusColumnName} = @productXmlStatus
            """;

        var parameters = new { productXmlStatus = (int)productXmlStatusEnum };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductWorkStatuses> data = await dbConnection.QueryAsync<ProductWorkStatuses>(
            getAllWithProductXmlStatusQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<ProductWorkStatuses>> GetAllWithReadyForImageInsertAsync(bool readyForImageInsert)
    {
        const string getAllWithReadyForImageInsertQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductNewStatusColumnName},
                {ProductXmlStatusColumnName},
                {ReadyForImageInsertColumnName},
                {CreateUserNameColumnName},
                {CreateDateColumnName},
                {LastUpdateUserNameColumnName},
                {LastUpdateDateColumnName}
            FROM {ProductWorkStatusesTableName}
            WHERE {ReadyForImageInsertColumnName} = @readyForImageInsert
            """;

        var parameters = new { readyForImageInsert = readyForImageInsert };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductWorkStatuses> data = await dbConnection.QueryAsync<ProductWorkStatuses>(
            getAllWithReadyForImageInsertQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<ProductWorkStatuses?> GetByIdAsync(int productWorkStatusesId)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 {IdColumnName} AS {IdColumnAlias},
                {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductNewStatusColumnName},
                {ProductXmlStatusColumnName},
                {ReadyForImageInsertColumnName},
                {CreateUserNameColumnName},
                {CreateDateColumnName},
                {LastUpdateUserNameColumnName},
                {LastUpdateDateColumnName}
            FROM {ProductWorkStatusesTableName}
            WHERE {IdColumnName} = @productWorkStatusesId
            """;

        var parameters = new { productWorkStatusesId = productWorkStatusesId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductWorkStatuses? data = await dbConnection.QueryFirstOrDefaultAsync<ProductWorkStatuses>(getByIdQuery, parameters, commandType: CommandType.Text);

        return data;
    }

    public async Task<ProductWorkStatuses?> GetByProductIdAsync(int productId)
    {
        const string getAllByProductIdQuery =
            $"""
            SELECT TOP 1 {IdColumnName} AS {IdColumnAlias},
                {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductNewStatusColumnName},
                {ProductXmlStatusColumnName},
                {ReadyForImageInsertColumnName},
                {CreateUserNameColumnName},
                {CreateDateColumnName},
                {LastUpdateUserNameColumnName},
                {LastUpdateDateColumnName}
            FROM {ProductWorkStatusesTableName}
            WHERE {ProductIdColumnName} = @productId
            """;

        var parameters = new { productId = productId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductWorkStatuses? data = await dbConnection.QueryFirstOrDefaultAsync<ProductWorkStatuses>(getAllByProductIdQuery, parameters, commandType: CommandType.Text);

        return data;
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertIfItDoesntExistAsync(ProductWorkStatusesCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @Status INT = 0;
            DECLARE @InsertedIdTable TABLE (Id INT);

            IF EXISTS (
                SELECT 1 FROM {ProductWorkStatusesTableName}
                WHERE {ProductIdColumnName} = @productId)
            BEGIN
                SET @Status = -1;
            END

            IF @Status = 0
            BEGIN
                INSERT INTO {ProductWorkStatusesTableName} ({ProductIdColumnName},
                    {ProductNewStatusColumnName}, {ProductXmlStatusColumnName}, {ReadyForImageInsertColumnName},
                    {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName})
                OUTPUT INSERTED.{IdColumnName} INTO @InsertedIdTable
                VALUES(@productId, @ProductNewStatus, @ProductXmlStatus, @ReadyForImageInsert,
                    @CreateUserName, @CreateDate, @LastUpdateUserName, @LastUpdateDate)
            END

            SELECT ISNULL((SELECT TOP 1 Id FROM @InsertedIdTable), @Status);
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            ProductNewStatus = (int)createRequest.ProductNewStatus,
            ProductXmlStatus = (int)createRequest.ProductXmlStatus,
            ReadyForImageInsert = createRequest.ReadyForImageInsert,
            CreateUserName = createRequest.CreateUserName,
            CreateDate = createRequest.CreateDate,
            LastUpdateUserName = createRequest.LastUpdateUserName,
            LastUpdateDate = createRequest.LastUpdateDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? result = await dbConnection.ExecuteScalarAsync<int?>(insertQuery, parameters, commandType: CommandType.Text);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return result.Value;

        ValidationResult validationResult = GetValidationResultFromFailedInsertResult(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    private static ValidationResult GetValidationResultFromFailedInsertResult(int result)
    {
        ValidationResult validationResult = new();

        if (result == -1)
        {
            validationResult.Errors.Add(new(nameof(ProductStatuses.ProductId), "Product status already exists for this product"));
        }

        return validationResult;
    }

    public async Task<bool> UpdateByIdAsync(ProductWorkStatusesUpdateByIdRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {ProductWorkStatusesTableName}
            SET {ProductNewStatusColumnName} = @ProductNewStatus,
                {ProductXmlStatusColumnName} = @ProductXmlStatus,
                {ReadyForImageInsertColumnName} = @ReadyForImageInsert,
                {LastUpdateUserNameColumnName} = @LastUpdateUserName,
                {LastUpdateDateColumnName} = LastUpdateDate
            
            WHERE {IdColumnName} = @id
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            ProductNewStatus = updateRequest.ProductNewStatus,
            ProductXmlStatus = updateRequest.ProductXmlStatus,
            ReadyForImageInsert = updateRequest.ReadyForImageInsert,
            LastUpdateUserName = updateRequest.LastUpdateUserName,
            LastUpdateDate = updateRequest.LastUpdateDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateByProductIdAsync(ProductWorkStatusesUpdateByProductIdRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {ProductWorkStatusesTableName}
            SET {ProductNewStatusColumnName} = @ProductNewStatus,
                {ProductXmlStatusColumnName} = @ProductXmlStatus,
                {ReadyForImageInsertColumnName} = @ReadyForImageInsert,
                {LastUpdateUserNameColumnName} = @LastUpdateUserName,
                {LastUpdateDateColumnName} = LastUpdateDate
            
            WHERE {ProductIdColumnName} = @productId
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            ProductNewStatus = updateRequest.ProductNewStatus,
            ProductXmlStatus = updateRequest.ProductXmlStatus,
            ReadyForImageInsert = updateRequest.ReadyForImageInsert,
            LastUpdateUserName = updateRequest.LastUpdateUserName,
            LastUpdateDate = updateRequest.LastUpdateDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<OneOf<int, UnexpectedFailureResult>> UpsertByProductIdAsync(ProductWorkStatusesUpsertRequest upsertRequest)
    {
        const string upsertQuery =
            $"""
            IF EXISTS (
                SELECT 1 FROM {ProductWorkStatusesTableName}
                WHERE {ProductIdColumnName} = @productId
            )
            BEGIN
                UPDATE {ProductWorkStatusesTableName}
                SET {ProductNewStatusColumnName} = @ProductNewStatus,
                    {ProductXmlStatusColumnName} = @ProductXmlStatus,
                    {ReadyForImageInsertColumnName} = @ReadyForImageInsert,
                    {LastUpdateUserNameColumnName} = @UpsertUserName,
                    {LastUpdateDateColumnName} = @UpsertDate
            
                WHERE {ProductIdColumnName} = @productId;

                SELECT TOP 1 {IdColumnName} FROM {ProductWorkStatusesTableName}
                WHERE {ProductIdColumnName} = @productId
            END
            ELSE
            BEGIN
                DECLARE @InsertedIdTable TABLE (Id INT);
            
                INSERT INTO {ProductWorkStatusesTableName} ({ProductIdColumnName},
                    {ProductNewStatusColumnName}, {ProductXmlStatusColumnName}, {ReadyForImageInsertColumnName},
                    {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName})
                OUTPUT INSERTED.{IdColumnName} INTO @InsertedIdTable
                VALUES(@productId, @ProductNewStatus, @ProductXmlStatus, @ReadyForImageInsert,
                    @UpsertUserName, @UpsertDate, @UpsertUserName, @UpsertDate);

                SELECT TOP 1 Id FROM @InsertedIdTable;
            END
            """;

        var parameters = new
        {
            productId = upsertRequest.ProductId,
            ProductNewStatus = (int)upsertRequest.ProductNewStatus,
            ProductXmlStatus = (int)upsertRequest.ProductXmlStatus,
            ReadyForImageInsert = upsertRequest.ReadyForImageInsert,
            UpsertUserName = upsertRequest.UpsertUserName,
            UpsertDate = upsertRequest.UpsertDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? elementId = await dbConnection.ExecuteScalarAsync<int?>(upsertQuery, parameters, commandType: CommandType.Text);

        if (elementId is null || elementId <= 0) return new UnexpectedFailureResult();

        return elementId.Value;
    }

    public async Task<bool> DeleteAllAsync()
    {
        const string deleteAllQuery =
            $"""
            DELETE FROM {ProductWorkStatusesTableName}
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteAllQuery, new { }, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAllWithProductNewStatusAsync(ProductNewStatus productNewStatusEnum)
    {
        const string deleteAllWithProductNewStatusQuery =
            $"""
            DELETE FROM {ProductWorkStatusesTableName}
            WHERE {ProductNewStatusColumnName} = @productNewStatus
            """;

        var parameters = new { productNewStatus = (int)productNewStatusEnum };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteAllWithProductNewStatusQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAllWithProductXmlStatusAsync(ProductXmlStatus productXmlStatusEnum)
    {
        const string deleteAllWithProductXmlStatusQuery =
            $"""
            DELETE FROM {ProductWorkStatusesTableName}
            WHERE {ProductXmlStatusColumnName} = @productXmlStatus
            """;

        var parameters = new { productXmlStatus = (int)productXmlStatusEnum };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteAllWithProductXmlStatusQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAllWithReadyForImageInsertAsync(bool readyForImageInsert)
    {
        const string deleteAllWithReadyForImageInsertQuery =
            $"""
            DELETE FROM {ProductWorkStatusesTableName}
            WHERE {ReadyForImageInsertColumnName} = @readyForImageInsert
            """;

        var parameters = new { readyForImageInsert = readyForImageInsert };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteAllWithReadyForImageInsertQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByIdAsync(int productWorkStatusesId)
    {
        const string deleteByIdQuery =
            $"""
            DELETE FROM {ProductWorkStatusesTableName}
            WHERE {IdColumnName} = @productWorkStatusesId
            """;

        var parameters = new { productWorkStatusesId = productWorkStatusesId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteByIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByProductIdAsync(int productId)
    {
        const string deleteAllByProductIdQuery =
            $"""
            DELETE FROM {ProductWorkStatusesTableName}
            WHERE {ProductIdColumnName} = @productId
            """;

        var parameters = new { productId = productId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteAllByProductIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }
}