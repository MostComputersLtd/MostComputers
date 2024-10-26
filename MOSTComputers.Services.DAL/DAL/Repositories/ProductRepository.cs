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
using FluentValidation.Results;
using static MOSTComputers.Services.DAL.DAL.Repositories.RepositoryCommonElements;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductRepository : RepositoryBase, IProductRepository
{
    private const string _tableName = "dbo.MOSTPrices";
    private const string _categoriesTableName = "dbo.Categories";
    private const string _manifacturersTableName = "dbo.Manufacturer";
    private const string _firstImagesTableName = "dbo.Images";
    private const string _allImagesTableName = "dbo.ImagesAll";
    private const string _propertiesTableName = "dbo.ProductXML";
    private const string _productCharacteristicsTableName = "dbo.ProductKeyword";
    private const string _imageFileNamesTableName = "dbo.ImageFileName";
    private const string _productStatusesTableName = "dbo.ProductStatuses";

    public ProductRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name
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

    public IEnumerable<Product> GetAllInCategory_WithManifacturerAndCategory(int categoryId)
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
            WHERE TID = @categoryId
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

            new { categoryId = categoryId });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategory_WhereSearchNameContainsSubstring(string subString)
    {
        const string getAllWhereNameMatchesWithManifacturerAndCategoryQuery =
            $"""
            SELECT * FROM
            (
                SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                    PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                    CategoryID, ParentId, Description, IsLeaf,
                    man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                    CHARINDEX(@subString, products.CFGSUBTYPE) AS SubPosition

                FROM {_tableName} products
                LEFT JOIN {_categoriesTableName} cat
                ON cat.CategoryID = products.TID
                LEFT JOIN {_manifacturersTableName} man
                ON man.MfrID = products.MfrID
            )
                AS Data
            WHERE SubPosition <> 0
            ORDER BY SubPosition, S;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(getAllWhereNameMatchesWithManifacturerAndCategoryQuery,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            new { subString = subString });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(string searchStringParts)
    {
        const string getAllWhereSearchStringMatchesAllSearchStringPartsQuery =
            $"""
            DECLARE @TEMP_TABLE_GET_ALL_WHERE_SEARCHSTRING_MATCHES TABLE (SearchStringPart VARCHAR(50));
            
            DECLARE @StartIndex INT = 1;

            SET @searchStringParts = TRIM(@searchStringParts);

            WHILE (@StartIndex != 0)
            BEGIN
                SET @StartIndex = CHARINDEX(' ', @searchStringParts);

                INSERT INTO @TEMP_TABLE_GET_ALL_WHERE_SEARCHSTRING_MATCHES(SearchStringPart)
                VALUES (
                    CASE
                        WHEN @StartIndex != 0 THEN SUBSTRING(@searchStringParts, 0, @StartIndex - 1)
                        ELSE @searchStringParts
                    END);

                SET @searchStringParts = RIGHT(@searchStringParts, LEN(@searchStringParts) - @StartIndex)

                IF (LEN(@searchStringParts) = 0) BREAK;
            END

            SELECT * FROM
            (
                SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                    PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                    CategoryID, ParentId, Description, IsLeaf,
                    man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                        (SELECT SUM(LocalPosition) FROM
                            (SELECT CHARINDEX(SearchStringPart, products.SPLMODEL2 COLLATE Cyrillic_General_CI_AS) AS LocalPosition
                                FROM @TEMP_TABLE_GET_ALL_WHERE_SEARCHSTRING_MATCHES) AS SearchStringCharInd) AS SubPosition

                FROM {_tableName} products
                LEFT JOIN {_categoriesTableName} cat
                ON cat.CategoryID = products.TID
                LEFT JOIN {_manifacturersTableName} man
                ON man.MfrID = products.MfrID
            )
                AS Data
            WHERE SubPosition <> 0
            ORDER BY SubPosition, S;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(getAllWhereSearchStringMatchesAllSearchStringPartsQuery,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            new { searchStringParts = searchStringParts });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategory_ByIds(List<int> ids)
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

            new { });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(List<int> ids)
    {
        const string getAllWithManifacturerAndCategoryAndFirstImageByIdsQuery =
            $"""
            SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                ID AS ImageProductId, firstImages.Description AS HtmlData, Image, ImageFileExt, DateModified

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

                if (image is not null)
                {
                    product.Images.Add(image);
                }

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,ImageProductId",

            new { });
    }

    public IEnumerable<Product> GetAll_WithManifacturerAndCategoryAndProperties_ByIds(List<int> ids)
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
            WHERE products.CSTID IN @productIds
            ORDER BY S;
            """;
        
        Dictionary<int, Product> productsAndIdsDict = new();

        _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductProperty, dynamic>(getAllWithManifacturerAndCategoryAndPropertiesByIdsQuery,
            (product, category, manifacturer, property) =>
            {
                bool exists = productsAndIdsDict.TryGetValue(product.Id, out Product? productSaved);

                if (exists)
                {
                    if (property is not null)
                    {
                        productSaved!.Properties.Add(property);
                    }

                    return product;
                }

                product.Category = category;
                product.Manifacturer = manifacturer;

                product.Properties ??= new();

                if (property is not null)
                {
                    product.Properties.Add(property);
                }

                productsAndIdsDict.Add(product.Id, product);

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,PropertyProductId",

            new { productIds = ids } );

        return productsAndIdsDict.Values;
    }

    public IEnumerable<Product> GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(int start, uint end)
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

                FROM {_tableName} products
                LEFT JOIN Categories cat
                ON cat.CategoryID = products.TID
                LEFT JOIN {_manifacturersTableName} man
                ON man.MfrID = products.MfrID
            ) AS selectSt

            WHERE @SelectStart < RN
            AND RN <= @SelectEnd;
            """;

        var paramters = new
        {
            SelectStart = start,
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
    
    public IEnumerable<Product> GetFirstInRange_WithManifacturerAndCategory_WhereNameContainsSubstring(int start, uint end, string subString)
    {
        const string getAllWithManifacturerAndCategoryQuery =
            $"""
            SELECT * FROM
            (
                SELECT TOP (@start + @end) CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, rowguid,
                    PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                    CategoryID, ParentId, Description, IsLeaf,
                    PersonalManifacturerId, BGName, Name, ManifacturerDisplayOrder, Active,
                    SubPosition,
                    ROW_NUMBER() OVER (ORDER BY SubPosition, S) AS RN
                FROM
                (
                    SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                        PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                        CategoryID, ParentId, Description, IsLeaf,
                        man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                        CHARINDEX(@subString, products.CFGSUBTYPE) AS SubPosition

                    FROM {_tableName} products
                    LEFT JOIN {_categoriesTableName} cat
                    ON cat.CategoryID = products.TID
                    LEFT JOIN {_manifacturersTableName} man
                    ON man.MfrID = products.MfrID
                    WHERE CHARINDEX(@subString, products.CFGSUBTYPE) > 0
                )
                    AS Data
            ) AS TopData

            WHERE RN BETWEEN @start + 1 AND @end;
            """;


        var parameters = new
        {
            start = start,
            end = (int)end,
            subString = subString
        };

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(getAllWithManifacturerAndCategoryQuery,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            parameters);
    }
    
    public IEnumerable<Product> GetFirstInRange_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(
        int start,
        uint end,
        string searchStringParts)
    {
        const string getFistInRangeWhereSearchStringMatchesAllSearchStringPartsQuery =
            $"""
            DECLARE @TEMP_TABLE_GET_FIRST_IN_RANGE_WHERE_SEARCHSTRING_MATCHES TABLE (SearchStringPart VARCHAR(50));
            
            DECLARE @StartIndex INT = 1;

            SET @searchStringParts = TRIM(@searchStringParts);

            WHILE (@StartIndex != 0)
            BEGIN
                SET @StartIndex = CHARINDEX(' ', @searchStringParts);

                INSERT INTO @TEMP_TABLE_GET_FIRST_IN_RANGE_WHERE_SEARCHSTRING_MATCHES(SearchStringPart)
                VALUES (
                    CASE
                        WHEN @StartIndex != 0 THEN SUBSTRING(@searchStringParts, 0, @StartIndex - 1)
                        ELSE @searchStringParts
                    END);

                SET @searchStringParts = RIGHT(@searchStringParts, LEN(@searchStringParts) - @StartIndex)

                IF (LEN(@searchStringParts) = 0) BREAK;
            END

            SELECT * FROM
            (
                SELECT TOP (@start + @end) CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, rowguid,
                    PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                    CategoryID, ParentId, Description, IsLeaf,
                    PersonalManifacturerId, BGName, Name, ManifacturerDisplayOrder, Active, SubPosition,
                    ROW_NUMBER() OVER (ORDER BY SubPosition, S) AS RN FROM
                (
                    SELECT CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                        PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                        CategoryID, ParentId, Description, IsLeaf,
                        man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                            (SELECT SUM(LocalPosition) FROM
                                (SELECT CHARINDEX(SearchStringPart, products.SPLMODEL2 COLLATE Cyrillic_General_CI_AS) AS LocalPosition
                                    FROM @TEMP_TABLE_GET_FIRST_IN_RANGE_WHERE_SEARCHSTRING_MATCHES) AS SearchStringCharInd) AS SubPosition

                    FROM {_tableName} products
                    LEFT JOIN {_categoriesTableName} cat
                    ON cat.CategoryID = products.TID
                    LEFT JOIN {_manifacturersTableName} man
                    ON man.MfrID = products.MfrID
                    WHERE 0 < ISNULL(
                        (SELECT SUM(LocalPos) FROM
                            (SELECT CHARINDEX(SearchStringPart, products.SPLMODEL2 COLLATE Cyrillic_General_CI_AS) AS LocalPos
                                FROM @TEMP_TABLE_GET_FIRST_IN_RANGE_WHERE_SEARCHSTRING_MATCHES) AS SearchStringSubPos
                                    HAVING MIN(LocalPos) > 0), 0)
                )
                    AS Data
            ) AS TopData
            WHERE RN BETWEEN @start + 1 AND @end;
            """;

        var parameters = new
        {
            searchStringParts = searchStringParts,
            start = start,
            end = (int)end,
        };

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(getFistInRangeWhereSearchStringMatchesAllSearchStringPartsQuery,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            parameters);
    }

    public IEnumerable<Product> GetFirstInRange_WithManifacturerAndCategoryAndStatuses_WhereAllConditionsAreMet(
        int start,
        uint end,
        ProductConditionalSearchRequest productConditionalSearchRequest)
    {
        const string searchStringSubPosition = "SearchStringSubPosition";
        const string nameSubPosition = "NameSubPosition";

        string getFirstInRangeWhenConditionsAreMet = GetQueryFromRequest(productConditionalSearchRequest);

        var parameters = new
        {
            start = start,
            end = (int)end,
            searchStringParts = productConditionalSearchRequest.SearchStringSubstring,
            NameSubstring = productConditionalSearchRequest.NameSubstring,
            CategoryId = productConditionalSearchRequest.CategoryId,
            Status = productConditionalSearchRequest.Status,
            IsProcessed = productConditionalSearchRequest.IsProcessed,
            NeedsToBeUpdated = productConditionalSearchRequest.NeedsToBeUpdated,
        };

       return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(getFirstInRangeWhenConditionsAreMet,
            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            parameters);

        string GetWhereOrAndSqlBasedOnIfThereIsAWhereAlready(bool isWhereAlreadyWritten)
        {
            return isWhereAlreadyWritten ? "AND" : "WHERE";
        }

        string GetOrderByStringFromRequest(ProductConditionalSearchRequest productConditionalSearchRequest)
        {
            if (productConditionalSearchRequest.SearchStringSubstring is null
            && productConditionalSearchRequest.NameSubstring is null)
            {
                return "ORDER BY S";
            }

            if (productConditionalSearchRequest.SearchStringSubstring is not null
            && productConditionalSearchRequest.NameSubstring is not null)
            {
                return $"ORDER BY {searchStringSubPosition} + {nameSubPosition}, S";
            }

            else if (productConditionalSearchRequest.SearchStringSubstring is not null)
            {
                return $"ORDER BY {searchStringSubPosition}, S";
            }
            else
            {
                return $"ORDER BY {nameSubPosition}, S";
            }
        }

        string GetQueryFromRequest(ProductConditionalSearchRequest productConditionalSearchRequest)
        {
            string orderByString = GetOrderByStringFromRequest(productConditionalSearchRequest);

            string queryStart =
                $"""
                DECLARE @TEMP_TABLE_GET_ALL_WHERE_SEARCHSTRING_MATCHES TABLE (SearchStringPart VARCHAR(50));
                
                DECLARE @StartIndex INT = 1;
                
                SET @searchStringParts = TRIM(@searchStringParts);
                
                WHILE (@StartIndex != 0)
                BEGIN
                    SET @StartIndex = CHARINDEX(' ', @searchStringParts);
                
                    INSERT INTO @TEMP_TABLE_GET_ALL_WHERE_SEARCHSTRING_MATCHES(SearchStringPart)
                    VALUES (
                        CASE
                            WHEN @StartIndex != 0 THEN SUBSTRING(@searchStringParts, 0, @StartIndex)
                            ELSE @searchStringParts
                        END);
                
                    SET @searchStringParts = RIGHT(@searchStringParts, LEN(@searchStringParts) - @StartIndex)
                
                    IF (LEN(@searchStringParts) = 0) BREAK;
                END
                            
                SELECT * FROM
                (
                    SELECT TOP (@start + @end) CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, rowguid,
                        PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                        CategoryID, ParentId, Description, IsLeaf,
                        PersonalManifacturerId, BGName, Name, ManifacturerDisplayOrder, Active
                """;
                
            string queryFirstSelect =
                $"""
                ,
                        ROW_NUMBER() OVER ({orderByString}) AS RN
                    FROM
                    (
                        SELECT products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                            PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                            CategoryID, ParentId, Description, IsLeaf,
                            man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active
                """;

            string querySecondSelect =
                $"""
                        FROM {_tableName} products
                        LEFT JOIN {_categoriesTableName} cat
                        ON cat.CategoryID = products.TID
                        LEFT JOIN {_manifacturersTableName} man
                        ON man.MfrID = products.MfrID
                """;

            string queryWhereStatementInSelect = "";

            string queryEnd =
                """
                    ) AS Data
                ) AS TopData

                WHERE RN BETWEEN @start + 1 AND @end;
                """;

            bool hasWhereStatementInSelectPart = false;

            if (productConditionalSearchRequest.SearchStringSubstring is not null)
            {
                queryStart += $", {searchStringSubPosition}";

                queryFirstSelect +=
                    $"""
                    ,
                            (SELECT SUM(LocalPosition) FROM
                                (SELECT CHARINDEX(SearchStringPart, products.SPLMODEL2 COLLATE Cyrillic_General_CI_AS) AS LocalPosition
                                    FROM @TEMP_TABLE_GET_ALL_WHERE_SEARCHSTRING_MATCHES) AS SearchStringCharInd) AS {searchStringSubPosition}
                    """;
                
                queryWhereStatementInSelect +=
                    $"""
                        {GetWhereOrAndSqlBasedOnIfThereIsAWhereAlready(hasWhereStatementInSelectPart)} 0 < ISNULL(
                            (SELECT SUM(LocalPos) FROM
                                (SELECT CHARINDEX(SearchStringPart, products.SPLMODEL2 COLLATE Cyrillic_General_CI_AS) AS LocalPos
                                    FROM @TEMP_TABLE_GET_ALL_WHERE_SEARCHSTRING_MATCHES) AS SearchStringSubPos
                                    HAVING MIN(LocalPos) > 0), 0)
                    """;

                hasWhereStatementInSelectPart = true;
            }

            if (productConditionalSearchRequest.NameSubstring is not null)
            {
                queryStart += $", {nameSubPosition}";

                queryFirstSelect +=
                    $"""
                    ,
                    CHARINDEX(@NameSubstring, products.CFGSUBTYPE) AS {nameSubPosition}
                    """;

                queryWhereStatementInSelect += $" {GetWhereOrAndSqlBasedOnIfThereIsAWhereAlready(hasWhereStatementInSelectPart)} CHARINDEX(@NameSubstring, products.CFGSUBTYPE) > 0";

                hasWhereStatementInSelectPart = true;
            }

            if (productConditionalSearchRequest.Status is not null)
            {
                queryWhereStatementInSelect += $" {GetWhereOrAndSqlBasedOnIfThereIsAWhereAlready(hasWhereStatementInSelectPart)} products.OLD = @Status";

                hasWhereStatementInSelectPart = true;
            }

            if (productConditionalSearchRequest.CategoryId is not null)
            {
                queryWhereStatementInSelect += $" {GetWhereOrAndSqlBasedOnIfThereIsAWhereAlready(hasWhereStatementInSelectPart)} products.TID = @CategoryId";

                hasWhereStatementInSelectPart = true;
            }

            if (productConditionalSearchRequest.IsProcessed is not null
                || productConditionalSearchRequest.NeedsToBeUpdated is not null)
            {
                querySecondSelect +=
                    $"""

                    INNER JOIN {_productStatusesTableName} productStatuses
                    ON productStatuses.CSTID = products.CSTID
                    """;

                if (productConditionalSearchRequest.IsProcessed is not null)
                {
                    queryWhereStatementInSelect += $" {GetWhereOrAndSqlBasedOnIfThereIsAWhereAlready(hasWhereStatementInSelectPart)} productStatuses.IsProcessed = @IsProcessed";
                }

                if (productConditionalSearchRequest.NeedsToBeUpdated is not null)
                {
                    queryWhereStatementInSelect += $" {GetWhereOrAndSqlBasedOnIfThereIsAWhereAlready(hasWhereStatementInSelectPart)} productStatuses.NeedsToBeUpdated = @NeedsToBeUpdated";
                }
            }

            return
                $"""
                {queryStart + queryFirstSelect}
                {querySecondSelect}
                {queryWhereStatementInSelect}
                {queryEnd}
                """;
        }
    }

    public Product? GetById_WithManifacturerAndCategoryAndFirstImage(int id)
    {
        const string getByIdWithManifacturerAndCategoryAndFirstImageQuery =
            $"""
            SELECT products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                ID AS ImageProductId, firstImages.Description AS HtmlData, Image, ImageFileExt, DateModified
            
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

                if (image is not null)
                {
                    product.Images.Add(image);
                }

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,ImageProductId",

            new { id = id })
            .FirstOrDefault();
    }

    public Product? GetById_WithManifacturerAndCategoryAndProperties(int id)
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

        Product? output = null;

        _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductProperty, dynamic>(
            getByIdWithManifacturerAndCategoryAndPropertiesQuery,

            (product, category, manifacturer, property) =>
            {
                if (output is null)
                {
                    output ??= product;
                    output.Category = category;
                    output.Manifacturer = manifacturer;

                    output.Properties = new();
                }
                if (property is not null)
                {
                    output.Properties.Add(property);
                }

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,PropertyProductId",

            new { id = id });

        return output;
    }

    public Product? GetById_WithManifacturerAndCategoryAndImages(int id)
    {
        const string getByIdWithManifacturerAndCategoryAndImagesQuery =
            $"""
            SELECT products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf, cat.S AS CategoryDisplayOrder, cat.rowguid AS CategoryRowGuid, ProductsUpdateCounter,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active,
                images.CSTID AS ImageProductId, images.ID AS ImagePrime, images.Description AS HtmlData, Image, ImageFileExt, DateModified
            
            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            LEFT JOIN {_allImagesTableName} images
            ON images.CSTID = products.CSTID
            WHERE products.CSTID = @id;
            """;

        Product? output = null;

        _relationalDataAccess.GetData<Product, Category, Manifacturer, ProductImage, dynamic>(
            getByIdWithManifacturerAndCategoryAndImagesQuery,

            (product, category, manifacturer, image) =>
            {
                if (output is null)
                {
                    output = product;

                    output.Category = category;
                    output.Manifacturer = manifacturer;

                    output.Images = new();
                }

                if (image is not null)
                {
                    output.Images!.Add(image);
                }

                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId,ImageProductId",

            new { id = id });

        return output;
    }

    public Product? GetProductWithHighestId_WithManifacturerAndCategory()
    {
        const string getByIdWithManifacturerAndCategoryAndFirstImageQuery =
            $"""
            SELECT TOP 1 products.CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, products.S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, products.rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, products.MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2,
                CategoryID, ParentId, cat.Description, IsLeaf,
                man.MfrID AS PersonalManifacturerId, BGName, Name, man.S AS ManifacturerDisplayOrder, Active
            
            FROM {_tableName} products
            LEFT JOIN {_categoriesTableName} cat
            ON cat.CategoryID = products.TID
            LEFT JOIN {_manifacturersTableName} man
            ON man.MfrID = products.MfrID
            ORDER BY products.CSTID DESC;
            """;

        return _relationalDataAccess.GetData<Product, Category, Manifacturer, dynamic>(
            getByIdWithManifacturerAndCategoryAndFirstImageQuery,

            (product, category, manifacturer) =>
            {
                product.Category = category;
                product.Manifacturer = manifacturer;
                
                return product;
            },
            splitOn: "CategoryID,PersonalManifacturerId",

            new { })
            .FirstOrDefault();
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest)
    {
        const string insertProductQuery =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);
            
            INSERT INTO {_tableName} (CSTID, TID, CFGSUBTYPE, ADDWRR, ADDWRRTERM, ADDWRRDEF, DEFWRRTERM, S, OLD, PLSHOW, PRICE1, PRICE2, PRICE3, CurrencyId, rowguid,
                PromPID, PromRID, PromPictureID, PromExpDate, AlertPictureID, AlertExpDate, PriceListDescription, MfrID, SubcategoryID, SPLMODEL, SPLMODEL1, SPLMODEL2)
            OUTPUT INSERTED.CSTID INTO @InsertedIdTable
            VALUES (ISNULL((SELECT MAX(CSTID) + 1 FROM {_tableName}), 1), @CategoryId, @CfgSubType, @ADDWRR, @ADDWRRTERM, @ADDWRRDEF, @DEFWRRTERM, @DisplayOrder, @OLD, @PLSHOW, @PRICE1, @PRICE2, @PRICE3, @CurrencyId, @RowGuid,
                @PromPID, @PromRID, @PromPictureId, @PromExpDate, @AlertPictureId, @AlertExpDate, @PriceListDescription, @ManifacturerId, @SubcategoryId, @SPLMODEL, @SPLMODEL1, @SPLMODEL2)

            SELECT ISNULL((SELECT TOP 1 Id FROM @InsertedIdTable), 0);
            """;

        const string getCharacteristicsByIdQuery =
            $"""
            SELECT * FROM {_productCharacteristicsTableName}
            WHERE ProductKeywordID IN @characteristicIds;
            """;

        const string insertPropertiesQuery =
            $"""
            DECLARE @DefaultDisplayOrder INT, @Name VARCHAR(50);

            SELECT @DefaultDisplayOrder = S, @Name = Name
            FROM {_productCharacteristicsTableName}
            WHERE ProductKeywordID = @ProductCharacteristicId;

            INSERT INTO {_propertiesTableName} (CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr)
            SELECT @ProductId, @ProductCharacteristicId, ISNULL(@CustomDisplayOrder, @DefaultDisplayOrder), @Name, @Value, @XmlPlacement
            """;

        const string insertImageFileNameInfosQuery =
            $"""
            INSERT INTO {_imageFileNamesTableName} (CSTID, ImageNumber, S, ImgFileName, Active)
            VALUES (@ProductId, ISNULL((SELECT MAX(ImageNumber) + 1 FROM {_imageFileNamesTableName} WHERE CSTID = @ProductId), 1),
                @DisplayOrder, @FileName, @Active)
            """;

        const string insertInAllImagesQuery =
           $"""
            INSERT INTO {_allImagesTableName}(ID, CSTID, Description, Image, ImageFileExt, DateModified)
            VALUES (@Id, @ProductId, @HtmlData, @ImageData, @ImageContentType, @DateModified)
            """;

        const string insertInFirstImagesQuery =
            $"""
            INSERT INTO {_firstImagesTableName}(ID, Description, Image, ImageFileExt, DateModified)
            VALUES (@ProductId, @HtmlData, @ImageData, @ImageContentType, @DateModified)
            """;

        OneOf<int, ValidationResult, UnexpectedFailureResult> result
            = _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition(
                InsertAllRecordsInTransaction,
                resultLocal => resultLocal.Match(
                    id => true,
                    validationResult => false,
                    unexpectedFailureResult => false),
                createRequest);

        return result;

        OneOf<int, ValidationResult, UnexpectedFailureResult> InsertAllRecordsInTransaction(IDbConnection connection, ProductCreateRequest createRequest)
        {
            var parameters = new
            {
                CategoryId = createRequest.CategoryId,
                CfgSubType = createRequest.Name,
                ADDWRR = createRequest.AdditionalWarrantyPrice,
                ADDWRRTERM = createRequest.AdditionalWarrantyTermMonths,
                ADDWRRDEF = createRequest.StandardWarrantyPrice,
                DEFWRRTERM = createRequest.StandardWarrantyTermMonths,
                DisplayOrder = createRequest.DisplayOrder,
                OLD = createRequest.Status,
                PLSHOW = createRequest.PlShow,
                PRICE1 = createRequest.Price1,
                PRICE2 = createRequest.DisplayPrice,
                PRICE3 = createRequest.Price3,
                CurrencyId = createRequest.Currency,
                RowGuid = createRequest.RowGuid,
                PromPID = createRequest.PromotionId,
                PromRID = createRequest.PromRid,
                PromPictureId = createRequest.PromotionPictureId,
                PromExpDate = createRequest.PromotionExpireDate,
                AlertPictureId = createRequest.AlertPictureId,
                AlertExpDate = createRequest.AlertExpireDate,
                PriceListDescription = createRequest.PriceListDescription,
                ManifacturerId = createRequest.ManifacturerId,
                SubcategoryId = createRequest.SubCategoryId,
                SPLMODEL = createRequest.PartNumber1,
                SPLMODEL1 = createRequest.PartNumber2,
                SPLMODEL2 = createRequest.SearchString,
            };

            int newProductId = connection.ExecuteScalar<int>(insertProductQuery, parameters, commandType: CommandType.Text);

            if (createRequest.Properties is not null
                && createRequest.Properties.Count > 0)
            {
                IEnumerable<int> characteristicIds = createRequest.Properties.Select(x => x.ProductCharacteristicId);

                var relevantCharacteristicsQueryParameters = new
                {
                    characteristicIds = characteristicIds,
                };

                IEnumerable<ProductCharacteristic> characteristics = _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(
                    getCharacteristicsByIdQuery, relevantCharacteristicsQueryParameters);

                ValidationResult propertiesValidationResult = new();

                foreach (int characteristicId in characteristicIds)
                {
                    ProductCharacteristic? matchingCharacteristic = characteristics.FirstOrDefault(x => x.Id == characteristicId);

                    if (matchingCharacteristic is null)
                    {
                        AddValidationErrorsForInsertWithCharacteristicIdResult(-1, propertiesValidationResult);
                    }
                    else if (matchingCharacteristic.CategoryId != createRequest.CategoryId)
                    {
                        AddValidationErrorsForInsertWithCharacteristicIdResult(-3, propertiesValidationResult);
                    }
                }

                if (!propertiesValidationResult.IsValid)
                {
                    return propertiesValidationResult;
                }

                IEnumerable<ProductPropertyByCharacteristicIdCreateRequest> propertyCreateRequests
                    = createRequest.Properties.Select(x => Map(x, newProductId));

                int rowsAffectedByPropertyInsert = connection.Execute(insertPropertiesQuery, propertyCreateRequests, commandType: CommandType.Text);

                if (rowsAffectedByPropertyInsert < createRequest.Properties.Count) return new UnexpectedFailureResult();
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

                IEnumerable<ProductImageFileNameInfoCreateRequest> imageFileNameInfoCreateRequests
                    = orderedImageFileNames.Select(x => Map(x, newProductId));

                int rowsAffectedByImageFileNameInsert = connection.Execute(
                    insertImageFileNameInfosQuery, imageFileNameInfoCreateRequests, commandType: CommandType.Text);

                if (rowsAffectedByImageFileNameInsert < createRequest.ImageFileNames.Count) return new UnexpectedFailureResult();
            }

            if (createRequest.Images is not null
                && createRequest.Images.Count > 0)
            {
                List<LocalProductImageCreateRequest> imageCreateRequests = MapToLocalAllImages(createRequest.Images, newProductId, connection);

                int rowsAffectedByImageInsert = connection.Execute(insertInAllImagesQuery, imageCreateRequests, commandType: CommandType.Text);

                if (rowsAffectedByImageInsert < createRequest.Images.Count) return new UnexpectedFailureResult();

                ProductImageCreateRequest firstImageCreateRequest = Map(createRequest.Images[0], newProductId);

                int rowsAffectedByFirstImageInsert = connection.Execute(insertInFirstImagesQuery, firstImageCreateRequest, commandType: CommandType.Text);

                if (rowsAffectedByFirstImageInsert < 1) return new UnexpectedFailureResult();
            }

            return newProductId;
        }
    }

    public OneOf<Success, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest)
    {
        const string updateProductQuery =
            $"""
            UPDATE {_tableName}
            SET TID = @CategoryId,
                CFGSUBTYPE = @Name,
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
            SET S = ISNULL(@CustomDisplayOrder, S),
                KeywordValue = @Value,
                Discr = @XmlPlacement
            
            WHERE CSTID = @ProductId
            AND ProductKeywordID = @ProductCharacteristicId;
            """;

        const string updateImageFilePathInfosQuery =
            $"""
            DECLARE @DisplayOrder INT;
            DECLARE @MaxDisplayOrder INT;
            
            SELECT TOP 1 @DisplayOrder = S FROM {_imageFileNamesTableName}
            WHERE CSTID = @ProductId
            AND ImageNumber = @ImageNumber;
            
            SELECT @MaxDisplayOrder = ISNULL((SELECT COUNT(*) FROM {_imageFileNamesTableName} WHERE CSTID = @ProductId), 1);
            
            SET @NewDisplayOrder = 
            CASE 
                WHEN @NewDisplayOrder IS NULL THEN NULL
                WHEN @NewDisplayOrder < 1 THEN 1
                WHEN @NewDisplayOrder > @MaxDisplayOrder THEN @MaxDisplayOrder
                ELSE @NewDisplayOrder
            END;
            
            IF EXISTS (SELECT 1 FROM {_tableName} WHERE CSTID = @ProductId)
            BEGIN       
                UPDATE {_imageFileNamesTableName}
                SET S = 
                    CASE
                        WHEN S = @DisplayOrder AND ImageNumber = @ImageNumber THEN @NewDisplayOrder
                        WHEN @DisplayOrder < @NewDisplayOrder AND S > @DisplayOrder AND S <= @NewDisplayOrder THEN S - 1
                        WHEN @DisplayOrder > @NewDisplayOrder AND S < @DisplayOrder AND S >= @NewDisplayOrder THEN S + 1
                        ELSE S
                    END
                WHERE CSTID = @ProductId;
            
                UPDATE {_imageFileNamesTableName}
                SET ImgFileName = @FileName,
                    Active = @Active
            
                WHERE CSTID = @ProductId
                AND ImageNumber = @ImageNumber;
            
                IF @@ROWCOUNT > 0
                BEGIN
                    SELECT 1;
                END
                ELSE
                BEGIN
                    SELECT 0;
                END
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        const string updateInAllImagesQuery =
            $"""
            UPDATE {_allImagesTableName}
            SET Description = @HtmlData,
                Image = @ImageData,
                ImageFileExt = @ImageContentType,
                DateModified = @DateModified

            WHERE ID = @Id;
            """;

        const string updateInFirstImagesQuery =
            $"""
            UPDATE {_firstImagesTableName}
            SET Description = @HtmlData,
                Image = @ImageData,
                ImageFileExt = @ImageContentType,
                DateModified = @DateModified

            WHERE ID = @ProductId;
            """;

        OneOf<Success, UnexpectedFailureResult> result = _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition(
            UpdateAllRecordsInTransaction,
            resultLocal => resultLocal.Match(
                success => true,
                unexpectedFailureResult => false),
            updateRequest);

        return result;

        OneOf<Success, UnexpectedFailureResult> UpdateAllRecordsInTransaction(IDbConnection connection, ProductUpdateRequest updateRequest)
        {
            var parameters = new
            {
                id = updateRequest.Id,
                CategoryId = updateRequest.CategoryId,
                Name = updateRequest.Name,
                ADDWRR = updateRequest.AdditionalWarrantyPrice,
                ADDWRRTERM = updateRequest.AdditionalWarrantyTermMonths,
                ADDWRRDEF = updateRequest.StandardWarrantyPrice,
                DEFWRRTERM = updateRequest.StandardWarrantyTermMonths,
                DisplayOrder = updateRequest.DisplayOrder,
                Status = updateRequest.Status,
                PLSHOW = updateRequest.PlShow,
                PRICE1 = updateRequest.Price1,
                PRICE2 = updateRequest.DisplayPrice,
                PRICE3 = updateRequest.Price3,
                CurrencyId = updateRequest.Currency,
                RowGuid = updateRequest.RowGuid,
                PromPID = updateRequest.PromotionId,
                PromRID = updateRequest.PromRid,
                PromPictureId = updateRequest.PromotionPictureId,
                PromExpDate = updateRequest.PromotionExpireDate,
                AlertPictureId = updateRequest.AlertPictureId,
                AlertExpDate = updateRequest.AlertExpireDate,
                PriceListDescription = updateRequest.PriceListDescription,
                ManifacturerId = updateRequest.ManifacturerId,
                SubcategoryId = updateRequest.SubCategoryId,
                SPLMODEL = updateRequest.PartNumber1,
                SPLMODEL1 = updateRequest.PartNumber2,
                SPLMODEL2 = updateRequest.SearchString
            };

            connection.Execute(updateProductQuery, parameters, commandType: CommandType.Text);

            if (updateRequest.Properties is not null
                && updateRequest.Properties.Count > 0)
            {
                IEnumerable<ProductPropertyUpdateRequest> propertyUpdateRequests = updateRequest.Properties.Select(x => Map(x, updateRequest.Id));

                int rowsAffectedByPropertyUpdate = connection.Execute(updatePropertiesQuery, propertyUpdateRequests, commandType: CommandType.Text);

                if (rowsAffectedByPropertyUpdate < updateRequest.Properties.Count) return new UnexpectedFailureResult();
            }

            if (updateRequest.ImageFileNames is not null
                && updateRequest.ImageFileNames.Count > 0)
            {
                IEnumerable<ProductImageFileNameInfoByImageNumberUpdateRequest> imageFileNameUpdateRequests =
                    updateRequest.ImageFileNames.Select(x => Map(x, updateRequest.Id));

                int rowsAffectedByImageFileNameUpdate = connection.Execute(
                    updateImageFilePathInfosQuery, imageFileNameUpdateRequests, commandType: CommandType.Text);

                if (rowsAffectedByImageFileNameUpdate < updateRequest.ImageFileNames.Count) return new UnexpectedFailureResult();
            }

            if (updateRequest.Images is not null
                && updateRequest.Images.Count > 0)
            {
                IEnumerable<ProductImageUpdateRequest> imageUpdateRequests = updateRequest.Images.Select(x => Map(x, updateRequest.Id));

                int rowsAffectedByImagesUpdate = connection.Execute(updateInAllImagesQuery, imageUpdateRequests, commandType: CommandType.Text);

                if (rowsAffectedByImagesUpdate < updateRequest.Images.Count) return new UnexpectedFailureResult();

                ProductFirstImageUpdateRequest firstImageUpdateRequest = MapToFirstImage(updateRequest.Images.First(), updateRequest.Id);

                int rowsAffectedByFirstImageUpdate = connection.Execute(updateInFirstImagesQuery, firstImageUpdateRequest, commandType: CommandType.Text);

                if (rowsAffectedByFirstImageUpdate < 1) return new UnexpectedFailureResult();
            }

            return new Success();
        }
    }

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        const string deleteProductAndRelatedItemsQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @id;

            DELETE FROM {_propertiesTableName}
            WHERE CSTID = @id;

            DELETE FROM {_imageFileNamesTableName}
            WHERE CSTID = @id;

            DELETE FROM {_allImagesTableName}
            WHERE CSTID = @id;

            DELETE FROM {_firstImagesTableName}
            WHERE ID = @id;
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
            id = id
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

