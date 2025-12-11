using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.PromotionProductFileInfo;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionProductFiles;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PromotionProductFilesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files;
internal sealed class PromotionProductFileInfoRepository : IPromotionProductFileInfoRepository
{
    public PromotionProductFileInfoRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<IGrouping<int, PromotionProductFileInfo>>> GetAllForProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllForProductsQuery =
            $"""
            SELECT promotionProductFileInfos.{IdColumnName},
                promotionProductFileInfos.{ProductIdColumnName},
                promotionProductFileInfos.{PromotionFileIdColumnName} AS {PromotionFileIdColumnAlias},
                promotionProductFileInfos.{ValidFromDateColumnName} AS {ValidFromDateColumnAlias},
                promotionProductFileInfos.{ValidToDateColumnName} AS {ValidToDateColumnAlias},
                promotionProductFileInfos.{ActiveColumnName} AS {ActiveColumnAlias},
                promotionProductFileInfos.{ImagesAllIdColumnName},
                promotionProductFileInfos.{CreateUserNameColumnName} AS {CreateUserNameColumnAlias},
                promotionProductFileInfos.{CreateDateColumnName} AS {CreateDateColumnAlias},
                promotionProductFileInfos.{LastUpdateUserNameColumnName} AS {LastUpdateUserNameColumnAlias},
                promotionProductFileInfos.{LastUpdateDateColumnName} AS {LastUpdateDateColumnAlias},

                promotionFileInfos.{PromotionFilesTable.IdColumnName} AS {PromotionFilesTable.IdColumnAlias},
                promotionFileInfos.{PromotionFilesTable.NameColumnName},
                promotionFileInfos.{PromotionFilesTable.ActiveColumnName} AS {PromotionFilesTable.ActiveColumnAlias},
                promotionFileInfos.{PromotionFilesTable.ValidFromDateColumnName} AS {PromotionFilesTable.ValidFromDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.ValidToDateColumnName} AS {PromotionFilesTable.ValidToDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.FileNameColumnName},
                promotionFileInfos.{PromotionFilesTable.DescriptionColumnName},
                promotionFileInfos.{PromotionFilesTable.RelatedProductsColumnName},
                promotionFileInfos.{PromotionFilesTable.CreateUserNameColumnName} AS {PromotionFilesTable.CreateUserNameColumnAlias},
                promotionFileInfos.{PromotionFilesTable.CreateDateColumnName} AS {PromotionFilesTable.CreateDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.LastUpdateUserNameColumnName} AS {PromotionFilesTable.LastUpdateUserNameColumnAlias},
                promotionFileInfos.{PromotionFilesTable.LastUpdateDateColumnName} AS {PromotionFilesTable.LastUpdateDateColumnAlias}

            FROM {PromotionProductFilesTableName} promotionProductFileInfos

            LEFT JOIN {PromotionFilesTableName} promotionFileInfos
            ON promotionFileInfos.{PromotionFilesTable.IdColumnName} = promotionProductFileInfos.{PromotionFileIdColumnName}

            WHERE {ProductIdColumnName} IN @productIds;
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionProductFileInfoDAO> output = await ExecuteListQueryWithParametersInChunksAsync(
            idsChunk =>
            {
                var parameters = new { productIds = idsChunk };

                return dbConnection.QueryAsync<PromotionProductFileInfoDAO, PromotionFileInfo, PromotionProductFileInfoDAO>(
                    getAllForProductsQuery,
                    (promotionProductFileInfoDAO, promotionFileInfo) =>
                    {
                        promotionProductFileInfoDAO.PromotionFileInfo = promotionFileInfo;

                        return promotionProductFileInfoDAO;
                    },
                    parameters,
                    splitOn: $"{PromotionFilesTable.IdColumnAlias}",
                    commandType: CommandType.Text);
            },
            productIds.ToList());

        return MapAllFromDAO(output)
            .GroupBy(x => x.ProductId)
            .AsList();
    }

    public async Task<List<PromotionProductFileInfoForProductCountData>> GetCountOfAllForProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllForProductsQuery =
            $"""
            SELECT {ProductIdColumnName},
                COUNT(*) AS {CountColumnName}
            FROM {PromotionProductFilesTableName}
            WHERE {ProductIdColumnName} IN @productIds
            GROUP BY {ProductIdColumnName};
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<PromotionProductFileInfoForProductCountData> promotionProductFileCountData = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return dbConnection.QueryAsync<PromotionProductFileInfoForProductCountData>(
                    getAllForProductsQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        return promotionProductFileCountData;
    }

