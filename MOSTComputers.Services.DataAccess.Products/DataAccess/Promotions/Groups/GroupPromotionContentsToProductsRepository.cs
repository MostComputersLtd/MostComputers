using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using System.Data;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionContentsToProductsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;

internal sealed class GroupPromotionContentsToProductsRepository : IGroupPromotionContentsToProductsRepository
{
    private readonly struct PromotionContentsToProductsBinding
    {
        public required int PromotionId { get; init; }
        public required int ProductId { get; init; }
    }

    public GroupPromotionContentsToProductsRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<Dictionary<int, List<int>>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName} AS ProductId, {PromotionIdColumnName} AS PromotionId
            FROM {GroupPromotionContentsToProductsTableName} WITH (NOLOCK)
            ORDER BY {ProductIdColumnName}, {PromotionIdColumnName};
            """;

        var parameters = new
        {
        };

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionContentsToProductsBinding> promotionContentsToProductBindings
            = await connection.QueryAsync<PromotionContentsToProductsBinding>(query, parameters, commandType: CommandType.Text);

        Dictionary<int, List<int>> promotionIdsForProducts = new();

        int? currentProductId = null;

        List<int> currentPromotionIds = [];

        foreach (PromotionContentsToProductsBinding binding in promotionContentsToProductBindings)
        {
            if (currentProductId != binding.ProductId)
            {
                if (currentProductId is not null)
                {
                    promotionIdsForProducts.Add(currentProductId.Value, currentPromotionIds);
                }

                currentProductId = binding.ProductId;
                
                currentPromotionIds = [binding.PromotionId];
            }
            else
            {
                currentPromotionIds.Add(binding.PromotionId);
            }
        }

        if (currentProductId is not null)
        {
            promotionIdsForProducts.Add(currentProductId.Value, currentPromotionIds);
        }

        return promotionIdsForProducts;
    }

    public async Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId)
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName} FROM {GroupPromotionContentsToProductsTableName} WITH (NOLOCK)
            WHERE {PromotionIdColumnName} = @promotionId;
            """;

        var parameters = new
        {
            promotionId = promotionId,
        };

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<int> productIds = await connection.QueryAsync<int>(query, parameters, commandType: CommandType.Text);

        return productIds.AsList();
    }

    public async Task<Dictionary<int, List<int>>> GetAllPromotionIdsBoundToProductsAsync(IEnumerable<int> productIds)
    {
        const string query =
            $"""
            SELECT {ProductIdColumnName} AS ProductId, {PromotionIdColumnName} AS PromotionId
            FROM {GroupPromotionContentsToProductsTableName} WITH (NOLOCK)
            WHERE {ProductIdColumnName} IN @productIds
            ORDER BY {ProductIdColumnName}, {PromotionIdColumnName};
            """;

        var parameters = new
        {
            productIds = productIds,
        };

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionContentsToProductsBinding> promotionContentsToProductBindings
            = await connection.QueryAsync<PromotionContentsToProductsBinding>(query, parameters, commandType: CommandType.Text);

        Dictionary<int, List<int>> promotionIdsForProducts = new();

        int? currentProductId = null;

        List<int> currentPromotionIds = [];

        foreach (PromotionContentsToProductsBinding binding in promotionContentsToProductBindings)
        {
            if (currentProductId != binding.ProductId)
            {
                if (currentProductId is not null)
                {
                    promotionIdsForProducts.Add(currentProductId.Value, currentPromotionIds);
                }

                currentProductId = binding.ProductId;
                
                currentPromotionIds = [binding.PromotionId];
            }
            else
            {
                currentPromotionIds.Add(binding.PromotionId);
            }
        }

        if (currentProductId is not null)
        {
            promotionIdsForProducts.Add(currentProductId.Value, currentPromotionIds);
        }

        return promotionIdsForProducts;
    }

    public async Task<List<int>> GetAllPromotionIdsBoundToProductAsync(int productId)
    {
        const string query =
            $"""
            SELECT {PromotionIdColumnName} FROM {GroupPromotionContentsToProductsTableName} WITH (NOLOCK)
            WHERE {ProductIdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId = productId,
        };

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<int> promotionIds = await connection.QueryAsync<int>(query, parameters, commandType: CommandType.Text);

        return promotionIds.AsList();
    }

    public async Task UpsertAllAsync(int promotionId, List<int>? relatedProductIds)
    {
        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        await connection.OpenAsync();

        using SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            if (relatedProductIds == null || relatedProductIds.Count == 0)
            {
                const string deleteAllQuery =
                    $"""
                    DELETE FROM {GroupPromotionContentsToProductsTableName}
                    WHERE {PromotionIdColumnName} = @promotionId;
                    """;

                await connection.ExecuteAsync(deleteAllQuery, new { promotionId }, transaction: transaction);

                transaction.Commit();

                return;
            }

            DynamicParameters parameters = new();

            parameters.Add("promotionId", promotionId);

            List<string> valueEntries = new(relatedProductIds.Count);

            for (int i = 0; i < relatedProductIds.Count; i++)
            {
                string paramName = $"p{i}";

                parameters.Add(paramName, relatedProductIds[i], DbType.Int32);

                valueEntries.Add($"(@{paramName})");
            }

            string valuesClause = string.Join(", ", valueEntries);

            string query =
                $"""
                DECLARE @NewProducts TABLE (ProductId INT);

                INSERT INTO @NewProducts (ProductId)
                VALUES {valuesClause};

                DELETE FROM {GroupPromotionContentsToProductsTableName}
                WHERE {PromotionIdColumnName} = @promotionId
                    AND {ProductIdColumnName} NOT IN (SELECT ProductId FROM @NewProducts);

                INSERT INTO {GroupPromotionContentsToProductsTableName} ({PromotionIdColumnName}, {ProductIdColumnName})
                SELECT @promotionId, newProducts.ProductId
                FROM @NewProducts newProducts
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM {GroupPromotionContentsToProductsTableName} groupPromotionContentsToProducts
                    WHERE groupPromotionContentsToProducts.{PromotionIdColumnName} = @promotionId
                        AND groupPromotionContentsToProducts.{ProductIdColumnName} = newProducts.ProductId
                )
                """;

            await connection.ExecuteAsync(query, parameters, transaction: transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }
}