#pragma warning restore IDE0037 // Use inferred member name

    private static ProductPropertyByCharacteristicIdCreateRequest Map(CurrentProductPropertyCreateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ProductCharacteristicId = request.ProductCharacteristicId,
            CustomDisplayOrder = request.CustomDisplayOrder,
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
            Active = request.Active,
        };
    }

    private List<LocalProductImageCreateRequest> MapToLocalAllImages(
        List<CurrentProductImageCreateRequest> requests, int productId, IDbConnection connection)
    {
        List<LocalProductImageCreateRequest> output = new();

        int highestId = GetHighestImageId(connection) ?? 1;

        for (int i = 0; i < requests.Count; i++)
        {
            CurrentProductImageCreateRequest? item = requests[i];

            LocalProductImageCreateRequest localRequest = new()
            {
                Id = highestId + (i + 1),
                ProductId = productId,
                ImageData = item.ImageData,
                ImageContentType = item.ImageContentType,
                HtmlData = item.HtmlData,
                DateModified = item.DateModified,
            };

            output.Add(localRequest);
        }

        return output;
    }

    private int? GetHighestImageId(IDbConnection connection, IDbTransaction? transaction = null)
    {
        const string getHighestIdFromAllImages =
            $"""
            SELECT MAX(ID) FROM {_allImagesTableName}
            """;

        int? highestId = _relationalDataAccess.GetDataFirstOrDefault<int?, dynamic>(getHighestIdFromAllImages, new { }, connection, transaction);

        return highestId;
    }

    private static ProductImageCreateRequest Map(CurrentProductImageCreateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ImageData = request.ImageData,
            ImageContentType = request.ImageContentType,
            HtmlData = request.HtmlData,
            DateModified = request.DateModified,
        };
    }

    private static ProductPropertyUpdateRequest Map(CurrentProductPropertyUpdateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ProductCharacteristicId = request.ProductCharacteristicId,
            CustomDisplayOrder = request.CustomDisplayOrder,
            Value = request.Value,
            XmlPlacement = request.XmlPlacement,
        };
    }

    private static ProductImageFileNameInfoByImageNumberUpdateRequest Map(CurrentProductImageFileNameInfoUpdateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            NewDisplayOrder = request.NewDisplayOrder,
            ImageNumber = request.ImageNumber,
            FileName = request.FileName,
            Active = request.Active,
        };
    }

    private static ProductImageUpdateRequest Map(CurrentProductImageUpdateRequest request, int productId)
    {
        return new()
        {
            Id = productId,
            ImageData = request.ImageData,
            ImageContentType = request.ImageContentType,
            HtmlData = request.HtmlData,
        };
    }

    private static ProductFirstImageUpdateRequest MapToFirstImage(CurrentProductImageUpdateRequest request, int productId)
    {
        return new()
        {
            ProductId = productId,
            ImageData = request.ImageData,
            ImageContentType = request.ImageContentType,
            HtmlData = request.HtmlData,
        };
    }

    private class LocalProductImageCreateRequest
    {
        public int? Id { get; set; }
        public int? ProductId { get; set; }
        public string? HtmlData { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public DateTime? DateModified { get; set; }
    }
}