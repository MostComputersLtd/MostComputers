using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ExternalXmlImport.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ExternalXmlImport;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Data.Common;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductCharacteristicAndExternalXmlDataRelationsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ExternalXmlImport;
internal sealed class ProductCharacteristicAndExternalXmlDataRelationRepository : IProductCharacteristicAndExternalXmlDataRelationRepository
{
    public ProductCharacteristicAndExternalXmlDataRelationRepository(
        [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndExternalXmlDataRelation>(
            getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdAsync(int categoryId)
    {
        const string getAllWithSameCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {CategoryIdColumnName} = @categoryId;
            """;

        var parameters = new { categoryId = categoryId };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndExternalXmlDataRelation>(
            getAllWithSameCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdsAsync(IEnumerable<int> categoryIds)
    {
        const string getAllWithSameCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {CategoryIdColumnName} IN @categoryIds
            ORDER BY {CategoryIdColumnName};
            """;

        var parameters = new { categoryIds = categoryIds };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndExternalXmlDataRelation>(
            getAllWithSameCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithSameCategoryIdAndXmlNameAsync(int categoryId, string xmlName)
    {
        const string getAllWithSameCategoryIdAndXmlNameQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {CategoryIdColumnName} = @categoryId
            AND {XmlNameColumnName} = @Name;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = xmlName
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndExternalXmlDataRelation>(
            getAllWithSameCategoryIdAndXmlNameQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndExternalXmlDataRelation>> GetAllWithCharacteristicIdAsync(int characteristicId)
    {
        const string getByCharacteristicIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {ProductCharacteristicIdColumnName} = @characteristicId;
            """;

        var parameters = new { characteristicId = characteristicId };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndExternalXmlDataRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndExternalXmlDataRelation>(
            getByCharacteristicIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<ProductCharacteristicAndExternalXmlDataRelation?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id = id };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductCharacteristicAndExternalXmlDataRelation? data = await dbConnection.QueryFirstOrDefaultAsync<ProductCharacteristicAndExternalXmlDataRelation>(
            getByIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data;
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpsertByCharacteristicIdAsync(ProductCharacteristicAndExternalXmlDataRelationUpsertRequest createRequest)
    {
        const string upsertByCharacteristicIdQuery =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {CategoryIdColumnName} = @CategoryId AND {XmlNameColumnName} = @XmlName AND {XmlDisplayOrderColumnName} = @XmlDisplayOrder)
            OR (@ProductCharacteristicId = NULL)
            BEGIN
                INSERT INTO {ProductCharacteristicAndExternalXmlDataRelationsTableName} (
                    {CategoryIdColumnName}, {ProductCharacteristicIdColumnName}, {XmlNameColumnName}, {XmlDisplayOrderColumnName})
                VALUES (@CategoryId, @ProductCharacteristicId, @XmlName, @XmlDisplayOrder)
            END
            ELSE
            BEGIN
                UPDATE {ProductCharacteristicAndExternalXmlDataRelationsTableName}
                SET {ProductCharacteristicIdColumnName} = @ProductCharacteristicId

                WHERE {CategoryIdColumnName} = @CategoryId
                AND {XmlNameColumnName} = @XmlName
                AND {XmlDisplayOrderColumnName} = @XmlDisplayOrder;
            END
            """;

        var parameters = new
        {
            CategoryId = createRequest.CategoryId,
            ProductCharacteristicId = createRequest.ProductCharacteristicId,
            XmlName = createRequest.XmlName,
            XmlDisplayOrder = createRequest.XmlDisplayOrder,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(upsertByCharacteristicIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<bool> DeleteAllWithSameCategoryIdAsync(int categoryId)
    {
        const string deleteAllWithSameCategoryIdQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {CategoryIdColumnName} = @categoryId;
            """;

        var parameters = new { categoryId = categoryId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteAllWithSameCategoryIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAllWithSameCategoryIdAndXmlNameAsync(int categoryId, string xmlName)
    {
        const string deleteAllWithSameCategoryIdAndXmlNameQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {CategoryIdColumnName} = @categoryId
            AND {XmlNameColumnName} = @Name;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = xmlName
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteAllWithSameCategoryIdAndXmlNameQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        const string deleteByIdQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id = id };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteByIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByCharacteristicIdAsync(int characteristicId)
    {
        const string deleteByCharacteristicIdQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE {ProductCharacteristicIdColumnName} = @characteristicId;
            """;

        var parameters = new { characteristicId = characteristicId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteByCharacteristicIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }
}