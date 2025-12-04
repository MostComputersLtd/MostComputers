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
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductCharacteristicAndImageHtmlRelationsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ExternalXmlImport;
public sealed class ProductCharacteristicAndImageHtmlRelationRepository
{
    public ProductCharacteristicAndImageHtmlRelationRepository(
        [FromKeyedServices(ConfigureServices.ConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ProductCharacteristicAndImageHtmlRelation>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndImageHtmlRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndImageHtmlRelation>(
            getAllQuery, new { }, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndImageHtmlRelation>> GetAllWithSameCategoryIdAsync(int categoryId)
    {
        const string getAllWithSameCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {CategoryIdColumnName} = @categoryId;
            """;

        var parameters = new { categoryId = categoryId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndImageHtmlRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndImageHtmlRelation>(
            getAllWithSameCategoryIdQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndImageHtmlRelation>> GetAllWithSameCategoryIdsAsync(IEnumerable<int> categoryIds)
    {
        const string getAllWithSameCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {CategoryIdColumnName} IN @categoryIds
            ORDER BY {CategoryIdColumnName};
            """;

        var parameters = new { categoryIds = categoryIds };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndImageHtmlRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndImageHtmlRelation>(
            getAllWithSameCategoryIdQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndImageHtmlRelation>> GetAllWithSameCategoryIdAndXmlNameAsync(int categoryId, string xmlName)
    {
        const string getAllWithSameCategoryIdAndXmlNameQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {CategoryIdColumnName} = @categoryId
            AND {HtmlNameColumnName} = @Name;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = xmlName
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndImageHtmlRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndImageHtmlRelation>(
            getAllWithSameCategoryIdAndXmlNameQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<ProductCharacteristicAndImageHtmlRelation>> GetAllWithCharacteristicIdAsync(int characteristicId)
    {
        const string getByCharacteristicIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {ProductCharacteristicIdColumnName} = @characteristicId;
            """;

        var parameters = new { characteristicId = characteristicId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristicAndImageHtmlRelation> data = await dbConnection.QueryAsync<ProductCharacteristicAndImageHtmlRelation>(
            getByCharacteristicIdQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<ProductCharacteristicAndImageHtmlRelation?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 * FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id = id };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductCharacteristicAndImageHtmlRelation? data = await dbConnection.QueryFirstOrDefaultAsync<ProductCharacteristicAndImageHtmlRelation>(
            getByIdQuery, parameters, commandType: CommandType.Text);

        return data;
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpsertByCharacteristicIdAsync(ProductCharacteristicAndImageHtmlRelationUpsertRequest upsertRequest)
    {
        const string upsertByCharacteristicIdQuery =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {CategoryIdColumnName} = @CategoryId AND {HtmlNameColumnName} = @HtmlName)
            OR (@ProductCharacteristicId = NULL)
            BEGIN
                INSERT INTO {ProductCharacteristicAndImageHtmlRelationsTableName} (
                    {CategoryIdColumnName}, {ProductCharacteristicIdColumnName}, {HtmlNameColumnName})
                VALUES (@CategoryId, @ProductCharacteristicId, @HtmlName)
            END
            ELSE
            BEGIN
                UPDATE {ProductCharacteristicAndImageHtmlRelationsTableName}
                SET {ProductCharacteristicIdColumnName} = @ProductCharacteristicId

                WHERE {CategoryIdColumnName} = @CategoryId
                AND {HtmlNameColumnName} = @HtmlName;
            END
            """;

        var parameters = new
        {
            CategoryId = upsertRequest.CategoryId,
            ProductCharacteristicId = upsertRequest.ProductCharacteristicId,
            HtmlName = upsertRequest.HtmlName,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(upsertByCharacteristicIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<bool> DeleteAllWithSameCategoryIdAsync(int categoryId)
    {
        const string deleteAllWithSameCategoryIdQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
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
            DELETE FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {CategoryIdColumnName} = @categoryId
            AND {HtmlNameColumnName} = @Name;
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
            DELETE FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
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
            DELETE FROM {ProductCharacteristicAndImageHtmlRelationsTableName}
            WHERE {ProductCharacteristicIdColumnName} = @characteristicId;
            """;

        var parameters = new { characteristicId = characteristicId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteByCharacteristicIdQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }
}