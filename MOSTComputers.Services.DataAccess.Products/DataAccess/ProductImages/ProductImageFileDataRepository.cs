using Dapper;
using FluentValidation.Results;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ImageFileNamesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages;
internal sealed class ProductImageFileDataRepository : IProductImageFileDataRepository
{
    public ProductImageFileDataRepository(
        [FromKeyedServices(ConfigureServices.ConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ProductImageFileData>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {ImageFileNamesTableName}
            ORDER BY {ProductIdColumnName}, {DisplayOrderColumnName};
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductImageFileData> data = await dbConnection.QueryAsync<ProductImageFileData>(
            getAllQuery, new { }, commandType: CommandType.Text);
            
        return data.AsList();
    }
    
    public async Task<List<IGrouping<int, ProductImageFileData>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllForProductsQuery =
            $"""
            SELECT * FROM {ImageFileNamesTableName}
            WHERE {ProductIdColumnName} IN @productIds
            ORDER BY {DisplayOrderColumnName};
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductImageFileData> imageFileNameInfos = await ExecuteListQueryWithParametersInChunksAsync(
            async productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return await dbConnection.QueryAsync<ProductImageFileData>(getAllForProductsQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        return imageFileNameInfos
            .GroupBy(x => x.ProductId)
            .AsList();
    }

    public async Task<List<ProductImageFileData>> GetAllInProductAsync(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {ImageFileNamesTableName}
            WHERE {ProductIdColumnName} = @productId
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            productId
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductImageFileData> data = await dbConnection.QueryAsync<ProductImageFileData>(
            getAllForProductQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<ProductImageFileData?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 * FROM {ImageFileNamesTableName}
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new
        {
            id
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<ProductImageFileData>(
            getByIdQuery, parameters, commandType: CommandType.Text);
    }

    public async Task<ProductImageFileData?> GetByProductIdAndImageIdAsync(int productId, int imageId)
    {
        const string getByProductIdAndImageIdQuery =
            $"""
            SELECT TOP 1 * FROM {ImageFileNamesTableName}
            WHERE {ProductIdColumnName} = @productId
            AND {ImageIdColumnName} = @imageId
            """;

        var parameters = new
        {
            productId,
            imageId
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<ProductImageFileData>(
            getByProductIdAndImageIdQuery, parameters, commandType: CommandType.Text);
    }

    public async Task<ProductImageFileData?> GetByFileNameAsync(string fileName)
    {
        const string getAllForProductQuery =
            $"""
            SELECT TOP 1 * FROM {ImageFileNamesTableName}
            WHERE {FileNameColumnName} = @fileName
            """;

        var parameters = new
        {
            fileName
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<ProductImageFileData>(
            getAllForProductQuery, parameters, commandType: CommandType.Text);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ProductImageFileNameInfoCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @ImageFileNameInsertedIdTable TABLE (ID INT);

            DECLARE @DisplayOrderInRange INT;
            DECLARE @MaxDisplayOrderForProduct INT;

            SELECT @MaxDisplayOrderForProduct = ISNULL(
                (SELECT COUNT(*) FROM {ImageFileNamesTableName} WHERE {ProductIdColumnName} = @productId), 0) + 1;

            SELECT TOP 1 @DisplayOrderInRange =
                CASE
                    WHEN @CustomDisplayOrder IS NULL
                        THEN @MaxDisplayOrderForProduct
                    WHEN @MaxDisplayOrderForProduct <= @CustomDisplayOrder
                        THEN @MaxDisplayOrderForProduct
                    ELSE @CustomDisplayOrder
                END

            UPDATE {ImageFileNamesTableName}
                SET {DisplayOrderColumnName} = {DisplayOrderColumnName} + 1
            WHERE {ProductIdColumnName} = @productId
            AND {DisplayOrderColumnName} >= @DisplayOrderInRange;

            INSERT INTO {ImageFileNamesTableName}(
                {ProductIdColumnName}, {ImageIdColumnName},
                {DisplayOrderColumnName}, {FileNameColumnName}, {ActiveColumnName},
                {CreateUserNameColumnName}, {CreateDateColumnName},
                {LastUpdateUserNameColumnName}, {LastUpdateDateColumnName}
            )
            OUTPUT INSERTED.{IdColumnName} INTO @ImageFileNameInsertedIdTable
            SELECT
                @productId, @imageId,
                @DisplayOrderInRange, @FileName, @Active,
                @CreateUserName, @CreateDate, @LastUpdateUserName, @LastUpdateDate;

            SELECT Id FROM @ImageFileNameInsertedIdTable;
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            imageId = createRequest.ImageId,
            createRequest.FileName,
            createRequest.CustomDisplayOrder,
            createRequest.Active,

            createRequest.CreateUserName,
            createRequest.CreateDate,
            createRequest.LastUpdateUserName,
            createRequest.LastUpdateDate,
        };

        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int newFileNameInfoId = await dbConnection.ExecuteScalarAsync<int>(insertQuery, parameters, commandType: CommandType.Text, commandTimeout: 600);

        if (newFileNameInfoId <= 0) return new UnexpectedFailureResult();

        transactionScope.Complete();

        return newFileNameInfoId;
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateAsync(ProductImageFileNameInfoByIdUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            DECLARE @ProductId INT;
            DECLARE @DisplayOrder INT;

            DECLARE @MaxDisplayOrder INT;
            
            SELECT TOP 1 @ProductId = {ProductIdColumnName}, @DisplayOrder = {DisplayOrderColumnName}
            FROM {ImageFileNamesTableName}
            WHERE {IdColumnName} = @id;

            SELECT @MaxDisplayOrder = ISNULL((SELECT COUNT(*) FROM {ImageFileNamesTableName} WHERE {ProductIdColumnName} = @productId), 1);

            SET @NewDisplayOrder = 
            CASE 
                WHEN @NewDisplayOrder IS NULL THEN NULL
                WHEN @NewDisplayOrder < 1 THEN 1
                WHEN @NewDisplayOrder > @MaxDisplayOrder THEN @MaxDisplayOrder
                ELSE @NewDisplayOrder
            END;

            UPDATE {ImageFileNamesTableName}
            SET {DisplayOrderColumnName} = 
                CASE
                    WHEN {IdColumnName} = @id THEN @NewDisplayOrder
                    WHEN @DisplayOrder < @NewDisplayOrder AND {DisplayOrderColumnName} > @DisplayOrder AND {DisplayOrderColumnName} <= @NewDisplayOrder
                        THEN {DisplayOrderColumnName} - 1
                    WHEN @DisplayOrder > @NewDisplayOrder AND {DisplayOrderColumnName} < @DisplayOrder AND {DisplayOrderColumnName} >= @NewDisplayOrder
                        THEN {DisplayOrderColumnName} + 1
                    ELSE {DisplayOrderColumnName}
                END
            WHERE {ProductIdColumnName} = @ProductId;

            UPDATE {ImageFileNamesTableName}
            SET 
                {ImageIdColumnName} = @imageId,
                {FileNameColumnName} = @FileName,
                {ActiveColumnName} = @Active,
                {LastUpdateUserNameColumnName} = @LastUpdateUserName,
                {LastUpdateDateColumnName} = @LastUpdateDate

            WHERE {IdColumnName} = @id;
            """;

        const string updateQueryWithNoDisplayOrderChanges =
            $"""
            UPDATE {ImageFileNamesTableName}
            SET {ImageIdColumnName} = @imageId,
                {FileNameColumnName} = @FileName,
                {ActiveColumnName} = @Active,
                {LastUpdateUserNameColumnName} = @LastUpdateUserName,
                {LastUpdateDateColumnName} = @LastUpdateDate

            WHERE {IdColumnName} = @id;
            """;

        if (!updateRequest.ShouldUpdateDisplayOrder)
        {
            var parametersSimple = new
            {
                id = updateRequest.Id,
                imageId = updateRequest.ImageId,
                updateRequest.FileName,
                updateRequest.Active,

                updateRequest.LastUpdateUserName,
                updateRequest.LastUpdateDate,
            };

            using SqlConnection dbConnectionInner = new(_connectionStringProvider.ConnectionString);

            int rowsAffectedInner = await dbConnectionInner.ExecuteAsync(
                updateQueryWithNoDisplayOrderChanges, parametersSimple, commandType: CommandType.Text);

            if (rowsAffectedInner <= 0) return new UnexpectedFailureResult();

            return new Success();
        }

        var parameters = new
        {
            id = updateRequest.Id,
            imageId = updateRequest.ImageId,
            updateRequest.FileName,
            updateRequest.Active,
            updateRequest.NewDisplayOrder,

            updateRequest.LastUpdateUserName,
            updateRequest.LastUpdateDate,
        };

        using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        if (rowsAffected <= 0) return new UnexpectedFailureResult();

        transactionScope.Complete();

        return new Success();
    }

    public async Task<bool> DeleteAllForProductIdAsync(int productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {ImageFileNamesTableName}
            WHERE {ProductIdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId
        };

        try
        {
            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            int rowsAffected = await dbConnection.ExecuteAsync(deleteQuery, parameters, commandType: CommandType.Text);

            return rowsAffected > 0;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string deleteQuery =
            $"""
            DECLARE @ProductId INT;
            DECLARE @DisplayOrder INT;
            
            SELECT TOP 1 @ProductId = {ProductIdColumnName}, @DisplayOrder = {DisplayOrderColumnName}
            FROM {ImageFileNamesTableName}
            WHERE {IdColumnName} = @id;

            DELETE FROM {ImageFileNamesTableName}
            WHERE {IdColumnName} = @id;

            UPDATE {ImageFileNamesTableName}
            SET {DisplayOrderColumnName} = {DisplayOrderColumnName} - 1
            
            WHERE {ProductIdColumnName} = @ProductId
            AND {DisplayOrderColumnName} > @DisplayOrder;
            """;

        var parameters = new
        {
            id
        };

        try
        {
            using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            int rowsAffected = await dbConnection.ExecuteAsync(deleteQuery, parameters, commandType: CommandType.Text);

            transactionScope.Complete();

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}