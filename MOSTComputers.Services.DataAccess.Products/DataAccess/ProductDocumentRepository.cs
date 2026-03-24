using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductDocument;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;
using Dapper;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductDocumentsTable;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class ProductDocumentRepository : IProductDocumentRepository
{
    private readonly IConnectionStringProvider _connectionStringProvider;

    public ProductDocumentRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async Task<List<ProductDocument>> GetAllForProductAsync(int productId)
    {
        const string query =
            $"""
            SELECT
                {IdColumnName},
                {ProductIdColumnName},
                {FileExtensionColumnName},
                {FileNameColumnName},
                {DescriptionColumnName}
            FROM {ProductDocumentsTableName}
            WHERE {ProductIdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId = productId,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductDocument> data = await dbConnection.QueryAsync<ProductDocument>(query, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<ProductDocument?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1
                {IdColumnName},
                {ProductIdColumnName},
                {FileExtensionColumnName},
                {FileNameColumnName},
                {DescriptionColumnName}
            FROM {ProductDocumentsTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new
        {
            id = id,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductDocument? data = await dbConnection.QueryFirstOrDefaultAsync<ProductDocument>(query, parameters, commandType: CommandType.Text);

        return data;
    }

    public async Task<OneOf<ProductDocument, UnexpectedFailureResult>> InsertAsync(ProductDocumentCreateRequest createRequest)
    {
        const string query =
            $"""
            SET ARITHABORT ON;

            DECLARE @InsertedIdTable TABLE (Id INT);

            INSERT INTO {ProductDocumentsTableName} (
                {ProductIdColumnName},
                {DescriptionColumnName},
                {FileExtensionColumnName})
            OUTPUT INSERTED.{IdColumnName} INTO @InsertedIdTable
            VALUES (@ProductId, @Description, @FileExtension);

            SELECT TOP 1
                pd.{IdColumnName},
                {ProductIdColumnName},
                {FileNameColumnName},
                {FileExtensionColumnName},
                {DescriptionColumnName}
            FROM {ProductDocumentsTableName} pd
            INNER JOIN @InsertedIdTable insertedIdTable
            ON insertedIdTable.Id = pd.{IdColumnName};
            """;

        var parameters = new
        {
            ProductId = createRequest.ProductId,
            FileExtension = createRequest.FileExtension,
            Description = createRequest.Description,
        };

        using TransactionScope transactionScope = new(
            TransactionScopeOption.Required,
            new TransactionOptions()
            {
                IsolationLevel = System.Transactions.IsolationLevel.Serializable
            },
            TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductDocument? productDocument = await dbConnection.QueryFirstOrDefaultAsync<ProductDocument>(
            query, parameters, commandType: CommandType.Text);

        if (productDocument != null)
        {
            transactionScope.Complete();

            return productDocument;
        }

        return new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(ProductDocumentUpdateRequest updateRequest)
    {
        const string updateByIdQuery =
            $"""
            UPDATE {ProductDocumentsTableName}
            SET {DescriptionColumnName} = @Description
            WHERE {IdColumnName} = @Id;
            """;

        const string updateByFileNameQuery =
            $"""
            UPDATE {ProductDocumentsTableName}
            SET {DescriptionColumnName} = @Description
            WHERE {FileNameColumnName} = @FileName;
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected;

        if (updateRequest.IdOrFileName.IsT0)
        {
            var parameters = new
            {
                Id = updateRequest.IdOrFileName.AsT0,
                Description = updateRequest.Description,
            };

            rowsAffected = await dbConnection.ExecuteAsync(updateByIdQuery, parameters, commandType: CommandType.Text);
        }
        else
        {
            var parameters = new
            {
                Id = updateRequest.IdOrFileName.AsT0,
                Description = updateRequest.Description,
            };

            rowsAffected = await dbConnection.ExecuteAsync(updateByFileNameQuery, parameters, commandType: CommandType.Text);
        }

        if (rowsAffected <= 0)
        {
            return new NotFound();
        }

        return new Success();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(OneOf<int, string> idOrFileName)
    {
        const string deleteByIdQuery =
            $"""
            DELETE FROM {ProductDocumentsTableName}
            WHERE {IdColumnName} = @Id;
            """;

        const string deleteByFileNameQuery =
            $"""
            DELETE FROM {ProductDocumentsTableName}
            WHERE {FileNameColumnName} = @FileName;
            """;

        if (idOrFileName.IsT0)
        {
            var deleteByIdParameters = new
            {
                id = idOrFileName.AsT0,
            };

            using SqlConnection deleteByIdDbConnection = new(_connectionStringProvider.ConnectionString);

            int deleteByIdRowsAffected = await deleteByIdDbConnection.ExecuteAsync(deleteByIdQuery, deleteByIdParameters, commandType: CommandType.Text);

            return deleteByIdRowsAffected > 0 ? new Success() : new NotFound();
        }

        var deleteByFileNameParameters = new
        {
            id = idOrFileName.AsT0,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteByFileNameQuery, deleteByFileNameParameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new NotFound();
    }
}