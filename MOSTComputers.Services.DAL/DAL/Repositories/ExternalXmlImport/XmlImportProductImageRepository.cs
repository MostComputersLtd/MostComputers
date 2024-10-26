using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ExternalXmlImport.ProductImage;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;

namespace MOSTComputers.Services.DAL.DAL.Repositories.ExternalXmlImport;

internal sealed class XmlImportProductImageRepository : RepositoryBase, IXmlImportProductImageRepository
{
    private const string _firstImagesTableName = "dbo.TEST_Images";
    private const string _allImagesTableName = "dbo.TEST_ImagesAll";
    private const string _imageFileNameInfosTable = "dbo.ImageFileName";

    public XmlImportProductImageRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<XmlImportProductImage> GetAllInProduct(int productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT ID AS ImagePrime, CSTID AS ImageProductId, Description AS HtmlData, Image, ImageFileExt, DateModified
            FROM {_allImagesTableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<XmlImportProductImage, dynamic>(getAllInProductQuery, new { productId = productId });
    }

    public IEnumerable<XmlImportProductImage> GetAllFirstImagesForAllProducts()
    {
        const string getAllFirstImagesForAllProductsQuery =
            $"""
            SELECT ID AS ImageProductId, Description AS HtmlData, Image, ImageFileExt, DateModified
            FROM {_firstImagesTableName}
            """;

        IEnumerable<XmlImportProductFirstImage> images = _relationalDataAccess.GetData<XmlImportProductFirstImage, dynamic>(getAllFirstImagesForAllProductsQuery, new { });

        return images.Select(image => Map(image));
    }

    public IEnumerable<XmlImportProductImage> GetFirstImagesForSelectionOfProducts(List<int> productIds)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT ID AS ImageProductId, Description AS HtmlData, Image, ImageFileExt, DateModified
            FROM {_firstImagesTableName}
            WHERE ID IN @productIds
            """;

        IEnumerable<XmlImportProductFirstImage> images = _relationalDataAccess.GetData<XmlImportProductFirstImage, dynamic>(getByIdInFirstImagesQuery,
            new { productIds = productIds });

        return images.Select(x => Map(x));
    }

    public XmlImportProductImage? GetByIdInAllImages(int id)
    {
        const string getByIdInAllImagesQuery =
            $"""
            SELECT ID AS ImagePrime, CSTID AS ImageProductId, Description AS HtmlData, Image, ImageFileExt, DateModified
            FROM {_allImagesTableName}
            WHERE ID = @id;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<XmlImportProductImage, dynamic>(getByIdInAllImagesQuery, new { id = id });
    }

    public XmlImportProductImage? GetByProductIdInFirstImages(int productId)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT ID AS ImageProductId, Description AS HtmlData, Image, ImageFileExt, DateModified
            FROM {_firstImagesTableName}
            WHERE ID = @productId;
            """;

        XmlImportProductFirstImage? image = _relationalDataAccess.GetDataFirstOrDefault<XmlImportProductFirstImage, dynamic>(getByIdInFirstImagesQuery,
            new { productId = productId });

        if (image is null) return null;

        return Map(image);
    }

    public OneOf<int, UnexpectedFailureResult> UpsertInAllImages(XmlImportProductImageUpsertRequest upsertRequest)
    {
        const string upsertInAllImagesQuery =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {_allImagesTableName} WHERE ID = @id)
            BEGIN
                DECLARE @InsertedIdTable TABLE (Id INT);

                INSERT INTO {_allImagesTableName} (ID, CSTID, Description, Image, ImageFileExt, DateModified)
                OUTPUT INSERTED.ID INTO @InsertedIdTable
                VALUES (@id, @productId, @HtmlData, @ImageData, @ImageContentType, @DateModified);

                SELECT TOP 1 Id FROM @InsertedIdTable;
            END
            ELSE
            BEGIN
                UPDATE {_allImagesTableName}
                SET Description = @HtmlData,
                    Image = @ImageData,
                    ImageFileExt = @ImageContentType,
                    DateModified = @DateModified
            
                WHERE ID = @id;
            END
            """;

        var parameters = new
        {
            id = upsertRequest.Id,
            productId = upsertRequest.ProductId,
            HtmlData = upsertRequest.HtmlData,
            ImageData = upsertRequest.ImageData,
            ImageContentType = upsertRequest.ImageContentType,
            DateModified = upsertRequest.DateModified,
        };

        int? id = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(upsertInAllImagesQuery, parameters);

