using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal class ProductImageRepository : RepositoryBase, IProductImageRepository
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
            SELECT * FROM {_allImagesTableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<ProductImage, dynamic>(getAllInProductQuery, new { productId });
    }

    public IEnumerable<ProductImage> GetAllFirstImagesForAllProducts()
    {
        const string getAllFirstImagesForAllProductsQuery =
            $"""
            SELECT * FROM {_firstImagesTableName}
            """;

        IEnumerable<ProductFirstImage> images = _relationalDataAccess.GetData<ProductFirstImage, dynamic>(getAllFirstImagesForAllProductsQuery, new { });

        return images.Select(image => Map(image));
    }

    public ProductImage? GetByIdInAllImages(uint id)
    {
        const string getByIdInAllImagesQuery =
            $"""
            SELECT * FROM {_allImagesTableName}
            WHERE ID = @id;
            """;

        return _relationalDataAccess.GetData<ProductImage, dynamic>(getByIdInAllImagesQuery, new { id }).FirstOrDefault();
    }

    public ProductImage? GetByProductIdInFirstImages(uint productId)
    {
        const string getByIdInFirstImagesQuery =
            $"""
            SELECT * FROM {_firstImagesTableName}
            WHERE ID = @productId;
            """;

        ProductFirstImage? image = _relationalDataAccess.GetData<ProductFirstImage, dynamic>(getByIdInFirstImagesQuery, new { productId }).FirstOrDefault();

        if (image is null) return null;

        return Map(image);
    }

    public OneOf<Success, ValidationResult> InsertInAllImages(ProductImageCreateRequest createRequest, IValidator<ProductImageCreateRequest>? validator = null)
    {
        const string insertInAllImagesQuery =
            $"""
            INSERT INTO {_allImagesTableName}(CSTID, Description, Image, ImageFileExt, DateModified)
            VALUES (@productId, @XML, @ImageData, @ImageFileExtension, @DateModified)
            """;

        if (validator != null)
        {
            ValidationResult result = validator.Validate(createRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            productId = createRequest.CSTID,
            createRequest.XML,
            createRequest.ImageData,
            createRequest.ImageFileExtension,
            createRequest.DateModified,
        };

        _relationalDataAccess.SaveData<ProductImage, dynamic>(insertInAllImagesQuery, parameters);

        return new Success();
    }

    public OneOf<Success, ValidationResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest, IValidator<ProductFirstImageCreateRequest>? validator = null)
    {
        const string insertInFirstImagesQuery =
            $"""
            INSERT INTO {_firstImagesTableName}(ID, Description, Image, ImageFileExt, DateModified)
            VALUES (@productId, @XML, @ImageData, @ImageFileExtension, @DateModified)
            """;

        if (validator != null)
        {
            ValidationResult result = validator.Validate(createRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.XML,
            createRequest.ImageData,
            createRequest.ImageFileExtension,
            createRequest.DateModified,
        };

        _relationalDataAccess.SaveData<ProductFirstImage, dynamic>(insertInFirstImagesQuery, parameters);

        return new Success();
    }

    public OneOf<Success, ValidationResult> UpdateInAllImages(ProductImageUpdateRequest createRequest, IValidator<ProductImageUpdateRequest>? validator = null)
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

        if (validator != null)
        {
            ValidationResult result = validator.Validate(createRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            id = createRequest.Id,
            createRequest.XML,
            createRequest.ImageData,
            createRequest.ImageFileExtension,
            createRequest.DateModified,
        };

        _relationalDataAccess.SaveData<ProductImage, dynamic>(updateInAllImagesQuery, parameters);

        return new Success();
    }

    public OneOf<Success, ValidationResult> UpdateInFirstImages(ProductFirstImageUpdateRequest updateRequest, IValidator<ProductFirstImageUpdateRequest>? validator = null)
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

        if (validator != null)
        {
            ValidationResult result = validator.Validate(updateRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.XML,
            updateRequest.ImageData,
            updateRequest.ImageFileExtension,
            updateRequest.DateModified,
        };

        _relationalDataAccess.SaveData<ProductFirstImage, dynamic>(updateInFirstImagesQuery, parameters);

        return new Success();
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
            _relationalDataAccess.SaveData<ProductImage, dynamic>(deleteQuery, new { id });
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
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
            _relationalDataAccess.SaveData<ProductFirstImage, dynamic>(deleteQuery, new { id });
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
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
            _relationalDataAccess.SaveData<ProductImage, dynamic>(deleteQuery, new { productId });
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return true;
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