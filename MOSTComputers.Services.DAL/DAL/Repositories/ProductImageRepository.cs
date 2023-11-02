using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using static MOSTComputers.Services.DAL.DAL.Repositories.RepositoryCommonElements;
using OneOf;
using OneOf.Types;
using MOSTComputers.Services.DAL.Models.Responses;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductImageRepository : RepositoryBase, IProductImageRepository
{
    private const string _firstImagesTableName = "dbo.Images";
    private const string _allImagesTableName = "dbo.ImagesAll";

    public ProductImageRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<ProductImage> GetAllInProduct(uint productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT ID, CSTID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_allImagesTableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<ProductImage, dynamic>(getAllInProductQuery, new { productId });
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
            WHERE ID IN
            """;

        string queryWithIds = getByIdInFirstImagesQuery + $" ({GetDelimeteredListFromIds(productIds)});";

        IEnumerable<ProductFirstImage> images = _relationalDataAccess.GetData<ProductFirstImage, dynamic>(queryWithIds, new { });

        return images.Select(x => Map(x));
    }

    public ProductImage? GetByIdInAllImages(uint id)
    {
        const string getByIdInAllImagesQuery =
            $"""
            SELECT ID, CSTID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_allImagesTableName}
            WHERE ID = @id;
            """;

        return _relationalDataAccess.GetData<ProductImage, dynamic>(getByIdInAllImagesQuery, new { id }).FirstOrDefault();
    }

    public ProductImage? GetByProductIdInFirstImages(uint productId)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT ID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            FROM {_firstImagesTableName}
            WHERE ID = @productId;
            """;

        ProductFirstImage? image = _relationalDataAccess.GetData<ProductFirstImage, dynamic>(getByIdInFirstImagesQuery, new { productId }).FirstOrDefault();

        if (image is null) return null;

        return Map(image);
    }

    public OneOf<Success, UnexpectedFailureResult> InsertInAllImages(ProductImageCreateRequest createRequest)
    {
        const string insertInAllImagesQuery =
            $"""
            INSERT INTO {_allImagesTableName}(CSTID, Description, Image, ImageFileExt, DateModified)
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

        int rowsAffected = _relationalDataAccess.SaveData<ProductImage, dynamic>(insertInAllImagesQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
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

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
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

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
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
            int rowsAffected = _relationalDataAccess.SaveData<ProductImage, dynamic>(deleteQuery, new { id });

            if (rowsAffected == 0) return false;

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
            int rowsAffected = _relationalDataAccess.SaveData<ProductFirstImage, dynamic>(deleteQuery, new { id });

            if (rowsAffected == 0) return false;

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
            int rowsAffected = _relationalDataAccess.SaveData<ProductImage, dynamic>(deleteQuery, new { productId });

            if (rowsAffected == 0) return false;

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