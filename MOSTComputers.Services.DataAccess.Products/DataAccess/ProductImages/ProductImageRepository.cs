using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.ProductImage;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImage;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
using MOSTComputers.Services.DataAccess.Products.Utils;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;

using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages;
internal sealed class ProductImageRepository : IProductImageRepository
{
    public ProductImageRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private const int _minimumImagesAllInsertId = 100000;
    private const string _minimumImagesAllInsertIdString = "100000";

    private readonly IConnectionStringProvider _connectionStringProvider;

    public int GetMinimumImagesAllInsertIdForLocalApplication()
    {
        return _minimumImagesAllInsertId;
    }

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllWithoutFileDataAsync()
    {
        const string getAllQuery =
            $"""
            SELECT {AllImagesTable.IdColumnName} AS {AllImagesTable.IdColumnAlias},
                {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                {AllImagesTable.DescriptionColumnName} AS {AllImagesTable.DescriptionColumnAlias},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK);
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductImageData> productImages = await dbConnection.QueryAsync<ProductImageData>(
                getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return productImages.GroupBy(x => x.ProductId ?? -1)
            .ToList();
    }

    public async Task<List<IGrouping<int, ProductImage>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllInProductQuery =
            $"""
            SELECT {AllImagesTable.IdColumnName} AS {AllImagesTable.IdColumnAlias},
                {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                {AllImagesTable.DescriptionColumnName} AS {AllImagesTable.DescriptionColumnAlias},
                {AllImagesTable.ImageDataColumnName},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.ProductIdColumnName} IN @productIds;
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductImage> productImages = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk => dbConnection.QueryAsync<ProductImage>(
                getAllInProductQuery, new { productIds = productIdsChunk }, commandType: CommandType.Text),
            productIds.AsList());

        transactionScope.Complete();

