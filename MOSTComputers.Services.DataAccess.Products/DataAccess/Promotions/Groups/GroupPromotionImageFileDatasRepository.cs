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

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.GroupPromotionImageFileDatasTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;
internal sealed class GroupPromotionImageFileDatasRepository : IGroupPromotionImageFileDatasRepository
{
    public GroupPromotionImageFileDatasRepository(
        [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<GroupPromotionImageFileData>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionImageFileDatasTableName}
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<GroupPromotionImageFileData> groupPromotionImageFileDatas
            = await connection.QueryAsync<GroupPromotionImageFileData>(query, new { }, commandType: CommandType.Text);

        return groupPromotionImageFileDatas.AsList();
    }

    public async Task<List<IGrouping<int, GroupPromotionImageFileData>>> GetAllInPromotionsAsync(List<int> promotionIds)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionImageFileDatasTableName}
            WHERE {PromotionIdColumnName} IN @promotionIds;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            promotionIds = promotionIds,
        };

        IEnumerable<GroupPromotionImageFileData> groupPromotionImageFileDatas
            = await connection.QueryAsync<GroupPromotionImageFileData>(query, parameters, commandType: CommandType.Text);

        return groupPromotionImageFileDatas
            .GroupBy(x => x.PromotionId!.Value)
            .AsList();
    }


    public async Task<List<GroupPromotionImageFileData>> GetAllInPromotionAsync(int promotionId)
    {
        const string query =
            $"""
            SELECT * FROM {GroupPromotionImageFileDatasTableName}
            WHERE {PromotionIdColumnName} = @promotionId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            promotionId = promotionId,
        };

        IEnumerable<GroupPromotionImageFileData> groupPromotionImageFileDatas
            = await connection.QueryAsync<GroupPromotionImageFileData>(query, parameters, commandType: CommandType.Text);

        return groupPromotionImageFileDatas.AsList();
    }

    public async Task<GroupPromotionImageFileData?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 * FROM {GroupPromotionImageFileDatasTableName}
            WHERE {IdColumnName} = @id;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = id,
        };

        GroupPromotionImageFileData? groupPromotionImageFileData
            = await connection.QueryFirstOrDefaultAsync<GroupPromotionImageFileData>(query, parameters, commandType: CommandType.Text);

        return groupPromotionImageFileData;
    }

    public async Task<GroupPromotionImageFileData?> GetByImageIdAsync(int imageId)
    {
        const string query =
            $"""
            SELECT TOP 1 * FROM {GroupPromotionImageFileDatasTableName}
            WHERE {ImageIdColumnName} = @imageId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            imageId = imageId,
        };

        GroupPromotionImageFileData? groupPromotionImageFileData
            = await connection.QueryFirstOrDefaultAsync<GroupPromotionImageFileData>(query, parameters, commandType: CommandType.Text);

        return groupPromotionImageFileData;
    }

    public async Task<OneOf<int, ImageFileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(GroupPromotionImageFileDataCreateRequest createRequest)
    {
        const string query =
            $"""
            DECLARE @NewIdTable TABLE (Id INT);

            IF NOT EXISTS (
                SELECT 1 FROM {GroupPromotionImageFileDatasTableName}
                WHERE {ImageIdColumnName} = @imageId
            )
            BEGIN
                INSERT INTO {GroupPromotionImageFileDatasTableName} ({ImageIdColumnName}, {PromotionIdColumnName}, {FileNameColumnName})
                OUTPUT INSERTED.Id INTO @NewIdTable (Id)
                VALUES (@imageId, @promotionId, @FileName);

                SELECT TOP 1 Id FROM @NewIdTable;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            promotionId = createRequest.PromotionId,
            imageId = createRequest.ImageId,
            FileName = createRequest.FileName,
        };

        int? newId = await connection.ExecuteScalarAsync<int>(query, parameters, commandType: CommandType.Text);

        if (newId == -1)
        {
            return new ImageFileAlreadyExistsResult
            {
                ExistingImageId = createRequest.ImageId,
            };
        }

        if (newId > 0) return newId.Value;

        return new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(GroupPromotionImageFileDataUpdateRequest updateRequest)
    {
        const string query =
            $"""
            UPDATE {GroupPromotionImageFileDatasTableName}
            SET {FileNameColumnName} = @fileName
            WHERE {IdColumnName} = @id;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = updateRequest.Id,
            fileName = updateRequest.NewFileName,
        };

        int affectedRows = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return affectedRows > 0 ? new Success() : new NotFound();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query =
            $"""
            DELETE FROM {GroupPromotionImageFileDatasTableName}
            WHERE {IdColumnName} = @id;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            id = id,
        };

        int affectedRows = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return affectedRows > 0;
    }

    public async Task<bool> DeleteByImageIdAsync(int imageId)
    {
        const string query =
            $"""
            DELETE FROM {GroupPromotionImageFileDatasTableName}
            WHERE {ImageIdColumnName} = @imageId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            imageId = imageId,
        };

        int affectedRows = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return affectedRows > 0;
    }
}