    public async Task<List<PromotionProductFileInfo>> GetAllForProductAsync(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT promotionProductFileInfos.{IdColumnName},
                promotionProductFileInfos.{ProductIdColumnName},
                promotionProductFileInfos.{PromotionFileIdColumnName} AS {PromotionFileIdColumnAlias},
                promotionProductFileInfos.{ValidFromDateColumnName} AS {ValidFromDateColumnAlias},
                promotionProductFileInfos.{ValidToDateColumnName} AS {ValidToDateColumnAlias},
                promotionProductFileInfos.{ActiveColumnName} AS {ActiveColumnAlias},
                promotionProductFileInfos.{ImagesAllIdColumnName},
                promotionProductFileInfos.{CreateUserNameColumnName} AS {CreateUserNameColumnAlias},
                promotionProductFileInfos.{CreateDateColumnName} AS {CreateDateColumnAlias},
                promotionProductFileInfos.{LastUpdateUserNameColumnName} AS {LastUpdateUserNameColumnAlias},
                promotionProductFileInfos.{LastUpdateDateColumnName} AS {LastUpdateDateColumnAlias},
            
                promotionFileInfos.{PromotionFilesTable.IdColumnName} AS {PromotionFilesTable.IdColumnAlias},
                promotionFileInfos.{PromotionFilesTable.NameColumnName},
                promotionFileInfos.{PromotionFilesTable.ActiveColumnName} AS {PromotionFilesTable.ActiveColumnAlias},
                promotionFileInfos.{PromotionFilesTable.ValidFromDateColumnName} AS {PromotionFilesTable.ValidFromDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.ValidToDateColumnName} AS {PromotionFilesTable.ValidToDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.FileNameColumnName},
                promotionFileInfos.{PromotionFilesTable.DescriptionColumnName},
                promotionFileInfos.{PromotionFilesTable.RelatedProductsColumnName},
                promotionFileInfos.{PromotionFilesTable.CreateUserNameColumnName} AS {PromotionFilesTable.CreateUserNameColumnAlias},
                promotionFileInfos.{PromotionFilesTable.CreateDateColumnName} AS {PromotionFilesTable.CreateDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.LastUpdateUserNameColumnName} AS {PromotionFilesTable.LastUpdateUserNameColumnAlias},
                promotionFileInfos.{PromotionFilesTable.LastUpdateDateColumnName} AS {PromotionFilesTable.LastUpdateDateColumnAlias}
            
            FROM {PromotionProductFilesTableName} promotionProductFileInfos
            
            LEFT JOIN {PromotionFilesTableName} promotionFileInfos
            ON promotionFileInfos.{PromotionFilesTable.IdColumnName} = promotionProductFileInfos.{PromotionFileIdColumnName}
            
            WHERE {ProductIdColumnName} = @productId;
            """;

        var parameters = new { productId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionProductFileInfoDAO> output = await dbConnection.QueryAsync<PromotionProductFileInfoDAO, PromotionFileInfo, PromotionProductFileInfoDAO>(
            getAllForProductQuery,
            (promotionProductFileInfoDAO, promotionFileInfo) =>
            {
                promotionProductFileInfoDAO.PromotionFileInfo = promotionFileInfo;

                return promotionProductFileInfoDAO;
            },
            parameters,
            splitOn: $"{PromotionFilesTable.IdColumnAlias}",
            commandType: CommandType.Text);

        return MapAllFromDAO(output);
    }

    public async Task<int> GetCountOfAllForProductAsync(int productId)
    {
        const string getCountOfAllForProductQuery =
            $"""
            SELECT COUNT(*)
            FROM {PromotionProductFilesTableName}
            WHERE {ProductIdColumnName} = @productId;
            """;

        var parameters = new { productId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int countOfAllInProduct = await dbConnection.QueryFirstAsync<int>(
            getCountOfAllForProductQuery, parameters, commandType: CommandType.Text);

        return countOfAllInProduct;
    }

    public async Task<bool> DoesExistForPromotionFileAsync(int promotionFileId)
    {
        const string getCountOfAllForProductQuery =
            $"""
            IF EXISTS (SELECT 1
            FROM {PromotionProductFilesTableName}
            WHERE {PromotionFileIdColumnName} = @promotionFileId)
            BEGIN
                SELECT 1;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;

        var parameters = new { promotionFileId };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int result = await dbConnection.QueryFirstAsync<int>(
            getCountOfAllForProductQuery, parameters, commandType: CommandType.Text);

        return result > 0;
    }

