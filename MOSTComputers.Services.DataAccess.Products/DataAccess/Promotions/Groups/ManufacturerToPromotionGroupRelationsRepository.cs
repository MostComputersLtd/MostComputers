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

using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ManufacturerToPromotionGroupRelationsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;
internal sealed class ManufacturerToPromotionGroupRelationsRepository : IManufacturerToPromotionGroupRelationsRepository
{
    public ManufacturerToPromotionGroupRelationsRepository(
        [FromKeyedServices(ConfigureServices.LocalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ManufacturerToPromotionGroupRelation>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT * FROM {ManufacturerToPromotionGroupRelationsTableName}
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ManufacturerToPromotionGroupRelation> manufacturerToPromotionGroupRelations
            = await connection.QueryAsync<ManufacturerToPromotionGroupRelation>(query, new { }, commandType: CommandType.Text);

        return manufacturerToPromotionGroupRelations.AsList();
    }

    public async Task<ManufacturerToPromotionGroupRelation?> GetByManufacturerIdAsync(int manufacturerId)
    {
        const string query =
            $"""
            SELECT TOP 1 * FROM {ManufacturerToPromotionGroupRelationsTableName}
            WHERE {ManufacturerIdColumnName} = @manufacturerId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            manufacturerId = manufacturerId,
        };

        ManufacturerToPromotionGroupRelation? manufacturerToPromotionGroupRelation
            = await connection.QueryFirstOrDefaultAsync<ManufacturerToPromotionGroupRelation>(query, parameters, commandType: CommandType.Text);

        return manufacturerToPromotionGroupRelation;
    }

    public async Task<ManufacturerToPromotionGroupRelation?> GetByPromotionGroupIdAsync(int promotionGroupId)
    {
        const string query =
            $"""
            SELECT TOP 1 * FROM {ManufacturerToPromotionGroupRelationsTableName}
            WHERE {PromotionGroupIdColumnName} = @promotionGroupId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            promotionGroupId = promotionGroupId,
        };

        ManufacturerToPromotionGroupRelation? manufacturerToPromotionGroupRelation
            = await connection.QueryFirstOrDefaultAsync<ManufacturerToPromotionGroupRelation>(query, parameters, commandType: CommandType.Text);

        return manufacturerToPromotionGroupRelation;
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpsertByManufacturerIdAsync(ManufacturerToPromotionGroupRelationUpsertRequest upsertRequest)
    {
        const string query =
            $"""
            IF EXISTS (
                SELECT 1 FROM {ManufacturerToPromotionGroupRelationsTableName}
                WHERE {ManufacturerIdColumnName} = @manufacturerId
            )
            BEGIN
                UPDATE {ManufacturerToPromotionGroupRelationsTableName}
                SET {PromotionGroupIdColumnName} = @promotionGroupId
                WHERE {ManufacturerIdColumnName} = @manufacturerId;
            END
            ELSE
            BEGIN
                INSERT INTO {ManufacturerToPromotionGroupRelationsTableName} (
                    {ManufacturerIdColumnName},
                    {PromotionGroupIdColumnName})
                VALUES (
                    @manufacturerId,
                    @promotionGroupId
                );
            END
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            manufacturerId = upsertRequest.ManufacturerId,
            promotionGroupId = upsertRequest.PromotionGroupId,
        };

        int rowsAffected = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<bool> DeleteByManufacturerIdAsync(int manufacturerId)
    {
        const string query =
            $"""
            DELETE FROM {ManufacturerToPromotionGroupRelationsTableName}
            WHERE {ManufacturerIdColumnName} = @manufacturerId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            manufacturerId = manufacturerId,
        };

        int rowsAffected = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByPromotionGroupIdAsync(int promotionGroupId)
    {
        const string query =
            $"""
            DELETE FROM {ManufacturerToPromotionGroupRelationsTableName}
            WHERE {PromotionGroupIdColumnName} = @promotionGroupId;
            """;

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            promotionGroupId = promotionGroupId,
        };

        int rowsAffected = await connection.ExecuteAsync(query, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }
}