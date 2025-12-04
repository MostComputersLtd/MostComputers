using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.ProductIdentifiers;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductGTINCode;
using OneOf;
using OneOf.Types;
using System.Data;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductGTINCodesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers;
internal class ProductGTINCodeRepository : IProductGTINCodeRepository
{
    public ProductGTINCodeRepository(
        [FromKeyedServices(ConfigureServices.ConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<IGrouping<int, ProductGTINCode>>> GetAllForProductsAsync(List<int> productIds)
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName}, {CodeTypeColumnName}, {ValueColumnName}, {CodeTypeAsTextColumnName},
                {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName}
            FROM {ProductGTINCodesTableName}
            WHERE {ProductIdColumnName} IN @productIds;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            productIds = productIds
        };

        IEnumerable<ProductGTINCodeDAO> productGTINCodeDAOs = await connection.QueryAsync<ProductGTINCodeDAO>(
            query, parameters, commandType: CommandType.Text);

        List<ProductGTINCode> productGTINCodes = MapRangeFromDAO(productGTINCodeDAOs);

        return productGTINCodes
            .GroupBy(productGTINCode => productGTINCode.ProductId)
            .ToList();
    }

    public async Task<List<ProductGTINCode>> GetAllForProductAsync(int productId)
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName}, {CodeTypeColumnName}, {ValueColumnName}, {CodeTypeAsTextColumnName},
                {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName}
            FROM {ProductGTINCodesTableName}
            WHERE {ProductIdColumnName} = @productId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            productId = productId
        };

        IEnumerable<ProductGTINCodeDAO> productGTINCodeDAOs = await connection.QueryAsync<ProductGTINCodeDAO>(
            query, parameters, commandType: CommandType.Text);

        List<ProductGTINCode> productGTINCodes = MapRangeFromDAO(productGTINCodeDAOs);

        return productGTINCodes.ToList();
    }

    public async Task<ProductGTINCode?> GetByProductIdAndTypeAsync(int productId, ProductGTINCodeType productGTINCodeType)
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName}, {CodeTypeColumnName}, {ValueColumnName}, {CodeTypeAsTextColumnName},
                {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName}
            FROM {ProductGTINCodesTableName}
            WHERE {ProductIdColumnName} = @productId
            AND {CodeTypeColumnName} = @codeType;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            productId = productId,
            codeType = productGTINCodeType.Value
        };

        ProductGTINCodeDAO? productGTINCodeDAO = await connection.QueryFirstOrDefaultAsync<ProductGTINCodeDAO>(
            query, parameters, commandType: CommandType.Text);

        if (productGTINCodeDAO is null) return null;

        return MapFromDAO(productGTINCodeDAO);
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> InsertAsync(ProductGTINCodeCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {ProductGTINCodesTableName} (
                {ProductIdColumnName}, {CodeTypeColumnName}, {ValueColumnName}, {CodeTypeAsTextColumnName},
                {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName})
            VALUES (@productId, @CodeType, @CodeTypeAsText, @Value,
                @CreateUserName, @CreateDate, @LastUpdateUserName, @LastUpdateDate)
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,

            CodeType = createRequest.CodeType.Value,

            CodeTypeAsText = createRequest.CodeTypeAsText,
            Value = createRequest.Value,
            CreateUserName = createRequest.CreateUserName,
            CreateDate = createRequest.CreateDate,
            LastUpdateUserName = createRequest.CreateUserName,
            LastUpdateDate = createRequest.CreateDate
        };

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(insertQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(ProductGTINCodeUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {ProductGTINCodesTableName}
            SET {CodeTypeAsTextColumnName} = @CodeTypeAsText,
                {ValueColumnName} = @Value,
                {LastUpdateUserNameColumnName} = @LastUpdateUserName,
                {LastUpdateDateColumnName} = @LastUpdateDate
            WHERE {ProductIdColumnName} = @productId
            AND {CodeTypeColumnName} = @CodeType
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,

            CodeType = updateRequest.CodeType.Value,

            CodeTypeAsText = updateRequest.CodeTypeAsText,
            Value = updateRequest.Value,

            LastUpdateUserName = updateRequest.UpdateUserName,
            LastUpdateDate = updateRequest.UpdateDate
        };

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new NotFound();
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpsertAsync(ProductGTINCodeUpsertRequest upsertRequest)
    {
        const string updateQuery =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {ProductGTINCodesTableName}
                WHERE {ProductIdColumnName} = @productId
                AND {CodeTypeColumnName} = @CodeType)
            BEGIN
                INSERT INTO {ProductGTINCodesTableName} (
                    {ProductIdColumnName}, {CodeTypeColumnName}, {ValueColumnName}, {CodeTypeAsTextColumnName},
                    {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName})
                VALUES (@productId, @CodeType, @CodeTypeAsText, @Value,
                    @UpsertUserName, @UpsertDate, @UpsertUserName, @UpsertDate)
            END
            ELSE
            BEGIN
                UPDATE {ProductGTINCodesTableName}
                SET {CodeTypeAsTextColumnName} = @CodeTypeAsText,
                    {ValueColumnName} = @Value,
                    {LastUpdateUserNameColumnName} = @UpsertUserName,
                    {LastUpdateDateColumnName} = @UpsertDate
                WHERE {ProductIdColumnName} = @productId
                AND {CodeTypeColumnName} = @CodeType
            END
            """;

        var parameters = new
        {
            productId = upsertRequest.ProductId,

            CodeType = upsertRequest.CodeType.Value,

            CodeTypeAsText = upsertRequest.CodeTypeAsText,
            Value = upsertRequest.Value,

            UpsertUserName = upsertRequest.UpsertUserName,
            UpsertDate = upsertRequest.UpsertDate,
        };

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(int productId, ProductGTINCodeType productGTINCodeType)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {ProductGTINCodesTableName}

            WHERE {ProductIdColumnName} = @productId
            AND {CodeTypeColumnName} = @CodeType
            """;

        var parameters = new
        {
            productId = productId,
            CodeType = productGTINCodeType.Value,
        };

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(deleteQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new NotFound();
    }

    private static List<ProductGTINCode> MapRangeFromDAO(IEnumerable<ProductGTINCodeDAO> dao)
    {
        List<ProductGTINCode> output = new();

        foreach (ProductGTINCodeDAO productGTINCodeDAO in dao)
        {
            ProductGTINCode productGTINCode = MapFromDAO(productGTINCodeDAO);

            output.Add(productGTINCode);
        }

        return output;
    }

    private static ProductGTINCode MapFromDAO(ProductGTINCodeDAO dao)
    {
        ProductGTINCodeType? productGTINCodeType = ProductGTINCodeType.FromValue(dao.CodeType);

#pragma warning disable IDE0270 // Use coalesce expression
        if (productGTINCodeType is null)
        {
            throw new ArgumentException($"Invalid CodeType value: {dao.CodeType} for ProductGTINCodeDAO with ProductId: {dao.ProductId}");
        }
#pragma warning restore IDE0270 // Use coalesce expression

        return new ProductGTINCode
        {
            ProductId = dao.ProductId,
            CodeType = productGTINCodeType,
            CodeTypeAsText = dao.CodeTypeAsText,
            Value = dao.Value,
            CreateUserName = dao.CreateUserName,
            CreateDate = dao.CreateDate,
            LastUpdateUserName = dao.LastUpdateUserName,
            LastUpdateDate = dao.LastUpdateDate
        };
    }
}