    public async Task<PromotionProductFileInfo?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 promotionProductFileInfos.{IdColumnName},
                promotionProductFileInfos.{ProductIdColumnName},
                promotionProductFileInfos.{PromotionFileIdColumnName} AS {PromotionFileIdColumnAlias},
                promotionProductFileInfos.{ValidFromDateColumnName} AS {ValidFromDateColumnAlias},
                promotionProductFileInfos.{ValidToDateColumnName} AS {ValidToDateColumnAlias},
                promotionProductFileInfos.{ActiveColumnName} AS {ActiveColumnAlias},
                promotionProductFileInfos.{ImagesAllIdColumnName},
                promotionProductFileInfos.{CreateUserNameColumnName} AS {CreateUserNameColumnAlias},
                promotionProductFileInfos.{CreateDateColumnName} AS {CreateDateColumnAlias},
                promotionProductFileInfos.{LastUpdateUserNameColumnName} AS {LastUpdateUserNameColumnAlias},
                promotionProductFileInfos.{LastUpdateDateColumnName} AS {LastUpdateDateColumnAlias},
            
                promotionFileInfos.{PromotionFilesTable.IdColumnName} AS {PromotionFilesTable.IdColumnAlias},
                promotionFileInfos.{PromotionFilesTable.NameColumnName},
                promotionFileInfos.{PromotionFilesTable.ActiveColumnName} AS {PromotionFilesTable.ActiveColumnAlias},
                promotionFileInfos.{PromotionFilesTable.ValidFromDateColumnName} AS {PromotionFilesTable.ValidFromDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.ValidToDateColumnName} AS {PromotionFilesTable.ValidToDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.FileNameColumnName},
                promotionFileInfos.{PromotionFilesTable.DescriptionColumnName},
                promotionFileInfos.{PromotionFilesTable.RelatedProductsColumnName},
                promotionFileInfos.{PromotionFilesTable.CreateUserNameColumnName} AS {PromotionFilesTable.CreateUserNameColumnAlias},
                promotionFileInfos.{PromotionFilesTable.CreateDateColumnName} AS {PromotionFilesTable.CreateDateColumnAlias},
                promotionFileInfos.{PromotionFilesTable.LastUpdateUserNameColumnName} AS {PromotionFilesTable.LastUpdateUserNameColumnAlias},
                promotionFileInfos.{PromotionFilesTable.LastUpdateDateColumnName} AS {PromotionFilesTable.LastUpdateDateColumnAlias}
            
            FROM {PromotionProductFilesTableName} promotionProductFileInfos
            
            LEFT JOIN {PromotionFilesTableName} promotionFileInfos
            ON promotionFileInfos.{PromotionFilesTable.IdColumnName} = promotionProductFileInfos.{PromotionFileIdColumnName}
            