        return id is not null && id > 0 ? id.Value : new UnexpectedFailureResult();
    }

    public OneOf<int, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(
        XmlImportProductImageUpsertRequest createRequest, int? displayOrder = null)
    {
        const string insertInAllImagesAndImageFileNameInfosQuery =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);

            DECLARE @DisplayOrderInRange INT, @MaxDisplayOrderForProduct INT

            SELECT TOP 1 @MaxDisplayOrderForProduct = MAX(S) + 1 FROM {_imageFileNameInfosTable}
            WHERE CSTID = @productId;
            
            SET @displayOrder = ISNULL(@displayOrder, ISNULL(@MaxDisplayOrderForProduct, 1));

            SELECT TOP 1 @DisplayOrderInRange = ISNULL(
                (SELECT TOP 1 @MaxDisplayOrderForProduct FROM {_imageFileNameInfosTable}
                WHERE @MaxDisplayOrderForProduct <= @displayOrder),
                @displayOrder);
            
            UPDATE {_imageFileNameInfosTable}
                SET S = S + 1
            WHERE CSTID = @productId
            AND S >= @DisplayOrderInRange;

            INSERT INTO {_allImagesTableName}(ID, CSTID, Description, Image, ImageFileExt, DateModified)
            OUTPUT INSERTED.ID INTO @InsertedIdTable
            VALUES (ISNULL((SELECT MAX(ID) + 1 FROM {_allImagesTableName}), 1), @productId, @HtmlData, @ImageData, @ImageContentType, @DateModified)

            INSERT INTO {_imageFileNameInfosTable}(CSTID, ImageNumber, S, ImgFileName)
            VALUES (@productId,
                (SELECT MAX(ImageNumber) + 1 FROM {_imageFileNameInfosTable}
                WHERE CSTID = @productId),
                @DisplayOrderInRange,
                CONCAT(CAST((SELECT TOP 1 Id FROM @InsertedIdTable) AS VARCHAR), '.', SUBSTRING(@ImageContentType, CHARINDEX('/', @ImageContentType) + 1, 100)))

            SELECT TOP 1 Id FROM @InsertedIdTable;
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            displayOrder = displayOrder,
            HtmlData = createRequest.HtmlData,
            ImageData = createRequest.ImageData,
            ImageContentType = createRequest.ImageContentType,
            DateModified = createRequest.DateModified,
        };

        int? id = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(
            insertInAllImagesAndImageFileNameInfosQuery, parameters, doInTransaction: true);

        return id is not null && id > 0 ? id.Value : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> InsertInFirstImages(XmlImportProductFirstImageCreateRequest createRequest)
    {
        const string insertInFirstImagesQuery =
            $"""
            INSERT INTO {_firstImagesTableName}(ID, Description, Image, ImageFileExt, DateModified)
            VALUES (@productId, @HtmlData, @ImageData, @ImageContentType, @DateModified)
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            HtmlData = createRequest.HtmlData,
            ImageData = createRequest.ImageData,
            ImageContentType = createRequest.ImageContentType,
            DateModified = createRequest.DateModified,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(insertInFirstImagesQuery, parameters);

        return rowsAffected != 0 ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> UpdateInAllImages(XmlImportProductImageUpdateRequest updateRequest)
    {
        const string updateInAllImagesQuery =
            $"""
            UPDATE {_allImagesTableName}
            SET Description = @HtmlData,
                Image = @ImageData,
                ImageFileExt = @ImageContentType,
                DateModified = @DateModified

            WHERE ID = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            HtmlData = updateRequest.HtmlData,
            ImageData = updateRequest.ImageData,
            ImageContentType = updateRequest.ImageContentType,
            DateModified = updateRequest.DateModified,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateInAllImagesQuery, parameters);

        return rowsAffected >= 0 ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> UpdateInFirstImages(XmlImportProductFirstImageUpdateRequest updateRequest)
    {
        const string updateInFirstImagesQuery =
            $"""
            UPDATE {_firstImagesTableName}
            SET Description = @HtmlData,
                Image = @ImageData,
                ImageFileExt = @ImageContentType,
                DateModified = @DateModified

            WHERE ID = @productId;
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            HtmlData = updateRequest.HtmlData,
            ImageData = updateRequest.ImageData,
            ImageContentType = updateRequest.ImageContentType,
            DateModified = updateRequest.DateModified,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateInFirstImagesQuery, parameters);

        return rowsAffected >= 0 ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData)
    {
        const string updateHtmlDataInAllImagesByIdQuery =
            $"""
            IF EXISTS (SELECT 1 FROM {_allImagesTableName} WHERE ID = @imageId)
            BEGIN
                UPDATE {_allImagesTableName}
                SET Description = @HtmlData

                WHERE ID = @imageId;

                SELECT @@ROWCOUNT;            
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        var parameters = new
        {
            imageId = imageId,
            HtmlData = htmlData
        };

        int status = _relationalDataAccess.SaveDataAndReturnValue<int, dynamic>(updateHtmlDataInAllImagesByIdQuery, parameters);

        if (status == -1) return false;

        return status > 0 ? true : new UnexpectedFailureResult();
    }

    public OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData)
    {
        const string updateHtmlDataInFirstImagesByProductIdQuery =
            $"""
            IF EXISTS (SELECT 1 FROM {_firstImagesTableName} WHERE ID = @productId)
            BEGIN
                UPDATE {_firstImagesTableName}
                SET Description = @HtmlData

                WHERE ID = @productId;

                SELECT @@ROWCOUNT;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        var parameters = new
        {
            productId = productId,
            HtmlData = htmlData
        };

        int status = _relationalDataAccess.SaveDataAndReturnValue<int, dynamic>(updateHtmlDataInFirstImagesByProductIdQuery, parameters);

        if (status == -1) return false;

        return status > 0 ? true : new UnexpectedFailureResult();
    }

    public OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData)
    {
        const string updateHtmlDataInFirstAndAllImagesByProductIdQuery =
            $"""
            IF EXISTS (SELECT 1 FROM {_allImagesTableName} WHERE CSTID = @productId)
            OR EXISTS (SELECT 1 FROM {_firstImagesTableName} WHERE ID = @productId)
            BEGIN
                DECLARE @TotalAffectedRows INT = 0;
            
                UPDATE {_allImagesTableName}
                SET Description = @HtmlData
            
                WHERE CSTID = @productId;

                SET @TotalAffectedRows = @TotalAffectedRows + @@ROWCOUNT;

                UPDATE {_firstImagesTableName}
                SET Description = @HtmlData

                WHERE ID = @productId;

                SET @TotalAffectedRows = @TotalAffectedRows + @@ROWCOUNT;

                SELECT @TotalAffectedRows;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        var parameters = new
        {
            productId = productId,
            HtmlData = htmlData
        };

        int status = _relationalDataAccess.SaveDataAndReturnValue<int, dynamic>(updateHtmlDataInFirstAndAllImagesByProductIdQuery, parameters, true);

        if (status == -1) return false;

        return status > 0 ? true : new UnexpectedFailureResult();
    }

    public bool DeleteInAllImagesById(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_allImagesTableName}
            WHERE ID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, new { id = id });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteInAllImagesAndImageFilePathInfosById(int id)
    {
        const string deleteInAllImagesAndImageFilePathInfosByIdQuery =
            $"""
            DECLARE @displayOrderOfDeletedImage INT;
            DECLARE @productIdOfDeletedImage INT;

            DELETE FROM {_allImagesTableName}
            WHERE ID = @id;

            SELECT TOP 1 @displayOrderOfDeletedImage = S, @productIdOfDeletedImage = CSTID
            FROM {_imageFileNameInfosTable}
            WHERE SUBSTRING(ImgFileName, 1, CHARINDEX('.', ImgFileName) - 1) = CAST(@id as VARCHAR);

            DELETE FROM {_imageFileNameInfosTable}
            WHERE SUBSTRING(ImgFileName, 1, CHARINDEX('.', ImgFileName) - 1) = CAST(@id as VARCHAR);

            UPDATE {_imageFileNameInfosTable}
                SET S = S - 1

            WHERE CSTID = @productIdOfDeletedImage
            AND S > @displayOrderOfDeletedImage;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteInAllImagesAndImageFilePathInfosByIdQuery,
                new { id = id });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteInFirstImagesByProductId(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_firstImagesTableName}
            WHERE ID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, new { id = id });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllWithSameProductIdInAllImages(int productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_allImagesTableName}
            WHERE CSTID = @productId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, new { productId = productId });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

#pragma warning restore IDE0037 // Use inferred member name

    private static XmlImportProductImage Map(XmlImportProductFirstImage image)
    {
        return new XmlImportProductImage()
        {
            Id = image.Id,
            ProductId = image.Id,
            HtmlData = image.HtmlData,
            ImageData = image.ImageData,
            ImageContentType = image.ImageContentType,
            DateModified = image.DateModified,
        };
    }
}