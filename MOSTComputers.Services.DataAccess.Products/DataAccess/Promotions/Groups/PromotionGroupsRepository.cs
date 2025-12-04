using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using System.Data;
using System.Transactions;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionGroupsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;
internal sealed class PromotionGroupsRepository : IPromotionGroupsRepository
{
    public PromotionGroupsRepository(
        [FromKeyedServices(ConfigureServices.ReadOnlyDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<PromotionGroup>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT * FROM {PromotionGroupsTableName} WITH (NOLOCK)
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionGroup> promotionGroups = await connection.QueryAsync<PromotionGroup>(query, new { }, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroups.AsList();
    }

    public async Task<PromotionGroup?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 * FROM {PromotionGroupsTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id;
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = id,
        };

        PromotionGroup? promotionGroup = await connection.QueryFirstOrDefaultAsync<PromotionGroup>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroup;
    }
}