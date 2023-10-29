using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using static MOSTComputers.Services.DAL.DAL.Repositories.RepositoryCommonElements;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Text;
using Dapper;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductRepository : RepositoryBase, IProductRepository
{
    private const string _tableName = "dbo.MOSTPrices";
    private const string _categoriesTableName = "dbo.Categories";
    private const string _manifacturersTableName = "dbo.Manifacturer";
    private const string _firstImagesTableName = "dbo.Images";
    private const string _allImagesTableName = "dbo.ImagesAll";
    private const string _propertiesTableName = "dbo.ProductXML";
    private const string _imageFileNamesTable = "dbo.ImageFileName";

    internal ProductRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategory()
    {
        const string getAllWithManifacturerAndCategoryQuery =
            $"""
            SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active

            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(getAllWithManifacturerAndCategoryQuery,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            new { });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategory_ByIds(List<uint> ids)
    {
        const string getAllWithManifacturerAndCategoryByIdsQuery =
            $"""
            SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active

            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            WHERE CSTID IN
            """;

        string queryWithIds = getAllWithManifacturerAndCategoryByIdsQuery + 
            $"""
            ({GetDelimeteredListFromIds(ids)})
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(queryWithIds,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            new { ids });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(List<uint> ids)
    {
        const string getAllWithManifacturerAndCategoryAndFirstImageByIdsQuery =
            $"""
            SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                ID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified

            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            LEFT JOIN {_firstImagesTableName} firstImages
            ON firstImages.ID = products.CSTID
            WHERE CSTID IN
            """;

        string queryWithIds = getAllWithManifacturerAndCategoryAndFirstImageByIdsQuery +
            $"""
            ({GetDelimeteredListFromIds(ids)})
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductImage, dynamic>(queryWithIds,
            (product, category, manifacturer, image) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                product.Images ??= new List<ProductImage>();

                product.Images.Add(image);

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,ImageProductId",

            new { });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndProperties_ByIds(List<uint> ids)
    {
        const string getAllWithManifacturerAndCategoryAndPropertiesByIdsQuery =
            $"""
            SELECT products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                properties.CSTID AS PropertyProductId, ProductKeywordID, properties.S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr

            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            LEFT JOIN {_propertiesTableName} properties
            ON properties.CSTID = products.CSTID
            WHERE products.CSTID IN
            """;

        string queryWithIds = getAllWithManifacturerAndCategoryAndPropertiesByIdsQuery +
            $"""
            ({GetDelimeteredListFromIds(ids)})
            ORDER BY S; 
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductProperty, dynamic>(queryWithIds,
            (product, category, manifacturer, property) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                product.Properties ??= new();

                product.Properties.Add(property);

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,PropertyProductId",

            new { });
    }

    public Product? GetById_WithManifacturerAndCategoryAndFirstImage(uint id)
    {
        const string getByIdWithManifacturerAndCategoryAndFirstImageQuery =
            $"""
            SELECT products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                ID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            
            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            LEFT JOIN {_firstImagesTableName} firstImages
            ON firstImages.ID = products.CSTID
            WHERE products.CSTID = @id;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductImage, dynamic>(
            getByIdWithManifacturerAndCategoryAndFirstImageQuery,

            (product, category, manifacturer, image) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                product.Images ??= new();

                product.Images.Add(image);

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,ImageProductId",

            new { id })
            .FirstOrDefault();
    }

    public Product? GetById_WithManifacturerAndCategoryAndProperties(uint id)
    {
        const string getByIdWithManifacturerAndCategoryAndPropertiesQuery =
            $"""
            SELECT products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                properties.CSTID AS PropertyProductId, ProductKeywordID, properties.S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr
            
            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            LEFT JOIN {_propertiesTableName} properties
            ON properties.CSTID = products.CSTID
            WHERE products.CSTID = @id;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductProperty, dynamic>(
            getByIdWithManifacturerAndCategoryAndPropertiesQuery,

            (product, category, manifacturer, property) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                product.Properties ??= new();

                product.Properties.Add(property);

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,PropertyProductId",

            new { id })
            .FirstOrDefault();
    }

    public Product? GetById_WithManifacturerAndCategoryAndImages(uint id)
    {
        const string getByIdWithManifacturerAndCategoryAndImagesQuery =
            $"""
            SELECT products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                ID, CSTID AS ImageProductId, Description AS XMLData, Image, ImageFileExt, DateModified
            
            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            LEFT JOIN {_allImagesTableName} images
            ON images.CSTID = products.CSTID
            WHERE products.CSTID = @id;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductImage, dynamic>(
            getByIdWithManifacturerAndCategoryAndImagesQuery,

            (product, category, manifacturer, image) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                product.Images ??= new();

                product.Images.Add(image);

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,ImageProductId",

            new { id })
            .FirstOrDefault();
    }

    public OneOf<Success, ValidationResult> Insert(ProductCreateRequest createRequest, IValidator<ProductCreateRequest>? validator = null)
    {
        const string insertProductQuery =
            $"""
            INSERT INTO {_tableName} (CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2

            VALUES (@Id, @CategoryId, @CfgSubType, @ADDWRR, @ADDWRRTERM, @ADDWRRDEF, @DEFWRRTERM, @DisplayOrder, @OLD, @PLSHOW, @PRICE1, @PRICE2, @PRICE3, @CurrencyId, @RowGuid,
                @PromPID, @PromRID, @PromPictureId, @PromExpDate, @AlertPictureId, @AlertExpDate, @PriceListDescription, @ManifacturerId, @SubcategoryId, @SPLMODEL, @SPLMODEL1, @SPLMODEL2)
            """;

        const string insertPropertiesQuery =
            $"""
            INSERT INTO {_propertiesTableName}(CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr)
            VALUES(@ProductId, @ProductCharacteristicId, @DisplayOrder, @Name, @Value, @XmlPlacement)
            """;

        const string insertImageFilePathInfosQuery =
            $"""
            INSERT INTO {_imageFileNamesTable}(CSTID, ImgNo, FileName)
            VALUES (@ProductId, @DisplayOrderInRange, @FileName)
            """;

        const string insertInAllImagesQuery =
            $"""
            INSERT INTO {_allImagesTableName}(CSTID, Description, Image, ImageFileExt, DateModified)
            VALUES (@CSTID, @XML, @ImageData, @ImageFileExtension, @DateModified)
            """;

        const string insertInFirstImagesQuery =
            $"""
            INSERT INTO {_firstImagesTableName}(ID, Description, Image, ImageFileExt, DateModified)
            VALUES (@ProductId, @XML, @ImageData, @ImageFileExtension, @DateModified)
            """;

        if (validator is not null)
        {
            ValidationResult result = validator.Validate(createRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            createRequest.Id,
            CategoryId = createRequest.CategoryID,
            CfgSubType = createRequest.SearchString,
            ADDWRR = createRequest.AddWrr,
            ADDWRRTERM = createRequest.AddWrrTerm,
            ADDWRRDEF = createRequest.AddWrrDef,
            DEFWRRTERM = createRequest.DefWrrTerm,
            createRequest.DisplayOrder,
            OLD = createRequest.Status,
            PLSHOW = createRequest.PlShow,
            PRICE1 = createRequest.Price1,
            PRICE2 = createRequest.Price2,
            PRICE3 = createRequest.Price3,
            CurrencyId = createRequest.Currency,
            createRequest.RowGuid,
            PromPID = createRequest.PromPid,
            PromRID = createRequest.PromRid,
            createRequest.PromPictureId,
            createRequest.PromExpDate,
            createRequest.AlertPictureId,
            createRequest.AlertExpDate,
            createRequest.PriceListDescription,
            createRequest.ManifacturerId,
            SubcategoryId = createRequest.SubCategoryId,
            SPLMODEL = createRequest.SplModel1,
            SPLMODEL1 = createRequest.SplModel2,
            SPLMODEL2 = createRequest.SplModel3,
        };

        _relationalDataAccess.SaveDataInTransactionUsingAction<Product, dynamic>(InsertAllRecordsInTransaction<dynamic>, parameters);

        return new Success();

        void InsertAllRecordsInTransaction<U>(IDbConnection connection, IDbTransaction transaction, U parametersLocal)
        {
            connection.Execute(insertProductQuery, parametersLocal, transaction, commandType: CommandType.Text);

            if (createRequest.Properties is not null
                && createRequest.Properties.Count > 0)
            {
                connection.Execute(insertPropertiesQuery, createRequest.Properties.Select(x => Map(x, createRequest.Id)), transaction, commandType: CommandType.Text);
            }


            if (createRequest.ImageFileNames is not null
                && createRequest.ImageFileNames.Count > 0)
            {
                List<CurrentProductImageFileNameInfoCreateRequest> orderedImageFileNames = createRequest.ImageFileNames
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();

                for (int i = 0; i < orderedImageFileNames.Count; i++)
                {
                    CurrentProductImageFileNameInfoCreateRequest imageFileNameCreateRequest = orderedImageFileNames[i];

                    imageFileNameCreateRequest.DisplayOrder = i + 1;
                }

                connection.Execute(insertImageFilePathInfosQuery, orderedImageFileNames.Select(x => Map(x, createRequest.Id)), transaction, commandType: CommandType.Text);
            }

            if (createRequest.Images is not null
                && createRequest.Images.Count > 0)
            {
                connection.Execute(insertInAllImagesQuery, createRequest.Images.Select(x => Map(x, createRequest.Id)), transaction, commandType: CommandType.Text);

                connection.Execute(insertInFirstImagesQuery, Map(createRequest.Images.First(), createRequest.Id), transaction, commandType: CommandType.Text);
            }
        }
    }

    public OneOf<Success, ValidationResult> Update(ProductUpdateRequest createRequest, IValidator<ProductUpdateRequest>? validator = null)
    {
        const string updateProductQuery =
            $"""
            UPDATE {_tableName}
            SET TID = @CategoryId,
                CFGSUBTYPE = @SearchString,
                ADDWRR = @ADDWRR,
                ADDWRRTERM = @ADDWRRTERM,
                ADDWRRDEF = @ADDWRRDEF,
                DEFWRRTERM = @DEFWRRTERM,
                S = @DisplayOrder,
                OLD = @Status,
                PLSHOW = @PLSHOW,
                PRICE1 = @PRICE1,
                PRICE2 = @PRICE2,
                PRICE3 = @PRICE3,
                CurrencyId = @CurrencyId,
                rowguid = @RowGuid,
                PromPID = @PromPID,
                PromRID = @PromRID,
                PromPictureID = @PromPictureId,
                PromExpDate = @PromExpDate,
                AlertPictureID = @AlertPictureId,
                AlertExpDate = @AlertExpDate,
                PriceListDescription = @PriceListDescription,
                MfrID = @ManifacturerId,
                SubcategoryID = @SubcategoryId,
                SPLMODEL = @SPLMODEL,
                SPLMODEL1 = @SPLMODEL1,
                SPLMODEL2 = @SPLMODEL2
            
            WHERE CSTID = @id;
            """;

        const string updatePropertiesQuery =
            $"""
            UPDATE {_propertiesTableName}
            SET S = @DisplayOrder,
                Keyword = @Name,
                KeywordValue = @Value,
                Discr = @XmlPlacement
            
            WHERE CSTID = @ProductId
            AND ProductKeywordID = @ProductCharacteristicId;
            """;

        const string updateImageFilePathInfosQuery =
            $"""
            SELECT @NewDisplayOrderInRange = ISNULL(
                (SELECT MAX(ImgNo) + 1 AS MaxNoPlus1 FROM dbo.ImageFileName
                WHERE MaxNoPlus1 <= @NewDisplayOrder),
                @NewDisplayOrder);
            
            UPDATE dbo.ImageFileName
                SET ImgNo = ImgNo + 1
            
            WHERE CSTID = @productId
            AND ImgNo >= @NewDisplayOrderInRange;

            UPDATE {_imageFileNamesTable}
            SET ImgNo = @NewDisplayOrderInRange,
                FileName = @FileName

            WHERE CSTID = @productId
            AND DisplayOrder = @DisplayOrder;
            """;

        const string updateInAllImagesQuery =
            $"""
            UPDATE {_allImagesTableName}
            SET Description = @XML,
                Image = @ImageData,
                ImageFileExt = @ImageFileExtension,
                DateModified = @DateModified

            WHERE ID = @Id;
            """;

        const string updateInFirstImagesQuery =
            $"""
            UPDATE {_firstImagesTableName}
            SET Description = @XML,
                Image = @ImageData,
                ImageFileExt = @ImageFileExtension,
                DateModified = @DateModified

            WHERE ID = @ProductId;
            """;

        if (validator is not null)
        {
            ValidationResult result = validator.Validate(createRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            CategoryId = createRequest.CategoryID,
            createRequest.SearchString,
            ADDWRR = createRequest.AddWrr,
            ADDWRRTERM = createRequest.AddWrrTerm,
            ADDWRRDEF = createRequest.AddWrrDef,
            DEFWRRTERM = createRequest.DefWrrTerm,
            createRequest.DisplayOrder,
            createRequest.Status,
            PLSHOW = createRequest.PlShow,
            PRICE1 = createRequest.Price1,
            PRICE2 = createRequest.Price2,
            PRICE3 = createRequest.Price3,
            CurrencyId = createRequest.Currency,
            createRequest.RowGuid,
            PromPID = createRequest.PromPid,
            PromRID = createRequest.PromRid,
            createRequest.PromPictureId,
            createRequest.PromExpDate,
            createRequest.AlertPictureId,
            createRequest.AlertExpDate,
            createRequest.PriceListDescription,
            createRequest.ManifacturerId,
            SubcategoryId = createRequest.SubCategoryId,
            SPLMODEL = createRequest.SplModel1,
            SPLMODEL1 = createRequest.SplModel2,
            SPLMODEL2 = createRequest.SplModel3
        };

        _relationalDataAccess.SaveDataInTransactionUsingAction<Product, dynamic>(UpdateAllRecordsInTransaction<dynamic>, parameters);

        return new Success();

        void UpdateAllRecordsInTransaction<U>(IDbConnection connection, IDbTransaction transaction, U _)
        {
            connection.Execute(updateProductQuery, parameters, transaction, commandType: CommandType.Text);

            if (createRequest.Properties is not null
                && createRequest.Properties.Count > 0)
            {
                connection.Execute(updatePropertiesQuery, createRequest.Properties.Select(x => Map(x, createRequest.Id)), transaction, commandType: CommandType.Text);
            }

            if (createRequest.ImageFileNames is not null
                && createRequest.ImageFileNames.Count > 0)
            {
                connection.Execute(updateImageFilePathInfosQuery, createRequest.ImageFileNames
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => Map(x, createRequest.Id)),
                    transaction,
                    commandType: CommandType.Text);
            }

            if (createRequest.Images is not null
                && createRequest.Images.Count > 0)
            {
                connection.Execute(updateInAllImagesQuery, createRequest.Images.Select(x => Map(x, createRequest.Id)), transaction, commandType: CommandType.Text);

                connection.Execute(updateInFirstImagesQuery, MapToFirstImage(createRequest.Images.First(), createRequest.Id), transaction, commandType: CommandType.Text);
            }
        }
    }

    public bool Delete(uint id)
    {
        const string deleteProductQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @id;
            """;

        const string deletePropertiesForProductQuery =
            $"""
            DELETE FROM {_propertiesTableName}
            WHERE CSTID = @productId;
            """;

        const string deleteImageFileNamesForProductQuery =
            $"""
            DELETE FROM {_imageFileNamesTable}
            WHERE CSTID = @productId;
            """;

        const string deleteAllImagesForProductQuery =
            $"""
            DELETE FROM {_allImagesTableName}
            WHERE CSTID = @productId;
            """;

        const string deleteFirstForProductImage =
            $"""
            DELETE FROM {_firstImagesTableName}
            WHERE ID = @productId;
            """;

        var parameters = new
        {
            id
        };

        _relationalDataAccess.SaveDataInTransactionUsingAction<Product, dynamic>(DeleteAllRecordsInTransaction, parameters);

        return true;

        void DeleteAllRecordsInTransaction(IDbConnection connection, IDbTransaction transaction, dynamic _)
        {
            connection.Execute(deleteProductQuery, parameters, transaction, commandType: CommandType.Text);

            connection.Execute(deletePropertiesForProductQuery, new { productId = id }, transaction, commandType: CommandType.Text);

            connection.Execute(deleteImageFileNamesForProductQuery, new { productId = id }, transaction, commandType: CommandType.Text);

            connection.Execute(deleteAllImagesForProductQuery, new { productId = id }, transaction, commandType: CommandType.Text);

            connection.Execute(deleteFirstForProductImage, new { productId = id }, transaction, commandType: CommandType.Text);
        }
    }

    

    private static ProductPropertyCreateRequest Map(CurrentProductPropertyCreateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ProductCharacteristicId = request.ProductCharacteristicId,
            DisplayOrder = request.DisplayOrder,
            Value = request.Value,
            XmlPlacement = request.XmlPlacement,
        };
    }

    private static ProductImageFileNameInfoCreateRequest Map(CurrentProductImageFileNameInfoCreateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            DisplayOrder = request.DisplayOrder,
            FileName = request.FileName,
        };
    }

    private static ProductImageCreateRequest Map(CurrentProductImageCreateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ImageData = request.ImageData,
            ImageFileExtension = request.ImageFileExtension,
            XML = request.XML,
        };
    }

    private static ProductPropertyUpdateRequest Map(CurrentProductPropertyUpdateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ProductCharacteristicId = request.ProductCharacteristicId,
            DisplayOrder = request.DisplayOrder,
            Value = request.Value,
            XmlPlacement = request.XmlPlacement,
        };
    }

    private static ProductImageFileNameInfoUpdateRequest Map(CurrentProductImageFileNameInfoUpdateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            DisplayOrder = request.DisplayOrder,
            NewDisplayOrder = request.NewDisplayOrder,
            FileName = request.FileName,
        };
    }

    private static ProductImageUpdateRequest Map(CurrentProductImageUpdateRequest request, int productId)
    {
        return new()
        {
            Id = productId,
            ImageData = request.ImageData,
            ImageFileExtension = request.ImageFileExtension,
            XML = request.XML,
        };
    }

    private static ProductFirstImageUpdateRequest MapToFirstImage(CurrentProductImageUpdateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ImageData = request.ImageData,
            ImageFileExtension = request.ImageFileExtension,
            XML = request.XML,
        };
    }
}