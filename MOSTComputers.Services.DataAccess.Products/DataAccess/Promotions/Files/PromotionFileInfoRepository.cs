using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionFiles;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionFilesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files;
internal sealed class PromotionFileInfoRepository : IPromotionFileInfoRepository
{
    public PromotionFileInfoRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<PromotionFileInfo>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {NameColumnName},
                {ActiveColumnName} AS {ActiveColumnAlias},
                {ValidFromDateColumnName} AS {ValidFromDateColumnAlias},
                {ValidToDateColumnName} AS {ValidToDateColumnAlias},
                {FileNameColumnName},
                {DescriptionColumnName},
                {RelatedProductsColumnName},
                {CreateUserNameColumnName} AS {CreateUserNameColumnAlias},
                {CreateDateColumnName} AS {CreateDateColumnAlias},
                {LastUpdateUserNameColumnName} AS {LastUpdateUserNameColumnAlias},
                {LastUpdateDateColumnName} AS {LastUpdateDateColumnAlias}
            FROM {PromotionFilesTableName}
            """;

        var parameters = new { };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionFileInfo> data = await dbConnection.QueryAsync<PromotionFileInfo>(getAllQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<PromotionFileInfo>> GetAllByActivityAsync(bool active = true)
    {
        const string getAllByActivityQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {NameColumnName},
                {ActiveColumnName} AS {ActiveColumnAlias},
                {ValidFromDateColumnName} AS {ValidFromDateColumnAlias},
                {ValidToDateColumnName} AS {ValidToDateColumnAlias},
                {FileNameColumnName},
                {DescriptionColumnName},
                {RelatedProductsColumnName},
                {CreateUserNameColumnName} AS {CreateUserNameColumnAlias},
                {CreateDateColumnName} AS {CreateDateColumnAlias},
                {LastUpdateUserNameColumnName} AS {LastUpdateUserNameColumnAlias},
                {LastUpdateDateColumnName} AS {LastUpdateDateColumnAlias}
            FROM {PromotionFilesTableName}
            WHERE {ActiveColumnName} = @active;
            """;

        var parameters = new { active };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionFileInfo> data = await dbConnection.QueryAsync<PromotionFileInfo>(getAllByActivityQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<PromotionFileInfo>> GetByIdsAsync(IEnumerable<int> ids)
    {
        const string getByIdQuery =
            $"""
            SELECT {IdColumnName} AS {IdColumnAlias},
                {NameColumnName},
                {ActiveColumnName} AS {ActiveColumnAlias},
                {ValidFromDateColumnName} AS {ValidFromDateColumnAlias},
                {ValidToDateColumnName} AS {ValidToDateColumnAlias},
                {FileNameColumnName},
                {DescriptionColumnName},
                {RelatedProductsColumnName},
                {CreateUserNameColumnName} AS {CreateUserNameColumnAlias},
                {CreateDateColumnName} AS {CreateDateColumnAlias},
                {LastUpdateUserNameColumnName} AS {LastUpdateUserNameColumnAlias},
                {LastUpdateDateColumnName} AS {LastUpdateDateColumnAlias}
            FROM {PromotionFilesTableName}
            WHERE {IdColumnName} IN @ids;
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<PromotionFileInfo> promotionFileInfos = await ExecuteListQueryWithParametersInChunksAsync(
            idsChunk =>
            {
                var parameters = new
                {
                    ids = idsChunk
                };

                return dbConnection.QueryAsync<PromotionFileInfo>(getByIdQuery, parameters, commandType: CommandType.Text);
            },
            ids.AsList());

        return promotionFileInfos;
    }

    public async Task<PromotionFileInfo?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 {IdColumnName} AS {IdColumnAlias},
                {NameColumnName},
                {ActiveColumnName} AS {ActiveColumnAlias},
                {ValidFromDateColumnName} AS {ValidFromDateColumnAlias},
                {ValidToDateColumnName} AS {ValidToDateColumnAlias},
                {FileNameColumnName},
                {DescriptionColumnName},
                {RelatedProductsColumnName},
                {CreateUserNameColumnName} AS {CreateUserNameColumnAlias},
                {CreateDateColumnName} AS {CreateDateColumnAlias},
                {LastUpdateUserNameColumnName} AS {LastUpdateUserNameColumnAlias},
                {LastUpdateDateColumnName} AS {LastUpdateDateColumnAlias}
            FROM {PromotionFilesTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<PromotionFileInfo>(getByIdQuery, parameters, commandType: CommandType.Text);
    }

    public async Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(PromotionFileInfoCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);

            INSERT INTO {PromotionFilesTableName} ({NameColumnName}, {ActiveColumnName}, {ValidFromDateColumnName},
                {ValidToDateColumnName}, {FileNameColumnName}, {DescriptionColumnName}, {RelatedProductsColumnName},
                {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName})
            OUTPUT INSERTED.{IdColumnName} INTO @InsertedIdTable
            VALUES (@Name, @Active, @ValidFrom, @ValidTo, @FileName, @Description,
                @RelatedProducts, @CreateUserName, @CreateDate, @LastUpdateUserName, @LastUpdateDate)

            SELECT TOP 1 Id FROM @InsertedIdTable;
            """;

        var parameters = new
        {
            createRequest.Name,
            createRequest.Active,
            createRequest.ValidFrom,
            createRequest.ValidTo,
            createRequest.FileName,
            createRequest.Description,
            RelatedProducts = createRequest.RelatedProductsString,
            createRequest.CreateUserName,
            createRequest.CreateDate,
            createRequest.LastUpdateUserName,
            createRequest.LastUpdateDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? id = await dbConnection.ExecuteScalarAsync<int>(insertQuery, parameters, commandType: CommandType.Text);

        return id > 0 ? id.Value : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpdateAsync(PromotionFileInfoUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {PromotionFilesTableName}
            SET
                {NameColumnName} = @Name,
                {ActiveColumnName} = @Active,
                {ValidFromDateColumnName} = @ValidFrom,
                {ValidToDateColumnName} = @ValidTo,
                {FileNameColumnName} = @FileName,
                {DescriptionColumnName} = @Description,
                {RelatedProductsColumnName} = @RelatedProducts,
                {LastUpdateUserNameColumnName} = @LastUpdateUserId,
                {LastUpdateDateColumnName} = @LastUpdateDate
            
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.Name,
            updateRequest.Active,
            updateRequest.ValidFrom,
            updateRequest.ValidTo,
            updateRequest.FileName,
            updateRequest.Description,
            RelatedProducts = updateRequest.RelatedProductsString,
            LastUpdateUserId = updateRequest.LastUpdateUserName,
            updateRequest.LastUpdateDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string deleteQuery =
            $"""
            DELETE
            FROM {PromotionFilesTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }
}