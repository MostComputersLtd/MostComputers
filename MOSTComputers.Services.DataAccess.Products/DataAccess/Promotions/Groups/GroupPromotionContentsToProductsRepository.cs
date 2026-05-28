using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionContentsToProductsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;

internal sealed class GroupPromotionContentsToProductsRepository : IGroupPromotionContentsToProductsRepository
{
    public GroupPromotionContentsToProductsRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

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
}