        return productImages.GroupBy(x => x.ProductId ?? -1)
            .ToList();
    }

    public async Task<List<IGrouping<int, ProductImageData>>> GetAllInProductsWithoutFileDataAsync(IEnumerable<int> productIds)
    {
        const string getAllInProductWithoutFileDataQuery =
            $"""
            SELECT {AllImagesTable.IdColumnName} AS {AllImagesTable.IdColumnAlias},
                {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                {AllImagesTable.DescriptionColumnName} AS {AllImagesTable.DescriptionColumnAlias},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.ProductIdColumnName} IN @productIds;
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductImageData> productImagesWithoutFileData = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return dbConnection.QueryAsync<ProductImageData>(
                    getAllInProductWithoutFileDataQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        transactionScope.Complete();

        return productImagesWithoutFileData.GroupBy(x => x.ProductId ?? -1)
            .ToList();
    }

    public async Task<List<ProductImagesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        const string getCountOfAllInProductQuery =
            $"""
            SELECT {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                COUNT(*) AS {AllImagesTable.CountColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.ProductIdColumnName} IN @productIds
            GROUP BY {AllImagesTable.ProductIdColumnName};
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductImagesForProductCountData> productImagesCountData = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk,
                };

                return dbConnection.QueryAsync<ProductImagesForProductCountData>(
                    getCountOfAllInProductQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        transactionScope.Complete();

        return productImagesCountData;
    }

    public async Task<List<ProductImage>> GetAllInProductAsync(int productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT {AllImagesTable.IdColumnName} AS {AllImagesTable.IdColumnAlias},
                {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                {AllImagesTable.DescriptionColumnName} AS {AllImagesTable.DescriptionColumnAlias},
                {AllImagesTable.ImageDataColumnName},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.ProductIdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductImage> images = await dbConnection.QueryAsync<ProductImage>(
            getAllInProductQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return images.AsList();
    }

    public async Task<List<ProductImageData>> GetAllInProductWithoutFileDataAsync(int productId)
    {
        const string getAllInProductWithoutFileDataQuery =
            $"""
            SELECT {AllImagesTable.IdColumnName} AS {AllImagesTable.IdColumnAlias},
                {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                {AllImagesTable.DescriptionColumnName} AS {AllImagesTable.DescriptionColumnAlias},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.ProductIdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductImageData> productImageDatas = await dbConnection.QueryAsync<ProductImageData>(
            getAllInProductWithoutFileDataQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productImageDatas.AsList();
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT COUNT(*)
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.ProductIdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int countOfAllInProduct = await dbConnection.QueryFirstOrDefaultAsync<int>(
            getAllInProductQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return countOfAllInProduct;
    }

    public async Task<List<ProductImage>> GetAllFirstImagesForAllProductsAsync()
    {
        const string getAllFirstImagesForAllProductsQuery =
            $"""
            SELECT {FirstImagesTable.IdColumnName} AS {FirstImagesTable.IdColumnAlias},
                {FirstImagesTable.DescriptionColumnName} AS {FirstImagesTable.DescriptionColumnAlias},
                {FirstImagesTable.ImageDataColumnName},
                {FirstImagesTable.ImageContentTypeColumnName},
                {FirstImagesTable.DateModifiedColumnName}
            FROM {FirstImagesTableName} WITH (NOLOCK)
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductFirstImageDAO> images = await dbConnection.QueryAsync<ProductFirstImageDAO>(
            getAllFirstImagesForAllProductsQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return images.SelectAsList(image => Map(image));
    }

    public async Task<List<ProductImageData>> GetAllFirstImagesWithoutFileDataForAllProductsAsync()
    {
        const string getAllFirstImagesForAllProductsQuery =
            $"""
            SELECT {FirstImagesTable.IdColumnName} AS {FirstImagesTable.IdColumnAlias},
                {FirstImagesTable.DescriptionColumnName} AS {FirstImagesTable.DescriptionColumnAlias},
                {FirstImagesTable.ImageContentTypeColumnName},
                {FirstImagesTable.DateModifiedColumnName}
            FROM {FirstImagesTableName} WITH (NOLOCK)
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductFirstImageDataDAO> images = await dbConnection.QueryAsync<ProductFirstImageDataDAO>(
            getAllFirstImagesForAllProductsQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return images.SelectAsList(image => Map(image));
    }

    public async Task<List<ProductImage>> GetFirstImagesForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT {FirstImagesTable.IdColumnName} AS {FirstImagesTable.IdColumnAlias},
                {FirstImagesTable.DescriptionColumnName} AS {FirstImagesTable.DescriptionColumnAlias},
                {FirstImagesTable.ImageDataColumnName},
                {FirstImagesTable.ImageContentTypeColumnName},
                {FirstImagesTable.DateModifiedColumnName}
            FROM {FirstImagesTableName} WITH (NOLOCK)
            WHERE {FirstImagesTable.IdColumnName} IN @productIds
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductFirstImageDAO> images = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk,
                };

                return dbConnection.QueryAsync<ProductFirstImageDAO>(
                    getByIdInFirstImagesQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        transactionScope.Complete();

        return images.SelectAsList(x => Map(x));
    }

    public async Task<List<ProductImageData>> GetFirstImagesWithoutFileDataForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT {FirstImagesTable.IdColumnName} AS {FirstImagesTable.IdColumnAlias},
                {FirstImagesTable.DescriptionColumnName} AS {FirstImagesTable.DescriptionColumnAlias},
                {FirstImagesTable.ImageContentTypeColumnName},
                {FirstImagesTable.DateModifiedColumnName}
            FROM {FirstImagesTableName} WITH (NOLOCK)
            WHERE {FirstImagesTable.IdColumnName} IN @productIds
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductFirstImageDataDAO> images = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk,
                };

                return dbConnection.QueryAsync<ProductFirstImageDataDAO>(
                    getByIdInFirstImagesQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        transactionScope.Complete();

        return images.SelectAsList(x => Map(x));
    }

    public async Task<List<ProductFirstImageExistsForProductData>> DoProductsHaveImagesInFirstImagesAsync(IEnumerable<int> productIds)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT {FirstImagesTable.IdColumnName} AS {FirstImagesTable.IdColumnAlias}
            FROM {FirstImagesTableName} WITH (NOLOCK)
            WHERE {FirstImagesTable.IdColumnName} IN @productIds
            """;

        var parameters = new { productIds };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<int> existingImageProductIds = await ExecuteListQueryWithParametersInChunksAsync(
            async productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk,
                };

                return await dbConnection.QueryAsync<int>(
                    getByIdInFirstImagesQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        transactionScope.Complete();

        List<ProductFirstImageExistsForProductData> output = new();

        foreach (int productId in productIds)
        {
            ProductFirstImageExistsForProductData firstImageExistsForProductData = new()
            {
                ProductId = productId,
                FirstImageExists = existingImageProductIds.Contains(productId)
            };

            output.Add(firstImageExistsForProductData);
        }

        return output;
    }

    public async Task<ProductImage?> GetByIdInAllImagesAsync(int id)
    {
        const string getByIdInAllImagesQuery =
            $"""
            SELECT {AllImagesTable.IdColumnName} AS {AllImagesTable.IdColumnAlias},
                {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                {AllImagesTable.DescriptionColumnName} AS {AllImagesTable.DescriptionColumnAlias},
                {AllImagesTable.ImageDataColumnName},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.IdColumnName} = @id;
            """;

        var parameters = new
        {
            id
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductImage? image = await dbConnection.QueryFirstOrDefaultAsync<ProductImage>(
            getByIdInAllImagesQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return image;
    }

    public async Task<ProductImageData?> GetByIdInAllImagesWithoutFileDataAsync(int id)
    {
        const string getByIdInAllImagesWithoutFileDataQuery =
            $"""
            SELECT {AllImagesTable.IdColumnName} AS {AllImagesTable.IdColumnAlias},
                {AllImagesTable.ProductIdColumnName} AS {AllImagesTable.ProductIdColumnAlias},
                {AllImagesTable.DescriptionColumnName} AS {AllImagesTable.DescriptionColumnAlias},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName}
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.IdColumnName} = @id;
            """;

        var parameters = new
        {
            id
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductImageData? productImageData = await dbConnection.QueryFirstOrDefaultAsync<ProductImageData>(
            getByIdInAllImagesWithoutFileDataQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productImageData;
    }

    public async Task<bool> DoesProductImageExistAsync(int imageId)
    {
        const string doesProductImageExistQuery =
            $"""
            IF EXISTS (SELECT 1
            FROM {AllImagesTableName} WITH (NOLOCK)
            WHERE {AllImagesTable.IdColumnName} = @id)
            BEGIN
                SELECT 1;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;

        var parameters = new
        {
            imageId
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        bool doesImageExist = await dbConnection.QueryFirstOrDefaultAsync<bool>(
            doesProductImageExistQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return doesImageExist;
    }

    public async Task<ProductImage?> GetByProductIdInFirstImagesAsync(int productId)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT {FirstImagesTable.IdColumnName} AS {FirstImagesTable.IdColumnAlias},
                {FirstImagesTable.DescriptionColumnName} AS {FirstImagesTable.DescriptionColumnAlias},
                {FirstImagesTable.ImageDataColumnName},
                {FirstImagesTable.ImageContentTypeColumnName},
                {FirstImagesTable.DateModifiedColumnName}
            FROM {FirstImagesTableName} WITH (NOLOCK)
            WHERE {FirstImagesTable.IdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductFirstImageDAO? image = await dbConnection.QueryFirstOrDefaultAsync<ProductFirstImageDAO>(
            getByIdInFirstImagesQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        if (image is null) return null;

        return Map(image);
    }

    public async Task<bool> DoesProductHaveImageInFirstImagesAsync(int productId)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            IF EXISTS (SELECT 1
            FROM {FirstImagesTableName} WITH (NOLOCK)
            WHERE {FirstImagesTable.IdColumnName} = @productId)
            BEGIN
                SELECT 1;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;

        var parameters = new
        {
            productId
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int imagesCount = await dbConnection.QueryFirstOrDefaultAsync<int>(
            getByIdInFirstImagesQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return imagesCount > 0;
    }

    public async Task<OneOf<int, UnexpectedFailureResult>> InsertInAllImagesAsync(ProductImageCreateRequest createRequest)
    {
        const string insertInAllImagesQuery =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);

            INSERT INTO {AllImagesTableName} ({AllImagesTable.IdColumnName},
                {AllImagesTable.ProductIdColumnName},
                {AllImagesTable.DescriptionColumnName},
                {AllImagesTable.ImageDataColumnName},
                {AllImagesTable.ImageContentTypeColumnName},
                {AllImagesTable.DateModifiedColumnName})
            OUTPUT INSERTED.{AllImagesTable.IdColumnName} INTO @InsertedIdTable
            VALUES (ISNULL((SELECT MAX({AllImagesTable.IdColumnName}) + 1 FROM {AllImagesTableName}), {_minimumImagesAllInsertIdString}),
                @productId, @HtmlData, @ImageData, @ImageContentType, @DateModified)
                
            SELECT TOP 1 Id FROM @InsertedIdTable;
            """;

        //throw new NotImplementedException($"ImagesAll does not allow modification: {insertInAllImagesQuery}");

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.HtmlData,
            createRequest.ImageData,
            createRequest.ImageContentType,
            createRequest.DateModified,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? id = await dbConnection.ExecuteScalarAsync<int?>(
            insertInAllImagesQuery, parameters, commandType: CommandType.Text);

        return id is not null && id > 0 ? id.Value : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> InsertInFirstImagesAsync(ProductFirstImageCreateRequest createRequest)
    {
        const string insertInFirstImagesQuery =
            $"""
            INSERT INTO {FirstImagesTableName}({FirstImagesTable.IdColumnName},
                {FirstImagesTable.DescriptionColumnName},
                {FirstImagesTable.ImageDataColumnName},
                {FirstImagesTable.ImageContentTypeColumnName},
                {FirstImagesTable.DateModifiedColumnName})
            VALUES (@productId, @HtmlData, @ImageData, @ImageContentType, @DateModified)
            """;

        //throw new NotImplementedException($"Images does not allow modification: {insertInFirstImagesQuery}");

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.HtmlData,
            createRequest.ImageData,
            createRequest.ImageContentType,
            createRequest.DateModified,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(
            insertInFirstImagesQuery, parameters, commandType: CommandType.Text);

        return rowsAffected != 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpdateInAllImagesAsync(ProductImageUpdateRequest updateRequest)
    {
        const string updateInAllImagesQuery =
            $"""
            UPDATE {AllImagesTableName}
            SET {AllImagesTable.DescriptionColumnName} = @HtmlData,
                {AllImagesTable.ImageDataColumnName} = @ImageData,
                {AllImagesTable.ImageContentTypeColumnName} = @ImageContentType,
                {AllImagesTable.DateModifiedColumnName} = @DateModified

            WHERE ID = @id;
            """;

        //throw new NotImplementedException($"ImagesAll does not allow modification: {updateInAllImagesQuery}");

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.HtmlData,
            updateRequest.ImageData,
            updateRequest.ImageContentType,
            updateRequest.DateModified,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(
            updateInAllImagesQuery, parameters, commandType: CommandType.Text);

        return rowsAffected >= 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpdateInFirstImagesAsync(ProductFirstImageUpdateRequest updateRequest)
    {
        const string updateInFirstImagesQuery =
            $"""
            UPDATE {FirstImagesTableName}
            SET {FirstImagesTable.DescriptionColumnName} = @HtmlData,
                {FirstImagesTable.ImageDataColumnName} = @ImageData,
                {FirstImagesTable.ImageContentTypeColumnName} = @ImageContentType,
                {FirstImagesTable.DateModifiedColumnName} = @DateModified

            WHERE {FirstImagesTable.IdColumnName} = @productId;
            """;

        //throw new NotImplementedException($"Images does not allow modification: {updateInFirstImagesQuery}");

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.HtmlData,
            updateRequest.ImageData,
            updateRequest.ImageContentType,
            updateRequest.DateModified,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int rowsAffected = await dbConnection.ExecuteAsync(
            updateInFirstImagesQuery, parameters, commandType: CommandType.Text);

        return rowsAffected >= 0 ? new Success() : new UnexpectedFailureResult();
    }

    public async Task<OneOf<bool, UnexpectedFailureResult>> UpdateHtmlDataInAllImagesByIdAsync(int imageId, string htmlData)
    {
        const string updateHtmlDataInAllImagesByIdQuery =
            $"""
            IF EXISTS (SELECT 1 FROM {AllImagesTableName} WHERE {AllImagesTable.IdColumnName} = @imageId)
            BEGIN
                UPDATE {AllImagesTableName}
                SET {AllImagesTable.DescriptionColumnName} = @HtmlData

                WHERE {AllImagesTable.IdColumnName} = @imageId;

                SELECT @@ROWCOUNT;            
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        //throw new NotImplementedException($"ImagesAll does not allow modification: {updateHtmlDataInAllImagesByIdQuery}");

        var parameters = new
        {
            imageId,
            HtmlData = htmlData
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int status = await dbConnection.ExecuteScalarAsync<int>(
            updateHtmlDataInAllImagesByIdQuery, parameters, commandType: CommandType.Text);

        if (status == 0) return new UnexpectedFailureResult();

        transactionScope.Complete();

        if (status == -1) return false;

        return true;
    }

    public async Task<OneOf<bool, UnexpectedFailureResult>> UpdateHtmlDataInFirstImagesByProductIdAsync(int productId, string htmlData)
    {
        const string updateHtmlDataInFirstImagesByProductIdQuery =
            $"""
            IF EXISTS (SELECT 1 FROM {FirstImagesTableName} WHERE {FirstImagesTable.IdColumnName} = @productId)
            BEGIN
                UPDATE {FirstImagesTableName}
                SET {FirstImagesTable.DescriptionColumnName} = @HtmlData

                WHERE {FirstImagesTable.IdColumnName} = @productId;

                SELECT @@ROWCOUNT;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        throw new NotImplementedException($"ImagesAll does not allow modification: {updateHtmlDataInFirstImagesByProductIdQuery}");

        var parameters = new
        {
            productId,
            HtmlData = htmlData
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int status = await dbConnection.ExecuteScalarAsync<int>(
            updateHtmlDataInFirstImagesByProductIdQuery, parameters, commandType: CommandType.Text);

        if (status == -1) return false;

        return status > 0 ? true : new UnexpectedFailureResult();
    }

    public async Task<OneOf<bool, UnexpectedFailureResult>> UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(int productId, string htmlData)
    {
        const string updateHtmlDataInFirstAndAllImagesByProductIdQuery =
            $"""
            IF EXISTS (SELECT 1 FROM {AllImagesTableName} WHERE {AllImagesTable.ProductIdColumnName} = @productId)
            OR EXISTS (SELECT 1 FROM {FirstImagesTableName} WHERE {FirstImagesTable.IdColumnName} = @productId)
            BEGIN
                DECLARE @TotalAffectedRows INT = 0;
            
                UPDATE {AllImagesTableName}
                SET {AllImagesTable.DescriptionColumnName} = @HtmlData
            
                WHERE {AllImagesTable.ProductIdColumnName} = @productId;

                SET @TotalAffectedRows = @TotalAffectedRows + @@ROWCOUNT;

                UPDATE {FirstImagesTableName}
                SET {FirstImagesTable.DescriptionColumnName} = @HtmlData

                WHERE {FirstImagesTable.IdColumnName} = @productId;

                SET @TotalAffectedRows = @TotalAffectedRows + @@ROWCOUNT;

                SELECT @TotalAffectedRows;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        //throw new NotImplementedException($"ImagesAll does not allow modification: {updateHtmlDataInFirstAndAllImagesByProductIdQuery}");

        var parameters = new
        {
            productId,
            HtmlData = htmlData
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int status = await dbConnection.ExecuteScalarAsync<int>(
            updateHtmlDataInFirstAndAllImagesByProductIdQuery, parameters, commandType: CommandType.Text);

        if (status == 0) return new UnexpectedFailureResult();

        transactionScope.Complete();

        if (status == -1) return false;

        return true;
    }

    public async Task<bool> DeleteInAllImagesByIdAsync(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {AllImagesTableName}
            WHERE {AllImagesTable.IdColumnName} = @id;
            """;

        //throw new NotImplementedException($"ImagesAll does not allow modification: {deleteQuery}");

        try
        {
            var parameters = new
            {
                id
            };

            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            int rowsAffected = await dbConnection.ExecuteAsync(
                deleteQuery, parameters, commandType: CommandType.Text);

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
    
    public async Task<bool> DeleteInFirstImagesByProductIdAsync(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {FirstImagesTableName}
            WHERE {FirstImagesTable.IdColumnName} = @id;
            """;

        throw new NotImplementedException($"ImagesAll does not allow modification: {deleteQuery}");

        try
        {
            var parameters = new
            {
                id
            };

            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            int rowsAffected = await dbConnection.ExecuteAsync(
                deleteQuery, parameters, commandType: CommandType.Text);

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAllInAllImagesByProductIdAsync(int productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {AllImagesTableName}
            WHERE {AllImagesTable.ProductIdColumnName} = @productId;
            """;

        //throw new NotImplementedException($"ImagesAll does not allow modification: {deleteQuery}");

        try
        {
            var parameters = new
            {
                productId
            };

            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            int rowsAffected = await dbConnection.ExecuteAsync(
                deleteQuery, parameters, commandType: CommandType.Text);

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static ProductImage Map(ProductFirstImageDAO image)
    {
        return new ProductImage()
        {
            Id = image.Id,
            ProductId = image.Id,
            HtmlData = image.HtmlData,
            ImageData = image.ImageData,
            ImageContentType = image.ImageContentType,
            DateModified = image.DateModified,
        };
    }

    private static ProductImageData Map(ProductFirstImageDataDAO image)
    {
        return new ProductImageData()
        {
            Id = image.Id,
            ProductId = image.Id,
            HtmlData = image.HtmlData,
            ImageContentType = image.ImageContentType,
            DateModified = image.DateModified,
        };
    }
}