            WHERE {IdColumnName} = @id;
            """;
        
        var parameters = new { id };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<PromotionProductFileInfoDAO> data = await dbConnection.QueryAsync<PromotionProductFileInfoDAO, PromotionFileInfo, PromotionProductFileInfoDAO>(
            getByIdQuery,
            (promotionProductFileInfoDAO, promotionFileInfo) =>
            {
                promotionProductFileInfoDAO.PromotionFileInfo = promotionFileInfo;

                return promotionProductFileInfoDAO;
            },
            parameters,
            splitOn: $"{PromotionFilesTable.IdColumnAlias}",
            commandType: CommandType.Text);

        PromotionProductFileInfoDAO? output = data.First();

        if (output is null) return null;

        return MapFromDAO(output);
    }

    public async Task<OneOf<int, UnexpectedFailureResult>> InsertAsync(PromotionProductFileInfoCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);
            
            INSERT INTO {PromotionProductFilesTableName} ({ProductIdColumnName}, {PromotionFileIdColumnName},
                {ActiveColumnName}, {ValidFromDateColumnName}, {ValidToDateColumnName}, {ImagesAllIdColumnName},
                {CreateUserNameColumnName}, {CreateDateColumnName}, {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName})
            OUTPUT INSERTED.{IdColumnName} INTO @InsertedIdTable
            VALUES (@ProductId, @PromotionFileId, @Active, @ValidFrom, @ValidTo, @ImagesAllId,
                @CreateUserName, @CreateDate, @LastUpdateUserName, @LastUpdateDate)

            SELECT TOP 1 Id FROM @InsertedIdTable;
            """;

        var parameters = new
        {
            createRequest.ProductId,
            PromotionFileId = createRequest.PromotionFileInfoId,
            createRequest.Active,
            createRequest.ValidFrom,
            createRequest.ValidTo,
            ImagesAllId = createRequest.ProductImageId,
            createRequest.CreateUserName,
            createRequest.CreateDate,
            createRequest.LastUpdateUserName,
            createRequest.LastUpdateDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? id = await dbConnection.ExecuteScalarAsync<int?>(insertQuery, parameters, commandType: CommandType.Text);

        return id > 0 ? id.Value : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, NotFound>> UpdateAsync(PromotionProductFileInfoUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {PromotionProductFilesTableName}
            SET
                {PromotionFileIdColumnName} = ISNULL(@NewPromotionFileInfoId, {PromotionFileIdColumnName}),
                {ActiveColumnName} = @Active,
                {ValidFromDateColumnName} = @ValidFrom,
                {ValidToDateColumnName} = @ValidTo,
                {ImagesAllIdColumnName} = @ImagesAllId,
                {LastUpdateUserNameColumnName} = @LastUpdateUserName,
                {LastUpdateDateColumnName} = @LastUpdateDate
            
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.NewPromotionFileInfoId,
            updateRequest.Active,
            updateRequest.ValidFrom,
            updateRequest.ValidTo,
            ImagesAllId = updateRequest.ProductImageId,
            updateRequest.LastUpdateUserName,
            updateRequest.LastUpdateDate,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0 ? new Success() : new NotFound();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string deleteQuery =
            $"""
            DELETE
            FROM {PromotionProductFilesTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(deleteQuery, parameters, commandType: CommandType.Text);

        return rowsAffected > 0;
    }

    private static List<PromotionProductFileInfo> MapAllFromDAO(IEnumerable<PromotionProductFileInfoDAO> promotionProductFileInfoDAOs)
    {
        List<PromotionProductFileInfo> output = new();

        foreach (PromotionProductFileInfoDAO promotionProductFileInfoDAO in promotionProductFileInfoDAOs)
        {
            PromotionProductFileInfo promotionProductFileInfo = MapFromDAO(promotionProductFileInfoDAO);

            output.Add(promotionProductFileInfo);
        }

        return output;
    }

    private static PromotionProductFileInfo MapFromDAO(PromotionProductFileInfoDAO promotionProductFileInfoDAO)
    {
        return new()
        {
            Id = promotionProductFileInfoDAO.Id,
            ProductId = promotionProductFileInfoDAO.ProductId,
            ValidFrom = promotionProductFileInfoDAO.ValidFrom,
            ValidTo = promotionProductFileInfoDAO.ValidTo,
            Active = promotionProductFileInfoDAO.Active,
            ProductImageId = promotionProductFileInfoDAO.ProductImageId,
            CreateUserName = promotionProductFileInfoDAO.CreateUserName,
            CreateDate = promotionProductFileInfoDAO.CreateDate,
            LastUpdateUserName = promotionProductFileInfoDAO.LastUpdateUserName,
            LastUpdateDate = promotionProductFileInfoDAO.LastUpdateDate,
            PromotionFileInfoId = promotionProductFileInfoDAO.PromotionFileInfoId,
            PromotionFileInfo = promotionProductFileInfoDAO.PromotionFileInfo,
        };
    }
}