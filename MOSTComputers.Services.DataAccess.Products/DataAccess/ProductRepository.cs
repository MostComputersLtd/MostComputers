using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.DAOs.Product;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class ProductRepository : IProductRepository
{
    public ProductRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<int>> GetAllIdsAsync()
    {
        const string getAllIdsQuery =
            $"""
            SELECT {IdColumnName}
            FROM {ProductsTableName} products WITH (NOLOCK)
            """;

        var parameters = new { };

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<int> productIds = await connection.QueryAsync<int>(getAllIdsQuery, parameters, commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return productIds.AsList();
    }

    public async Task<List<int>> GetOnlyExistingIdsAsync(List<int> idsToValidate)
    {
        const string getOnlyExistingIdsQuery =
            $"""
            SELECT {IdColumnName}
            FROM {ProductsTableName} products WITH (NOLOCK)
            WHERE {IdColumnName} IN @ids
            """;
       
        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        List<int> validProductIds = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    ids = productIdsChunk
                };

                return connection.QueryAsync<int>(getOnlyExistingIdsQuery, parameters, commandType: CommandType.Text);
            },
            idsToValidate);

        suppressedTransactionScope.Complete();

        return validProductIds;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        const string getAllWithManufacturerAndCategoryQuery =
            $"""
            SELECT {IdColumnName}, {CategoryIdColumnName}, {NameColumnName}, {AdditionalWarrantyPriceColumnName}, {AdditionalWarrantyTermMonthsColumnName},
                {StandardWarrantyPriceColumnName}, {StandardWarrantyTermMonthsColumnName}, products.{DisplayOrderColumnName}, {StatusColumnName},
                {PlShowColumnName}, {Price1ColumnName}, {Price2ColumnName}, {Price3ColumnName}, {CurrencyIdColumnName}, products.{RowGuidColumnName},
                {PromotionPidColumnName}, {PromotionRidColumnName}, {PromotionPictureIdColumnName}, {PromotionExpireDateColumnName},
                {AlertPictureIdColumnName}, {AlertExpireDateColumnName}, {PriceListDescriptionColumnName},
                products.{ManufacturerIdColumnName}, {SubcategoryIdColumnName}, {PartNumber1ColumnName}, {PartNumber2ColumnName}, {SearchStringColumnName},
                {CategoriesTable.IdColumnName}, {CategoriesTable.ParentCategoryIdColumnName}, {CategoriesTable.DescriptionColumnName}, {CategoriesTable.IsLeafColumnName},
                man.{ManufacturersTable.IdColumnName} AS {ManufacturersTable.IdColumnAlias},
                {ManufacturersTable.BGNameColumnName},
                {ManufacturersTable.RealCompanyNameColumnName},
                man.{ManufacturersTable.DisplayOrderColumnName} AS {ManufacturersTable.DisplayOrderColumnAlias},
                {ManufacturersTable.ActiveColumnName}

            FROM {ProductsTableName} products WITH (NOLOCK)
            LEFT JOIN {CategoriesTableName} cat WITH (NOLOCK)
            ON cat.{CategoriesTable.IdColumnName} = products.{CategoryIdColumnName}
            LEFT JOIN {ManufacturersTableName} man WITH (NOLOCK)
            ON man.{ManufacturersTable.IdColumnName} = products.{ManufacturerIdColumnName}
            ORDER BY products.{DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductDAO> products = await connection.QueryAsync<ProductDAO, Category, Manufacturer, ProductDAO>(
            getAllWithManufacturerAndCategoryQuery,
            (product, category, manufacturer) =>
            {
                product.Category = category;
                product.Manufacturer = manufacturer;

                return product;
            },
            new { },
            splitOn: $"{CategoriesTable.IdColumnName},{ManufacturersTable.IdColumnAlias}",
            commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return MapAllFromDAO(products);
    }

    public async Task<List<Product>> GetAllWithStatusesAsync(List<ProductStatus> productStatuses)
    {
        const string getAllWithManufacturerAndCategoryQuery =
            $"""
            SELECT {IdColumnName}, {CategoryIdColumnName}, {NameColumnName}, {AdditionalWarrantyPriceColumnName}, {AdditionalWarrantyTermMonthsColumnName},
                {StandardWarrantyPriceColumnName}, {StandardWarrantyTermMonthsColumnName}, products.{DisplayOrderColumnName}, {StatusColumnName},
                {PlShowColumnName}, {Price1ColumnName}, {Price2ColumnName}, {Price3ColumnName}, {CurrencyIdColumnName}, products.{RowGuidColumnName},
                {PromotionPidColumnName}, {PromotionRidColumnName}, {PromotionPictureIdColumnName}, {PromotionExpireDateColumnName},
                {AlertPictureIdColumnName}, {AlertExpireDateColumnName}, {PriceListDescriptionColumnName},
                products.{ManufacturerIdColumnName}, {SubcategoryIdColumnName}, {PartNumber1ColumnName}, {PartNumber2ColumnName}, {SearchStringColumnName},
                {CategoriesTable.IdColumnName}, {CategoriesTable.ParentCategoryIdColumnName}, {CategoriesTable.DescriptionColumnName}, {CategoriesTable.IsLeafColumnName},
                man.{ManufacturersTable.IdColumnName} AS {ManufacturersTable.IdColumnAlias},
                {ManufacturersTable.BGNameColumnName},
                {ManufacturersTable.RealCompanyNameColumnName},
                man.{ManufacturersTable.DisplayOrderColumnName} AS {ManufacturersTable.DisplayOrderColumnAlias},
                {ManufacturersTable.ActiveColumnName}

            FROM {ProductsTableName} products WITH (NOLOCK)
            LEFT JOIN {CategoriesTableName} cat WITH (NOLOCK)
            ON cat.{CategoriesTable.IdColumnName} = products.{CategoryIdColumnName}
            LEFT JOIN {ManufacturersTableName} man WITH (NOLOCK)
            ON man.{ManufacturersTable.IdColumnName} = products.{ManufacturerIdColumnName}
            WHERE products.{StatusColumnName} IN @productStatuses
            ORDER BY products.{DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        var param = new
        {
            productStatuses = productStatuses.Select(x => (int)x)
        };

        IEnumerable<ProductDAO> products = await connection.QueryAsync<ProductDAO, Category, Manufacturer, ProductDAO>(
            getAllWithManufacturerAndCategoryQuery,
            (product, category, manufacturer) =>
            {
                product.Category = category;
                product.Manufacturer = manufacturer;

                return product;
            },
            param,
            splitOn: $"{CategoriesTable.IdColumnName},{ManufacturersTable.IdColumnAlias}",
            commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return MapAllFromDAO(products);
    }

    public async Task<List<Product>> GetAllInCategoryAsync(int categoryId)
    {
        const string getAllWithManufacturerAndCategoryQuery =
            $"""
            SELECT {IdColumnName}, {CategoryIdColumnName}, {NameColumnName}, {AdditionalWarrantyPriceColumnName}, {AdditionalWarrantyTermMonthsColumnName},
                {StandardWarrantyPriceColumnName}, {StandardWarrantyTermMonthsColumnName}, products.{DisplayOrderColumnName}, {StatusColumnName},
                {PlShowColumnName}, {Price1ColumnName}, {Price2ColumnName}, {Price3ColumnName}, {CurrencyIdColumnName}, products.{RowGuidColumnName},
                {PromotionPidColumnName}, {PromotionRidColumnName}, {PromotionPictureIdColumnName}, {PromotionExpireDateColumnName},
                {AlertPictureIdColumnName}, {AlertExpireDateColumnName}, {PriceListDescriptionColumnName},
                products.{ManufacturerIdColumnName}, {SubcategoryIdColumnName}, {PartNumber1ColumnName}, {PartNumber2ColumnName}, {SearchStringColumnName},
                {CategoriesTable.IdColumnName}, {CategoriesTable.ParentCategoryIdColumnName}, {CategoriesTable.DescriptionColumnName}, {CategoriesTable.IsLeafColumnName},
                man.{ManufacturersTable.IdColumnName} AS {ManufacturersTable.IdColumnAlias},
                {ManufacturersTable.BGNameColumnName},
                {ManufacturersTable.RealCompanyNameColumnName},
                man.{ManufacturersTable.DisplayOrderColumnName} AS {ManufacturersTable.DisplayOrderColumnAlias},
                {ManufacturersTable.ActiveColumnName}
            
            FROM {ProductsTableName} products WITH (NOLOCK)
            LEFT JOIN {CategoriesTableName} cat WITH (NOLOCK)
            ON cat.{CategoriesTable.IdColumnName} = products.{CategoryIdColumnName}
            LEFT JOIN {ManufacturersTableName} man WITH (NOLOCK)
            ON man.{ManufacturersTable.IdColumnName} = products.{ManufacturerIdColumnName}
            WHERE {CategoryIdColumnName} = @categoryId
            ORDER BY products.{DisplayOrderColumnName};
            """;

        var parameters = new { categoryId = categoryId };

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductDAO> products = await connection.QueryAsync<ProductDAO, Category, Manufacturer, ProductDAO>(
            getAllWithManufacturerAndCategoryQuery,
            (product, category, manufacturer) =>
            {
                product.Category = category;
                product.Manufacturer = manufacturer;

                return product;
            },
            parameters,
            splitOn: $"{CategoriesTable.IdColumnName},{ManufacturersTable.IdColumnAlias}",
            commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return MapAllFromDAO(products);
    }

    public async Task<List<Product>> GetAllInCategoriesAsync(IEnumerable<int> categoryIds)
    {
        const string getAllWithManufacturerAndCategoryQuery =
            $"""
            SELECT {IdColumnName}, {CategoryIdColumnName}, {NameColumnName}, {AdditionalWarrantyPriceColumnName}, {AdditionalWarrantyTermMonthsColumnName},
                {StandardWarrantyPriceColumnName}, {StandardWarrantyTermMonthsColumnName}, products.{DisplayOrderColumnName}, {StatusColumnName},
                {PlShowColumnName}, {Price1ColumnName}, {Price2ColumnName}, {Price3ColumnName}, {CurrencyIdColumnName}, products.{RowGuidColumnName},
                {PromotionPidColumnName}, {PromotionRidColumnName}, {PromotionPictureIdColumnName}, {PromotionExpireDateColumnName},
                {AlertPictureIdColumnName}, {AlertExpireDateColumnName}, {PriceListDescriptionColumnName},
                products.{ManufacturerIdColumnName}, {SubcategoryIdColumnName}, {PartNumber1ColumnName}, {PartNumber2ColumnName}, {SearchStringColumnName},
                {CategoriesTable.IdColumnName}, {CategoriesTable.ParentCategoryIdColumnName}, {CategoriesTable.DescriptionColumnName}, {CategoriesTable.IsLeafColumnName},
                man.{ManufacturersTable.IdColumnName} AS {ManufacturersTable.IdColumnAlias},
                {ManufacturersTable.BGNameColumnName},
                {ManufacturersTable.RealCompanyNameColumnName},
                man.{ManufacturersTable.DisplayOrderColumnName} AS {ManufacturersTable.DisplayOrderColumnAlias},
                {ManufacturersTable.ActiveColumnName}
            
            FROM {ProductsTableName} products WITH (NOLOCK)
            LEFT JOIN {CategoriesTableName} cat WITH (NOLOCK)
            ON cat.{CategoriesTable.IdColumnName} = products.{CategoryIdColumnName}
            LEFT JOIN {ManufacturersTableName} man WITH (NOLOCK)
            ON man.{ManufacturersTable.IdColumnName} = products.{ManufacturerIdColumnName}
            WHERE {CategoryIdColumnName} IN @categoryIds
            ORDER BY products.{DisplayOrderColumnName};
            """;

        var parameters = new { categoryIds = categoryIds };

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductDAO> products = await connection.QueryAsync<ProductDAO, Category, Manufacturer, ProductDAO>(
            getAllWithManufacturerAndCategoryQuery,
            (product, category, manufacturer) =>
            {
                product.Category = category;
                product.Manufacturer = manufacturer;

                return product;
            },
            parameters,
            splitOn: $"{CategoriesTable.IdColumnName},{ManufacturersTable.IdColumnAlias}",
            commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        return MapAllFromDAO(products);
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        const string getAllWithManufacturerAndCategoryByIdsQuery =
            $"""
            SELECT {IdColumnName}, {CategoryIdColumnName}, {NameColumnName}, {AdditionalWarrantyPriceColumnName}, {AdditionalWarrantyTermMonthsColumnName},
                {StandardWarrantyPriceColumnName}, {StandardWarrantyTermMonthsColumnName}, products.{DisplayOrderColumnName}, {StatusColumnName},
                {PlShowColumnName}, {Price1ColumnName}, {Price2ColumnName}, {Price3ColumnName}, {CurrencyIdColumnName}, products.{RowGuidColumnName},
                {PromotionPidColumnName}, {PromotionRidColumnName}, {PromotionPictureIdColumnName}, {PromotionExpireDateColumnName},
                {AlertPictureIdColumnName}, {AlertExpireDateColumnName}, {PriceListDescriptionColumnName},
                products.{ManufacturerIdColumnName}, {SubcategoryIdColumnName}, {PartNumber1ColumnName}, {PartNumber2ColumnName}, {SearchStringColumnName},
                {CategoriesTable.IdColumnName}, {CategoriesTable.ParentCategoryIdColumnName}, {CategoriesTable.DescriptionColumnName}, {CategoriesTable.IsLeafColumnName},
                man.{ManufacturersTable.IdColumnName} AS {ManufacturersTable.IdColumnAlias},
                {ManufacturersTable.BGNameColumnName},
                {ManufacturersTable.RealCompanyNameColumnName},
                man.{ManufacturersTable.DisplayOrderColumnName} AS {ManufacturersTable.DisplayOrderColumnAlias},
                {ManufacturersTable.ActiveColumnName}
            
            FROM {ProductsTableName} products WITH (NOLOCK)
            LEFT JOIN {CategoriesTableName} cat WITH (NOLOCK)
            ON cat.{CategoriesTable.IdColumnName} = products.{CategoryIdColumnName}
            LEFT JOIN {ManufacturersTableName} man WITH (NOLOCK)
            ON man.{ManufacturersTable.IdColumnName} = products.{ManufacturerIdColumnName}
            WHERE {IdColumnName} IN @ids
            ORDER BY products.{DisplayOrderColumnName};
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        List<ProductDAO> products = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    ids = productIdsChunk
                };

                return connection.QueryAsync<ProductDAO, Category, Manufacturer, ProductDAO>(
                    getAllWithManufacturerAndCategoryByIdsQuery,
                    (product, category, manufacturer) =>
                    {
                        product.Category = category;
                        product.Manufacturer = manufacturer;

                        return product;
                    },
                    param: parameters,
                    splitOn: $"{CategoriesTable.IdColumnName},{ManufacturersTable.IdColumnAlias}",
                    commandType: CommandType.Text);
            },
            ids.AsList());

        suppressedTransactionScope.Complete();

        return MapAllFromDAO(products);
    }


    public async Task<Product?> GetByIdAsync(int id)
    {
        const string getByIdWithManufacturerAndCategoryAndFirstImageQuery =
            $"""
            SELECT TOP 1 {IdColumnName}, {CategoryIdColumnName}, {NameColumnName}, {AdditionalWarrantyPriceColumnName}, {AdditionalWarrantyTermMonthsColumnName},
                {StandardWarrantyPriceColumnName}, {StandardWarrantyTermMonthsColumnName}, products.{DisplayOrderColumnName}, {StatusColumnName},
                {PlShowColumnName}, {Price1ColumnName}, {Price2ColumnName}, {Price3ColumnName}, {CurrencyIdColumnName}, products.{RowGuidColumnName},
                {PromotionPidColumnName}, {PromotionRidColumnName}, {PromotionPictureIdColumnName}, {PromotionExpireDateColumnName},
                {AlertPictureIdColumnName}, {AlertExpireDateColumnName}, {PriceListDescriptionColumnName},
                products.{ManufacturerIdColumnName}, {SubcategoryIdColumnName}, {PartNumber1ColumnName}, {PartNumber2ColumnName}, {SearchStringColumnName},
                {CategoriesTable.IdColumnName}, {CategoriesTable.ParentCategoryIdColumnName}, {CategoriesTable.DescriptionColumnName}, {CategoriesTable.IsLeafColumnName},
                man.{ManufacturersTable.IdColumnName} AS {ManufacturersTable.IdColumnAlias},
                {ManufacturersTable.BGNameColumnName},
                {ManufacturersTable.RealCompanyNameColumnName},
                man.{ManufacturersTable.DisplayOrderColumnName} AS {ManufacturersTable.DisplayOrderColumnAlias},
                {ManufacturersTable.ActiveColumnName}
            
            FROM {ProductsTableName} products WITH (NOLOCK)
            LEFT JOIN {CategoriesTableName} cat WITH (NOLOCK)
            ON cat.{CategoriesTable.IdColumnName} = products.{CategoryIdColumnName}
            LEFT JOIN {ManufacturersTableName} man WITH (NOLOCK)
            ON man.{ManufacturersTable.IdColumnName} = products.{ManufacturerIdColumnName}
            WHERE products.{IdColumnName} = @id;
            """;

        var parameters = new { id = id };

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductDAO> data = await connection.QueryAsync<ProductDAO, Category, Manufacturer, ProductDAO>(
            getByIdWithManufacturerAndCategoryAndFirstImageQuery,

            (product, category, manufacturer) =>
            {
                product.Category = category;
                product.Manufacturer = manufacturer;

                return product;
            },
            parameters,
            splitOn: $"{CategoriesTable.IdColumnName},{ManufacturersTable.IdColumnAlias}",
            commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        ProductDAO? product = data.FirstOrDefault();

        if (product is null) return null;

        return MapFromDAO(product);
    }

    public async Task<Product?> GetProductWithHighestIdAsync()
    {
        const string getByIdWithManufacturerAndCategoryAndFirstImageQuery =
            $"""
            SELECT TOP 1 {IdColumnName}, {CategoryIdColumnName}, {NameColumnName}, {AdditionalWarrantyPriceColumnName}, {AdditionalWarrantyTermMonthsColumnName},
                {StandardWarrantyPriceColumnName}, {StandardWarrantyTermMonthsColumnName}, products.{DisplayOrderColumnName}, {StatusColumnName},
                {PlShowColumnName}, {Price1ColumnName}, {Price2ColumnName}, {Price3ColumnName}, {CurrencyIdColumnName}, products.{RowGuidColumnName},
                {PromotionPidColumnName}, {PromotionRidColumnName}, {PromotionPictureIdColumnName}, {PromotionExpireDateColumnName},
                {AlertPictureIdColumnName}, {AlertExpireDateColumnName}, {PriceListDescriptionColumnName},
                products.{ManufacturerIdColumnName}, {SubcategoryIdColumnName}, {PartNumber1ColumnName}, {PartNumber2ColumnName}, {SearchStringColumnName},
                {CategoriesTable.IdColumnName}, {CategoriesTable.ParentCategoryIdColumnName}, {CategoriesTable.DescriptionColumnName}, {CategoriesTable.IsLeafColumnName},
                man.{ManufacturersTable.IdColumnName} AS {ManufacturersTable.IdColumnAlias},
                {ManufacturersTable.BGNameColumnName},
                {ManufacturersTable.RealCompanyNameColumnName},
                man.{ManufacturersTable.DisplayOrderColumnName} AS {ManufacturersTable.DisplayOrderColumnAlias},
                {ManufacturersTable.ActiveColumnName}
            
            FROM {ProductsTableName} products WITH (NOLOCK)
            LEFT JOIN {CategoriesTableName} cat WITH (NOLOCK)
            ON cat.{CategoriesTable.IdColumnName} = products.{CategoryIdColumnName}
            LEFT JOIN {ManufacturersTableName} man WITH (NOLOCK)
            ON man.{ManufacturersTable.IdColumnName} = products.{ManufacturerIdColumnName}
            ORDER BY products.{IdColumnName} DESC;
            """;

        using TransactionScope suppressedTransactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection connection = new(_connectionStringProvider.ConnectionString);
        
        IEnumerable<ProductDAO> data = await connection.QueryAsync<ProductDAO, Category, Manufacturer, ProductDAO>(
            getByIdWithManufacturerAndCategoryAndFirstImageQuery,

            (product, category, manufacturer) =>
            {
                product.Category = category;
                product.Manufacturer = manufacturer;

                return product;
            },
            new { },
            splitOn: $"{CategoriesTable.IdColumnName},{ManufacturersTable.IdColumnAlias}",
            commandType: CommandType.Text);

        suppressedTransactionScope.Complete();

        ProductDAO? product = data.FirstOrDefault();

        if (product is null) return null;

        return MapFromDAO(product);
    }

    private static List<Product> MapAllFromDAO(IEnumerable<ProductDAO> productDAOs)
    {
        return productDAOs.SelectAsList(MapFromDAO);
    }

    private static Product MapFromDAO(ProductDAO productDAO)
    {
        return new()
        {
            Id = productDAO.Id,
            Name = productDAO.Name,
            AdditionalWarrantyPrice = productDAO.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productDAO.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productDAO.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productDAO.StandardWarrantyTermMonths,
            DisplayOrder = productDAO.DisplayOrder,
            Status = productDAO.Status,
            PlShow = productDAO.PlShow,
            Price = productDAO.Price,
            Currency = productDAO.Currency,
            RowGuid = productDAO.RowGuid,
            PromotionPid = productDAO.PromotionPid,
            PromotionRid = productDAO.PromotionRid,
            PromotionPictureId = productDAO.PromotionPictureId,
            PromotionExpireDate = productDAO.PromotionExpireDate,
            AlertPictureId = productDAO.AlertPictureId,
            AlertExpireDate = productDAO.AlertExpireDate,
            PriceListDescription = productDAO.PriceListDescription,
            PartNumber1 = productDAO.PartNumber1,
            PartNumber2 = productDAO.PartNumber2,
            SearchString = productDAO.SearchString,
            CategoryId = productDAO.CategoryId,
            Category = productDAO.Category,
            ManufacturerId = productDAO.ManufacturerId,
            Manufacturer = productDAO.Manufacturer,
            SubCategoryId = productDAO.SubCategoryId,
        };
    }
}