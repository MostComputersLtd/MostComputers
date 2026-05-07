using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionImagesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;
internal sealed class GroupPromotionImagesRepository : IGroupPromotionImagesRepository
{
    public GroupPromotionImagesRepository(
       [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<GroupPromotionImageWithoutFile>> GetAllWithoutFilesAsync()
    {
        const string query =
            $"""
            SELECT {IdColumnName}, {PromotionIdColumnName}, {ContentTypeColumnName} FROM {GroupPromotionImagesTableName} WITH (NOLOCK)
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new { };

        IEnumerable<GroupPromotionImageWithoutFile> promotionImages = await connection.QueryAsync<GroupPromotionImageWithoutFile>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionImages.AsList();
    }

    public async Task<List<GroupPromotionImage>> GetAllInPromotionAsync(int groupPromotionId)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionImagesTableName} WITH (NOLOCK)
            WHERE {PromotionIdColumnName} = @groupPromotionId
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new { groupPromotionId = groupPromotionId };

        IEnumerable<GroupPromotionImage> promotionImages = await connection.QueryAsync<GroupPromotionImage>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionImages.AsList();
    }

    public async Task<List<GroupPromotionImageWithoutFile>> GetAllInPromotionWithoutFilesAsync(int groupPromotionId)
    {
        const string query =
            $"""
            SELECT {IdColumnName}, {PromotionIdColumnName}, {ContentTypeColumnName} FROM {GroupPromotionImagesTableName} WITH (NOLOCK)
            WHERE {PromotionIdColumnName} = @groupPromotionId
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new { groupPromotionId = groupPromotionId };

        IEnumerable<GroupPromotionImageWithoutFile> promotionImages = await connection.QueryAsync<GroupPromotionImageWithoutFile>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionImages.AsList();
    }

    public async Task<List<GroupPromotionImage>> GetByIdsAsync(IEnumerable<int> ids)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionImagesTableName} WITH (NOLOCK)
            WHERE {IdColumnName} IN @ids;
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new { ids = ids };

        IEnumerable<GroupPromotionImage> promotionImages = await connection.QueryAsync<GroupPromotionImage>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionImages.AsList();
    }

    public async Task<List<GroupPromotionImageWithoutFile>> GetByIdsWithoutFilesAsync(IEnumerable<int> ids)
    {
        const string query =
            $"""
            SELECT {IdColumnName}, {PromotionIdColumnName}, {ContentTypeColumnName} FROM {GroupPromotionImagesTableName} WITH (NOLOCK)
            WHERE {IdColumnName} IN @ids;
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new { ids = ids };

        IEnumerable<GroupPromotionImageWithoutFile> promotionImages = await connection.QueryAsync<GroupPromotionImageWithoutFile>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionImages.AsList();
    }

    public async Task<GroupPromotionImage?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 * FROM {GroupPromotionImagesTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id;
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = id,
        };

        GroupPromotionImage? promotionImage = await connection.QueryFirstOrDefaultAsync<GroupPromotionImage>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionImage;
    }

    public async Task<GroupPromotionImageWithoutFile?> GetByIdWithoutFileAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 {IdColumnName}, {PromotionIdColumnName}, {ContentTypeColumnName} FROM {GroupPromotionImagesTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id;
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = id,
        };

        GroupPromotionImageWithoutFile? promotionImage = await connection.QueryFirstOrDefaultAsync<GroupPromotionImageWithoutFile>(query, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return promotionImage;
    }

    public async Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(GroupPromotionImageCreateRequest createRequest)
    {
        const string query =
            $"""
            DECLARE @TempIdTable TABLE (Id INT);

            INSERT INTO {GroupPromotionImagesTableName} (
                {PromotionIdColumnName},
                {ImageColumnName},
                {ContentTypeColumnName})

            OUTPUT INSERTED.Id INTO @TempIdTable (Id)
            VALUES (@PromotionId, @Image, @ContentType);

            SELECT TOP 1 Id FROM @TempIdTable;
            """;

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            PromotionId = createRequest.PromotionId,
            Image = createRequest.Image,
            ContentType = createRequest.ContentType,
        };

        int? insertedId = await sqlConnection.ExecuteScalarAsync<int?>(query, parameters, commandType: CommandType.Text);

        if (insertedId != null && insertedId > 0)
        {
            return insertedId.Value;
        }

        return new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(GroupPromotionImageUpdateRequest updateRequest)
    {
        const string query =
            $"""
            UPDATE {GroupPromotionImagesTableName}
            SET {PromotionIdColumnName} = @PromotionId,
                {ImageColumnName} = @Image,
                {ContentTypeColumnName} = @ContentType

            WHERE {IdColumnName} = @Id;
            """;

        var parameters = new
        {
            Id = updateRequest.Id,
            PromotionId = updateRequest.PromotionId,
            Image = updateRequest.Image,
            ContentType = updateRequest.ContentType,
        };

        SqlConnection sqlConnection = new (_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new NotFound();
    }

    public async Task<OneOf<Success, NotFound>> DeleteAsync(int id)
    {
        const string query =
            $"""
            DELETE FROM {GroupPromotionImagesTableName}
            WHERE {IdColumnName} = @Id;
            """;

        var parameters = new
        {
            Id = id,
        };

        SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await sqlConnection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new NotFound();
    }
}