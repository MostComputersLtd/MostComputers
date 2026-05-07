using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionContentsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;

internal sealed class GroupPromotionContentsRepository : IGroupPromotionContentsRepository
{
    public GroupPromotionContentsRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
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

    public async Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(GroupPromotionContentCreateRequest createRequest)
    {
        const string query =
            $"""
            DECLARE @TempIdTable TABLE (Id INT);

            INSERT INTO {GroupPromotionContentsTableName} (
                {NameColumnName},
                {GroupIdColumnName},
                {HtmlContentColumnName},
                {StartDateColumnName},
                {ExpireDateColumnName},
                {DisplayOrderColumnName},
                {DateModifiedColumnName},
                {DisabledColumnName},
                {RestrictedColumnName},
                {MemberOfDefaultGroupColumnName},
                {DefaultGroupPriorityColumnName})
                OUTPUT INSERTED.Id INTO @TempIdTable (Id)

            VALUES (@Name, @GroupId, @HtmlContent, @StartDate, @ExpirationDate,
                @DisplayOrder, @DateModified, @Disabled, @Restricted,
                @MemberOfDefaultGroup, @DefaultGroupPriority)

            SELECT TOP 1 Id FROM @TempIdTable;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            Name = createRequest.Name,
            GroupId = createRequest.GroupId,
            HtmlContent = createRequest.HtmlContent,
            StartDate = createRequest.StartDate,
            ExpirationDate = createRequest.ExpirationDate,
            DisplayOrder = createRequest.DisplayOrder,
            DateModified = createRequest.DateModified,
            Disabled = createRequest.Disabled,
            Restricted = createRequest.Restricted,
            MemberOfDefaultGroup = createRequest.MemberOfDefaultGroup,
            DefaultGroupPriority = createRequest.DefaultGroupPriority,
        };

        int? insertedId = await connection.ExecuteScalarAsync<int?>(query, parameters, commandType: CommandType.Text);

        if (insertedId != null && insertedId > 0)
        {
            return insertedId.Value;
        }

        return new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(GroupPromotionContentUpdateRequest updateRequest)
    {
        const string query =
            $"""
            UPDATE {GroupPromotionContentsTableName}
                SET {NameColumnName} = @Name,
                    {GroupIdColumnName} = @GroupId,
                    {HtmlContentColumnName} = @HtmlContent,
                    {StartDateColumnName} = @StartDate,
                    {ExpireDateColumnName} = @ExpirationDate,
                    {DisplayOrderColumnName} = @DisplayOrder,
                    {DateModifiedColumnName} = @DateModified,
                    {DisabledColumnName} = @Disabled,
                    {RestrictedColumnName} = @Restricted,
                    {MemberOfDefaultGroupColumnName} = @MemberOfDefaultGroup,
                    {DefaultGroupPriorityColumnName} = @DefaultGroupPriority

                WHERE {IdColumnName} = @Id;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            Id = updateRequest.Id,
            Name = updateRequest.Name,
            GroupId = updateRequest.GroupId,
            HtmlContent = updateRequest.HtmlContent,
            StartDate = updateRequest.StartDate,
            ExpirationDate = updateRequest.ExpirationDate,
            DisplayOrder = updateRequest.DisplayOrder,
            DateModified = updateRequest.DateModified,
            Disabled = updateRequest.Disabled,
            Restricted = updateRequest.Restricted,
            MemberOfDefaultGroup = updateRequest.MemberOfDefaultGroup,
            DefaultGroupPriority = updateRequest.DefaultGroupPriority,
        };

        int rowsAffected = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        if (rowsAffected > 0)
        {
            return new Success();
        }

        return new NotFound();
    }
}