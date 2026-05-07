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
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionGroupsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;
internal sealed class PromotionGroupsRepository : IPromotionGroupsRepository
{
    public PromotionGroupsRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
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

    public async Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(PromotionGroupCreateRequest createRequest)
    {
        const string query =
            $"""
            DECLARE @TempIdTable TABLE (Id INT);

            INSERT INTO {PromotionGroupsTableName} (
                {NameColumnName},
                {HeaderColumnName},
                {LogoColumnName},
                {LogoContentTypeColumnName},
                {DisplayOrderColumnName},
                {IsDefaultColumnName},
                {ShowEmptyForLoggedColumnName},
                {ShowEmptyForNonLoggedColumnName})
            OUTPUT INSERTED.Id INTO @TempIdTable.Id
            VALUES (@Name, @Header, @LogoImage, @LogoContentType, @DisplayOrder, @IsDefault, @ShowEmptyForLogged, @ShowEmptyForNonLogged)

            SELECT TOP 1 FROM @TempIdTable.Id;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
             Name = createRequest.Name,
             Header = createRequest.Header,
             LogoImage = createRequest.LogoImage,
             LogoContentType = createRequest.LogoContentType,
             DisplayOrder = createRequest.DisplayOrder,
             IsDefault = createRequest.IsDefault,
             ShowEmptyForLogged = createRequest.ShowEmptyForLogged,
             ShowEmptyForNonLogged = createRequest.ShowEmptyForNonLogged,
        };

        int? insertedId = await connection.ExecuteScalarAsync<int?>(query, parameters, commandType: CommandType.Text);

        if (insertedId != null && insertedId > 0)
        {
            return insertedId.Value;
        }

        return new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(PromotionGroupUpdateRequest updateRequest)
    {
        const string query =
            $"""
            UPDATE {PromotionGroupsTableName}
                SET {NameColumnName} = @Name,
                    {HeaderColumnName} = @Header,
                    {LogoColumnName} = @LogoImage,
                    {LogoContentTypeColumnName} = @LogoContentType,
                    {DisplayOrderColumnName} = @DisplayOrder,
                    {IsDefaultColumnName} = @IsDefault,
                    {ShowEmptyForLoggedColumnName} = @ShowEmptyForLogged,
                    {ShowEmptyForNonLoggedColumnName} = @ShowEmptyForNonLogged

                WHERE {IdColumnName} = @Id;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
             Id = updateRequest.Id,
             Name = updateRequest.Name,
             Header = updateRequest.Header,
             LogoImage = updateRequest.LogoImage,
             LogoContentType = updateRequest.LogoContentType,
             DisplayOrder = updateRequest.DisplayOrder,
             IsDefault = updateRequest.IsDefault,
             ShowEmptyForLogged = updateRequest.ShowEmptyForLogged,
             ShowEmptyForNonLogged = updateRequest.ShowEmptyForNonLogged,
        };

        int rowsAffected = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        if (rowsAffected > 0)
        {
            return new Success();
        }

        return new NotFound();
    }
}