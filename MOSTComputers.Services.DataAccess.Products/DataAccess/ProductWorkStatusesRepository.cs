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
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductWorkStatuses;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Reflection.Metadata;
using System.Text;
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

        ValidationResult validationResult = GetValidationResultFromFailedInsertResult(result.Value, nameof(ProductWorkStatusesCreateRequest.ProductId));

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    public async Task<ProductWorkStatusesCreateManyWithSameDataResponse> InsertAllIfTheyDontExistAsync(
        ProductWorkStatusesCreateManyWithSameDataRequest createRequest)
    {
        const string insertQueryResultsTableDeclaration =
            $"""
            DECLARE @InsertedIdTable TABLE (ProductId INT, IdOrStatus INT);


            """;

        const string insertQueryFinalSelect =
            """

            SELECT ProductId, IdOrStatus FROM @InsertedIdTable;
            """;

        SqlParameter productNewStatusParameter = new()
        {
            ParameterName = $"@ProductNewStatus",
            Value = (int)createRequest.ProductNewStatus,
            SqlDbType = SqlDbType.Int,
            IsNullable = false,
        };

        SqlParameter productXmlStatusParameter = new()
        {
            ParameterName = $"@ProductXmlStatus",
            Value = (int)createRequest.ProductXmlStatus,
            SqlDbType = SqlDbType.Int,
            IsNullable = false,
        };

        SqlParameter readyForImageInsertParameter = new()
        {
            ParameterName = $"@ReadyForImageInsert",
            Value = createRequest.ReadyForImageInsert,
            SqlDbType = SqlDbType.Bit,
            IsNullable = false,
        };

        SqlParameter createUserNameParameter = new()
        {
            ParameterName = $"@CreateUserName",
            Value = createRequest.CreateUserName,
            SqlDbType = SqlDbType.VarChar,
            IsNullable = false,
        };

        SqlParameter createDateParameter = new()
        {
            ParameterName = $"@CreateDate",
            Value = createRequest.CreateDate,
            SqlDbType = SqlDbType.DateTime,
            IsNullable = false,
        };

        SqlParameter lastUpdateUserNameParameter = new()
        {
            ParameterName = $"@LastUpdateUserName",
            Value = createRequest.LastUpdateUserName,
            SqlDbType = SqlDbType.VarChar,
            IsNullable = false,
        };

        SqlParameter lastUpdateDateParameter = new()
        {
            ParameterName = $"@LastUpdateDate",
            Value = createRequest.LastUpdateDate,
            SqlDbType = SqlDbType.DateTime,
            IsNullable = false,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        await dbConnection.OpenAsync();

        using SqlTransaction transaction = dbConnection.BeginTransaction();

        List<Tuple<int, int>> results;

        try
        {
            results = await ExecuteListQueryWithParametersInChunksAsync(
                RunInsertAllQueryForChunkOfProductIdsAsync,
                createRequest.ProductIds);

            await transaction.CommitAsync();
        }
        catch
                
        {
            await transaction.RollbackAsync();

            throw;
        }

        Dictionary<int, OneOf<int, ValidationResult, UnexpectedFailureResult>> output = new();

        for (int i = 0; i < results.Count; i++)
        {
            Tuple<int, int> result = results[i];

            OneOf<int, ValidationResult, UnexpectedFailureResult> outputForProduct = GetResultForProduct(result.Item2, i);

            output.Add(result.Item1, outputForProduct);
        }

        return new()
        {
            Results = output
        };

        static string GetInsertQueryWithProductIdParam(string productIdParamName)
        {
            string insert =
                $"""
                IF EXISTS (
                    SELECT 1 FROM {ProductWorkStatusesTableName}
                    WHERE {ProductIdColumnName} = {productIdParamName})
                BEGIN
                    INSERT INTO InsertedIdTable (ProductId, IdOrStatus)
                    VALUES ({productIdParamName}, -1)
                END
                ELSE
                BEGIN
                    INSERT INTO {ProductWorkStatusesTableName} ({ProductIdColumnName},
                        {ProductNewStatusColumnName}, {ProductXmlStatusColumnName}, {ReadyForImageInsertColumnName},
                        {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName})
                    OUTPUT INSERTED.{ProductIdColumnName}, INSERTED.{IdColumnName} INTO @InsertedIdTable
                    VALUES({productIdParamName}, @ProductNewStatus, @ProductXmlStatus, @ReadyForImageInsert,
                        @CreateUserName, @CreateDate, @LastUpdateUserName, @LastUpdateDate)
                END
                """;

            return insert;
        }

        async Task<IEnumerable<Tuple<int, int>>> RunInsertAllQueryForChunkOfProductIdsAsync(List<int> productIds)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine(insertQueryResultsTableDeclaration);

            SqlParameter[] productIdParameters = new SqlParameter[productIds.Count];

            for (int i = 0; i < productIds.Count; i++)
            {
                SqlParameter productIdParameter = new()
                {
                    ParameterName = $"@ProductId{i}",
                    Value = productIds[i],
                    SqlDbType = SqlDbType.Int,
                    IsNullable = false,
                };

                productIdParameters[i] = productIdParameter;

                string insertQuery = GetInsertQueryWithProductIdParam(productIdParameter.ParameterName);

                stringBuilder.AppendLine(insertQuery);
            }

            stringBuilder.AppendLine(insertQueryFinalSelect);

            using SqlCommand sqlCommand = new()
            {
                Connection = dbConnection,
                Transaction = transaction,
                CommandText = stringBuilder.ToString(),
                CommandType = CommandType.Text,
            };

            sqlCommand.Parameters.AddRange(productIdParameters);

            sqlCommand.Parameters.Add(productNewStatusParameter);
            sqlCommand.Parameters.Add(productXmlStatusParameter);
            sqlCommand.Parameters.Add(readyForImageInsertParameter);
            sqlCommand.Parameters.Add(createUserNameParameter);
            sqlCommand.Parameters.Add(createDateParameter);
            sqlCommand.Parameters.Add(lastUpdateUserNameParameter);
            sqlCommand.Parameters.Add(lastUpdateDateParameter);

            List<Tuple<int, int>> localResults = new();

            using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int productId = reader.GetInt32(0);
                    int idOrStatus = reader.GetInt32(1);

                    localResults.Add(new(productId, idOrStatus));
                }
            }

            sqlCommand.Parameters.Clear();

            return localResults;
        }

        static OneOf<int, ValidationResult, UnexpectedFailureResult> GetResultForProduct(int idOrStatus, int indexOfEntry)
        {
            if (idOrStatus == 0) return new UnexpectedFailureResult();

            if (idOrStatus > 0) return idOrStatus;

            string productIdPropertyName = $"{nameof(ProductWorkStatusesCreateManyWithSameDataRequest.ProductIds)}.[{indexOfEntry}]";

            ValidationResult validationResult = GetValidationResultFromFailedInsertResult(idOrStatus, productIdPropertyName);

            return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
        }
    }

    private static ValidationResult GetValidationResultFromFailedInsertResult(int result, string productIdPropertyName)
    {
        ValidationResult validationResult = new();

        if (result == -1)
        {
            validationResult.Errors.Add(new(productIdPropertyName, "Product status already exists for this product"));
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