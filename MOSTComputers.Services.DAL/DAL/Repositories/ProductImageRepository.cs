using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using static MOSTComputers.Services.DAL.DAL.Repositories.RepositoryCommonElements;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductImageRepository : RepositoryBase, IProductImageRepository
{
    private const string _firstImagesTableName = "dbo.Images";
    private const string _allImagesTableName = "dbo.ImagesAll";
    private const string _imageFileNameInfosTable = "dbo.ImageFileName";

    public ProductImageRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<ProductImage> GetAllInProduct(uint productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT ID AS ImagePrime, CSTID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_allImagesTableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<ProductImage, dynamic>(getAllInProductQuery, new { productId = (int)productId });
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts()
    {
        const string getAllFirstImagesForAllProductsQuery =
            $"""
            SELECT ID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_firstImagesTableName}
            """;

        IEnumerable<ProductFirstImage> images = _relationalDataAccess.GetData<ProductFirstImage, dynamic>(getAllFirstImagesForAllProductsQuery, new { });

        return images.Select(image => Map(image));
    }

    public IEnumerable<ProductImage> GetFirstImagesForSelectionOfProducts(List<uint> productIds)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT ID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_firstImagesTableName}
            WHERE ID IN @productIds
            """;

        IEnumerable<ProductFirstImage> images = _relationalDataAccess.GetData<ProductFirstImage, dynamic>(getByIdInFirstImagesQuery, new { productIds = productIds.Select(x => (int)x) });

        return images.Select(x => Map(x));
    }

    public ProductImage? GetByIdInAllImages(uint id)
    {
        const string getByIdInAllImagesQuery =
            $"""
            SELECT ID AS ImagePrime, CSTID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_allImagesTableName}
            WHERE ID = @id;
            """;

        return _relationalDataAccess.GetData<ProductImage, dynamic>(getByIdInAllImagesQuery, new { id = (int)id }).FirstOrDefault();
    }

    public ProductImage? GetByProductIdInFirstImages(uint productId)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT ID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_firstImagesTableName}
            WHERE ID = @productId;
            """;

        ProductFirstImage? image = _relationalDataAccess.GetData<ProductFirstImage, dynamic>(getByIdInFirstImagesQuery, new { productId = (int)productId })
            .FirstOrDefault();

        if (image is null) return null;

        return Map(image);
    }

    public OneOf<uint, UnexpectedFailureResult> InsertInAllImages(ProductImageCreateRequest createRequest)
    {
        const string insertInAllImagesQuery =
            $"""
            CREATE TABLE #Temp_Table (ID INT)

            INSERT INTO {_allImagesTableName}(ID, CSTID, Description, Image, ImageFileExt, DateModified)
            OUTPUT INSERTED.ID INTO #Temp_Table
            VALUES (ISNULL((SELECT MAX(ID) + 1 FROM {_allImagesTableName}), 0), @productId, @XML, @ImageData, @ImageFileExtension, @DateModified)

            SELECT TOP 1 ID FROM #Temp_Table
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.XML,
            createRequest.ImageData,
            createRequest.ImageFileExtension,
            createRequest.DateModified,
        };

        int? id = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertInAllImagesQuery, parameters);

        return (id is not null && id >= 0) ? (uint)id : new UnexpectedFailureResult();
    }

    public OneOf<uint, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(ProductImageCreateRequest createRequest, uint? displayOrder = null)
    {
        const string insertInAllImagesAndImageFileNameInfosQuery =
            $"""
            CREATE TABLE #Temp_Table (ID INT)

            DECLARE @DisplayOrderInRange INT, @MaxDisplayOrderForProduct INT

            SELECT TOP 1 @MaxDisplayOrderForProduct = MAX(ImgNo) + 1 FROM {_imageFileNameInfosTable}
            WHERE CSTID = @productId;
            
            SET @displayOrder = ISNULL(@displayOrder, ISNULL(@MaxDisplayOrderForProduct, 1));

            SELECT TOP 1 @DisplayOrderInRange = ISNULL(
                (SELECT TOP 1 @MaxDisplayOrderForProduct FROM {_imageFileNameInfosTable}
                WHERE @MaxDisplayOrderForProduct <= @displayOrder),
                @displayOrder);
            
            UPDATE {_imageFileNameInfosTable}
                SET ImgNo = ImgNo + 1
            WHERE CSTID = @productId
            AND ImgNo >= @DisplayOrderInRange;

            INSERT INTO {_allImagesTableName}(ID, CSTID, Description, Image, ImageFileExt, DateModified)
            OUTPUT INSERTED.ID INTO #Temp_Table
            VALUES (ISNULL((SELECT MAX(ID) + 1 FROM {_allImagesTableName}), 0), @productId, @XML, @ImageData, @ImageFileExtension, @DateModified)

            INSERT INTO {_imageFileNameInfosTable}(CSTID, ImgNo, ImgFileName)
            VALUES (@productId,
                @DisplayOrderInRange,
                CONCAT(CAST((SELECT TOP 1 ID FROM #Temp_Table) AS VARCHAR), '.', SUBSTRING(@ImageFileExtension, CHARINDEX('/', @ImageFileExtension) + 1, 100)))

            SELECT TOP 1 ID FROM #Temp_Table
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            displayOrder = (int?)displayOrder,
            createRequest.XML,
            createRequest.ImageData,
            createRequest.ImageFileExtension,
            createRequest.DateModified,
        };

        int? id = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(
            insertInAllImagesAndImageFileNameInfosQuery, parameters, doInTransaction: true);

        return (id is not null && id >= 0) ? (uint)id : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest)
    {
        const string insertInFirstImagesQuery =
            $"""
            INSERT INTO {_firstImagesTableName}(ID, Description, Image, ImageFileExt, DateModified)
            VALUES (@productId, @XML, @ImageData, @ImageFileExtension, @DateModified)
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.XML,
            createRequest.ImageData,
            createRequest.ImageFileExtension,
            createRequest.DateModified,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductFirstImage, dynamic>(insertInFirstImagesQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> UpdateInAllImages(ProductImageUpdateRequest createRequest)
    {
        const string updateInAllImagesQuery =
            $"""
            UPDATE {_allImagesTableName}
            SET Description = @XML,
                Image = @ImageData,
                ImageFileExt = @ImageFileExtension,
                DateModified = @DateModified

            WHERE ID = @id;
            """;

        var parameters = new
        {
            id = createRequest.Id,
            createRequest.XML,
            createRequest.ImageData,
            createRequest.ImageFileExtension,
            createRequest.DateModified,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductImage, dynamic>(updateInAllImagesQuery, parameters);

        return (rowsAffected >= 0) ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> UpdateInFirstImages(ProductFirstImageUpdateRequest updateRequest)
    {
        const string updateInFirstImagesQuery =
            $"""
            UPDATE {_firstImagesTableName}
            SET Description = @XML,
                Image = @ImageData,
                ImageFileExt = @ImageFileExtension,
                DateModified = @DateModified

            WHERE ID = @productId;
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.XML,
            updateRequest.ImageData,
            updateRequest.ImageFileExtension,
            updateRequest.DateModified,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductFirstImage, dynamic>(updateInFirstImagesQuery, parameters);

        return (rowsAffected >= 0) ? new Success() : new UnexpectedFailureResult();
    }

    public bool DeleteInAllImagesById(uint id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_allImagesTableName}
            WHERE ID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductImage, dynamic>(deleteQuery, new { id = (int)id });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteInAllImagesAndImageFilePathInfosById(uint id)
    {
        const string deleteInAllImagesAndImageFilePathInfosByIdQuery =
            $"""
            DECLARE @displayOrderOfDeletedImage INT;
            DECLARE @productIdOfDeletedImage INT;

            DELETE FROM {_allImagesTableName}
            WHERE ID = @id;

            SELECT TOP 1 @displayOrderOfDeletedImage = ImgNo, @productIdOfDeletedImage = CSTID
            FROM {_imageFileNameInfosTable}
            WHERE SUBSTRING(ImgFileName, 1, CHARINDEX('.', ImgFileName) - 1) = CAST(@id as VARCHAR);

            DELETE FROM {_imageFileNameInfosTable}
            WHERE SUBSTRING(ImgFileName, 1, CHARINDEX('.', ImgFileName) - 1) = CAST(@id as VARCHAR);

            UPDATE {_imageFileNameInfosTable}
                SET ImgNo = ImgNo - 1

            WHERE CSTID = @productIdOfDeletedImage
            AND ImgNo > @displayOrderOfDeletedImage;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductImage, dynamic>(deleteInAllImagesAndImageFilePathInfosByIdQuery, new { id = (int)id });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteInFirstImagesByProductId(uint id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_firstImagesTableName}
            WHERE ID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductFirstImage, dynamic>(deleteQuery, new { id = (int)id });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllWithSameProductIdInAllImages(uint productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_allImagesTableName}
            WHERE CSTID = @productId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductImage, dynamic>(deleteQuery, new { productId = (int)productId });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static ProductImage Map(ProductFirstImage image)
    {
        return new ProductImage()
        {
            Id = image.Id,
            ProductId = image.Id,
            XML = image.XML,
            ImageData = image.ImageData,
            ImageFileExtension = image.ImageFileExtension,
            DateModified = image.DateModified,
        };
    }
}