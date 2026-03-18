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
        [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
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
            DECLARE @InsertedIdTable TABLE (Id INT);

            INSERT INTO {ProductDocumentsTableName} (
                {ProductIdColumnName},
                {DescriptionColumnName})
            OUTPUT INSERTED.{IdColumnName} INTO @InsertedIdTable (Id)
            VALUES (@ProductId, @Description);

            UPDATE pd
            SET {FileNameColumnName} = CAST({ProductIdColumnName} AS VARCHAR(10)) + '-' + CAST({IdColumnName} AS VARCHAR(10)) + '.' + @FileExtension
            FROM {ProductDocumentsTableName} pd
            INNER JOIN @InsertedIdTable insertedIdTable
            ON insertedIdTable.Id = pd.{IdColumnName};

            SELECT TOP 1
                pd.{IdColumnName},
                {ProductIdColumnName},
                {FileNameColumnName},
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

        ProductDocument? productDocument = await dbConnection.ExecuteScalarAsync<ProductDocument>(query, parameters, commandType: CommandType.Text);

        if (productDocument != null)
        {
            transactionScope.Complete();

            return productDocument;
        }

        return new UnexpectedFailureResult();
    }
}