using static MOSTComputers.Services.DAL.DAL.Repositories.RepositoryCommonElements;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;
using System.Data;
using Dapper;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductRepository : RepositoryBase, IProductRepository
{
    private const string _tableName = "dbo.MOSTPrices";
    private const string _categoriesTableName = "dbo.Categories";
    private const string _manifacturersTableName = "dbo.Manufacturer";
    private const string _firstImagesTableName = "dbo.Images";
    private const string _allImagesTableName = "dbo.ImagesAll";
    private const string _propertiesTableName = "dbo.ProductXML";
    private const string _imageFileNamesTable = "dbo.ImageFileName";

    public ProductRepository(IRelationalDataAccess relationalDataAccess)
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

    public IEnumerable<Product> GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(uint start, uint end)
    {
        const string getFirstBetweenStartAndEnd =
            $"""
            SELECT * FROM
            (
                SELECT TOP (@SelectStart + @SelectEnd) CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                    PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                    CategoryID, ParentId, Description, IsLeaf,
                    man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                    ROW_NUMBER() OVER (ORDER BY products.S) AS RN

                FROM dbo.MOSTPrices products
                LEFT JOIN Categories cat
                ON cat.CategoryID = products.TID
                LEFT JOIN dbo.Manufacturer man
                ON man.MfrID = products.MfrID
            ) AS selectSt

            WHERE @SelectStart < RN;
            """;

        var paramters = new
        {
            SelectStart = (int)start,
            SelectEnd = (int)end,
        };

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(getFirstBetweenStartAndEnd,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            paramters);
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

            new { id = (int)id })
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

            new { id = (int)id })
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

            new { id = (int)id })
            .FirstOrDefault();
    }



    // ====================================================================================================





    // FIX IDS IN THE PRODUCTS AND ALL IMAGES QUERIES





    // ====================================================================================================

    public OneOf<uint, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest)
    {
        const string insertProductQuery =
            $"""
            CREATE TABLE #Temp_Table (ID INT)
            
            INSERT INTO {_tableName} (CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2)
            OUTPUT INSERTED.CSTID INTO #Temp_Table
            VALUES (100000, @CategoryId, @CfgSubType, @ADDWRR, @ADDWRRTERM, @ADDWRRDEF, @DEFWRRTERM, @DisplayOrder, @OLD, @PLSHOW, @PRICE1, @PRICE2, @PRICE3, @CurrencyId, @RowGuid,
                @PromPID, @PromRID, @PromPictureId, @PromExpDate, @AlertPictureId, @AlertExpDate, @PriceListDescription, @ManifacturerId, @SubcategoryId, @SPLMODEL, @SPLMODEL1, @SPLMODEL2)

            SELECT TOP 1 ID FROM #Temp_Table
            """;

        const string insertPropertiesQuery =
            $"""
            INSERT INTO {_propertiesTableName}(CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr)
            VALUES(@ProductId, @ProductCharacteristicId, @DisplayOrder,
            (SELECT Name FROM dbo.ProductKeyword WHERE ProductKeywordID = @ProductCharacteristicId), @Value, @XmlPlacement)
            """;

        const string insertImageFilePathInfosQuery =
            $"""
            INSERT INTO {_imageFileNamesTable}(CSTID, ImgNo, ImgFileName)
            VALUES (@ProductId, @DisplayOrder, @FileName)
            """;

        string insertInAllImagesQuery =
           $"""
            INSERT INTO {_allImagesTableName}(ID, CSTID, Description, Image, ImageFileExt, DateModified)
            VALUES (@Id, @ProductId, @XML, @ImageData, @ImageFileExtension, @DateModified)
            """;

        const string insertInFirstImagesQuery =
            $"""
            INSERT INTO {_firstImagesTableName}(ID, Description, Image, ImageFileExt, DateModified)
            VALUES (@ProductId, @XML, @ImageData, @ImageFileExtension, @DateModified)
            """;

        var parameters = new
        {
            CategoryId = createRequest.CategoryID,
            CfgSubType = createRequest.Name,
            ADDWRR = createRequest.AdditionalWarrantyPrice,
            ADDWRRTERM = createRequest.AdditionalWarrantyTermMonths,
            ADDWRRDEF = createRequest.StandardWarrantyPrice,
            DEFWRRTERM = createRequest.StandardWarrantyTermMonths,
            createRequest.DisplayOrder,
            OLD = createRequest.Status,
            PLSHOW = createRequest.PlShow,
            PRICE1 = createRequest.Price1,
            PRICE2 = createRequest.DisplayPrice,
            PRICE3 = createRequest.Price3,
            CurrencyId = createRequest.Currency,
            createRequest.RowGuid,
            PromPID = createRequest.Promotionid,
            PromRID = createRequest.PromRid,
            PromPictureId = createRequest.PromotionPictureId,
            PromExpDate = createRequest.PromotionExpireDate,
            createRequest.AlertPictureId,
            AlertExpDate = createRequest.AlertExpireDate,
            createRequest.PriceListDescription,
            createRequest.ManifacturerId,
            SubcategoryId = createRequest.SubCategoryId,
            SPLMODEL = createRequest.PartNumber1,
            SPLMODEL1 = createRequest.PartNumber2,
            SPLMODEL2 = createRequest.SearchString,
        };

        OneOf<uint, UnexpectedFailureResult> result = _relationalDataAccess.SaveDataInTransactionUsingAction<Product, dynamic, OneOf<uint, UnexpectedFailureResult>>(InsertAllRecordsInTransaction<dynamic>, parameters);

        return result;

        OneOf<uint, UnexpectedFailureResult> InsertAllRecordsInTransaction<U>(IDbConnection connection, IDbTransaction transaction, U parametersLocal)
        {
            int id = connection.ExecuteScalar<int>(insertProductQuery, parametersLocal, transaction, commandType: CommandType.Text);

            if (createRequest.Properties is not null
                && createRequest.Properties.Count > 0)
            {
                int rowsAffected = connection.Execute(insertPropertiesQuery, createRequest.Properties.Select(x => Map(x, id)), transaction, commandType: CommandType.Text);

                if (rowsAffected == 0)
                {
                    transaction.Rollback();

                    return new UnexpectedFailureResult();
                }
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

                connection.Execute(insertImageFilePathInfosQuery, orderedImageFileNames.Select(x => Map(x, id)), transaction, commandType: CommandType.Text);
            }

            if (createRequest.Images is not null
                && createRequest.Images.Count > 0)
            {
                connection.Execute(insertInAllImagesQuery, createRequest.Images.Select(x => MapToLocal(x, id)), transaction, commandType: CommandType.Text);

                connection.Execute(insertInFirstImagesQuery, Map(createRequest.Images.First(), id), transaction, commandType: CommandType.Text);
            }

            transaction.Commit();

            return (uint)id;
        }
    }

    public OneOf<Success, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest)
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

        var parameters = new
        {
            id = updateRequest.Id,
            CategoryId = updateRequest.CategoryID,
            updateRequest.Name,
            ADDWRR = updateRequest.AdditionalWarrantyPrice,
            ADDWRRTERM = updateRequest.AdditionalWarrantyTermMonths,
            ADDWRRDEF = updateRequest.StandardWarrantyPrice,
            DEFWRRTERM = updateRequest.StandardWarrantyTermMonths,
            updateRequest.DisplayOrder,
            updateRequest.Status,
            PLSHOW = updateRequest.PlShow,
            PRICE1 = updateRequest.Price1,
            PRICE2 = updateRequest.DisplayPrice,
            PRICE3 = updateRequest.Price3,
            CurrencyId = updateRequest.Currency,
            updateRequest.RowGuid,
            PromPID = updateRequest.Promotionid,
            PromRID = updateRequest.PromRid,
            PromPictureId = updateRequest.PromotionPictureId,
            PromExpDate = updateRequest.PromotionExpireDate,
            updateRequest.AlertPictureId,
            AlertExpDate = updateRequest.AlertExpireDate,
            updateRequest.PriceListDescription,
            updateRequest.ManifacturerId,
            SubcategoryId = updateRequest.SubCategoryId,
            SPLMODEL = updateRequest.PartNumber1,
            SPLMODEL1 = updateRequest.PartNumber2,
            SPLMODEL2 = updateRequest.SearchString
        };

        var result = _relationalDataAccess.SaveDataInTransactionUsingAction<Product, dynamic, OneOf<Success, UnexpectedFailureResult>>(
            UpdateAllRecordsInTransaction<dynamic>, parameters);

        return result;

        OneOf<Success, UnexpectedFailureResult> UpdateAllRecordsInTransaction<U>(IDbConnection connection, IDbTransaction transaction, U _)
        {
            connection.Execute(updateProductQuery, parameters, transaction, commandType: CommandType.Text);

            if (updateRequest.Properties is not null
                && updateRequest.Properties.Count > 0)
            {
                int rowsAffected = connection.Execute(updatePropertiesQuery, updateRequest.Properties.Select(x => Map(x, updateRequest.Id)), transaction, commandType: CommandType.Text);

                if (rowsAffected == 0) return new UnexpectedFailureResult();
            }

            if (updateRequest.ImageFileNames is not null
                && updateRequest.ImageFileNames.Count > 0)
            {
                connection.Execute(updateImageFilePathInfosQuery, updateRequest.ImageFileNames
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => Map(x, updateRequest.Id)),
                    transaction,
                    commandType: CommandType.Text);
            }

            if (updateRequest.Images is not null
                && updateRequest.Images.Count > 0)
            {
                connection.Execute(updateInAllImagesQuery, updateRequest.Images.Select(x => Map(x, updateRequest.Id)), transaction, commandType: CommandType.Text);

                connection.Execute(updateInFirstImagesQuery, MapToFirstImage(updateRequest.Images.First(), updateRequest.Id), transaction, commandType: CommandType.Text);
            }

            return new Success();
        }
    }

    public bool Delete(uint id)
    {
        const string deleteProductAndRelatedItemsQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @id;

            DELETE FROM {_propertiesTableName}
            WHERE CSTID = @productId;

            DELETE FROM {_imageFileNamesTable}
            WHERE CSTID = @productId;

            DELETE FROM {_allImagesTableName}
            WHERE CSTID = @productId;

            DELETE FROM {_firstImagesTableName}
            WHERE ID = @productId;
            """;

        //const string deleteProductQuery =
        //    $"""
        //    DELETE FROM {_tableName}
        //    WHERE CSTID = @id;
        //    """;

        //const string deletePropertiesForProductQuery =
        //    $"""
        //    DELETE FROM {_propertiesTableName}
        //    WHERE CSTID = @productId;
        //    """;

        //const string deleteImageFileNamesForProductQuery =
        //    $"""
        //    DELETE FROM {_imageFileNamesTable}
        //    WHERE CSTID = @productId;
        //    """;

        //const string deleteAllImagesForProductQuery =
        //    $"""
        //    DELETE FROM {_allImagesTableName}
        //    WHERE CSTID = @productId;
        //    """;

        //const string deleteFirstForProductImage =
        //    $"""
        //    DELETE FROM {_firstImagesTableName}
        //    WHERE ID = @productId;
        //    """;

        var parameters = new
        {
            id = (int)id
        };

        OneOf<Success, UnexpectedFailureResult> result =
            _relationalDataAccess.SaveDataInTransactionUsingAction<Product, dynamic, OneOf<Success, UnexpectedFailureResult>>(DeleteAllRecordsInTransaction, parameters);

        return result.IsT0;

        OneOf<Success, UnexpectedFailureResult> DeleteAllRecordsInTransaction(IDbConnection connection, IDbTransaction transaction, dynamic _)
        {
            int rowsAffected = connection.Execute(deleteProductAndRelatedItemsQuery, parameters, transaction, commandType: CommandType.Text);

            return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();

            //connection.Execute(deleteProductQuery, parameters, transaction, commandType: CommandType.Text);

            //connection.Execute(deletePropertiesForProductQuery, new { productId = id }, transaction, commandType: CommandType.Text);

            //connection.Execute(deleteImageFileNamesForProductQuery, new { productId = id }, transaction, commandType: CommandType.Text);

            //connection.Execute(deleteAllImagesForProductQuery, new { productId = id }, transaction, commandType: CommandType.Text);

            //connection.Execute(deleteFirstForProductImage, new { productId = id }, transaction, commandType: CommandType.Text);
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

    public class LocalProductImageCreateRequest
    {
        public int? Id { get; set; }
        public int? ProductId { get; set; }
        public string? XML { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageFileExtension { get; set; }
        public DateTime? DateModified { get; set; }
    }

    int V = 40000;

    private LocalProductImageCreateRequest MapToLocal(CurrentProductImageCreateRequest request, int productId)
    {

        LocalProductImageCreateRequest output = new()
        {
            Id = ++V,
            ProductId = productId,
            ImageData = request.ImageData,
            ImageFileExtension = request.ImageFileExtension,
            XML = request.XML,
            DateModified = request.DateModified,
        };

        return output;
    }

    private static ProductImageCreateRequest Map(CurrentProductImageCreateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ImageData = request.ImageData,
            ImageFileExtension = request.ImageFileExtension,
            XML = request.XML,
            DateModified = request.DateModified,
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