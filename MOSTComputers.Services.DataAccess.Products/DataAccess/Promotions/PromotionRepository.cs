using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Contracts;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions;
internal sealed class PromotionRepository : IPromotionRepository
{
    public PromotionRepository(
        [FromKeyedServices(ConfigureServices.ReadOnlyDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<Promotion>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {PromotionsTableName} WITH (NOLOCK)
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<Promotion> promotions = await dbConnection.QueryAsync<Promotion>(
            getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return promotions.AsList();
    }

    public async Task<List<Promotion>> GetAllActiveAsync()
    {
        const string getAllActiveQuery =
            $"""
            SELECT * FROM {PromotionsTableName} WITH (NOLOCK)
            WHERE {ActiveColumnName} = 1;
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<Promotion> promotions = await dbConnection.QueryAsync<Promotion>(
            getAllActiveQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return promotions.AsList();
    }

    public async Task<List<IGrouping<int?, Promotion>>> GetAllForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllForSelectionOfProductsQuery =
            $"""
            SELECT * FROM {PromotionsTableName} WITH (NOLOCK)
            WHERE {ProductIdColumnName} IN @productIds
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<Promotion> promotionsForProducts = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return dbConnection.QueryAsync<Promotion>(
                    getAllForSelectionOfProductsQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        transactionScope.Complete();

        return promotionsForProducts.GroupBy(x => x.ProductId)
            .AsList();
    }

    public async Task<List<IGrouping<int?, Promotion>>> GetAllActiveForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllActiveForSelectionOfProductsQuery =
            $"""
            SELECT * FROM {PromotionsTableName} WITH (NOLOCK)
            WHERE {ActiveColumnName} = 1
            AND {ProductIdColumnName} IN @productIds
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<Promotion> promotionsForProducts = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return dbConnection.QueryAsync<Promotion>(
                    getAllActiveForSelectionOfProductsQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        transactionScope.Complete();

        return promotionsForProducts.GroupBy(x => x.ProductId)
            .AsList();
    }

    public async Task<List<Promotion>> GetAllForProductAsync(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {PromotionsTableName} WITH (NOLOCK)
            WHERE {ProductIdColumnName} = @productId;
            """;

        var parameters = new { productId };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<Promotion> promotions = await dbConnection.QueryAsync<Promotion>(
            getAllForProductQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return promotions.AsList();
    }

    public async Task<List<Promotion>> GetAllActiveForProductAsync(int productId)
    {
        const string getActiveForProductQuery =
            $"""
            SELECT * FROM {PromotionsTableName} WITH (NOLOCK)
            WHERE {ProductIdColumnName} = @productId
            AND {ActiveColumnName} = 1;
            """;

        var parameters = new { productId };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<Promotion> promotion = await dbConnection.QueryAsync<Promotion>(
            getActiveForProductQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return promotion.AsList();
    }
}