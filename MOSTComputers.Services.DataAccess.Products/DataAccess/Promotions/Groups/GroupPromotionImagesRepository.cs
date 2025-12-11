using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
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
}