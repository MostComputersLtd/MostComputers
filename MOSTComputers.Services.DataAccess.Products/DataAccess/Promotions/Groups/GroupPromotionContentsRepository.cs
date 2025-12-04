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
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionContentsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;

internal sealed class GroupPromotionContentsRepository : IGroupPromotionContentsRepository
{
    public GroupPromotionContentsRepository(
        [FromKeyedServices(ConfigureServices.ReadOnlyDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<GroupPromotionContent>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionContentsTableName} WITH (NOLOCK)
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<GroupPromotionContent> promotionGroups = await connection.QueryAsync<GroupPromotionContent>(query, new { }, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroups.AsList();
    }

    public async Task<List<GroupPromotionContent>> GetAllActiveAsync()
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionContentsTableName} WITH (NOLOCK)
            WHERE {DisabledColumnName} = 0
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<GroupPromotionContent> promotionGroups = await connection.QueryAsync<GroupPromotionContent>(query, new { }, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroups.AsList();
    }

    public async Task<List<IGrouping<int, GroupPromotionContent>>> GetAllInGroupsAsync(List<int> groupIds)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionContentsTableName} WITH (NOLOCK)
            WHERE {GroupIdColumnName} IN @groupIds
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            groupIds = groupIds,
        };

        IEnumerable<GroupPromotionContent> promotionGroups = await connection.QueryAsync<GroupPromotionContent>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroups
            .GroupBy(x => x.GroupId!.Value)
            .AsList();
    }

    public async Task<List<IGrouping<int, GroupPromotionContent>>> GetAllActiveInGroupsAsync(List<int> groupIds)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionContentsTableName} WITH (NOLOCK)
            WHERE {DisabledColumnName} = 0
            AND {GroupIdColumnName} IN @groupIds
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            groupIds = groupIds,
        };

        IEnumerable<GroupPromotionContent> promotionGroups = await connection.QueryAsync<GroupPromotionContent>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroups
            .GroupBy(x => x.GroupId!.Value)
            .AsList();
    }

    public async Task<List<GroupPromotionContent>> GetAllInGroupAsync(int groupId)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionContentsTableName} WITH (NOLOCK)
            WHERE {GroupIdColumnName} = @groupId
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            groupId = groupId,
        };

        IEnumerable<GroupPromotionContent> promotionGroups = await connection.QueryAsync<GroupPromotionContent>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroups.AsList();
    }

    public async Task<List<GroupPromotionContent>> GetAllActiveInGroupAsync(int groupId)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionContentsTableName} WITH (NOLOCK)
            WHERE {DisabledColumnName} = 0
            AND {GroupIdColumnName} = @groupId
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            groupId = groupId,
        };

        IEnumerable<GroupPromotionContent> promotionGroups = await connection.QueryAsync<GroupPromotionContent>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionGroups.AsList();
    }

    public async Task<GroupPromotionContent?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 * FROM {GroupPromotionContentsTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id;
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = id,
        };

        GroupPromotionContent? promotionContent = await connection.QueryFirstOrDefaultAsync<GroupPromotionContent>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionContent;
    }
}