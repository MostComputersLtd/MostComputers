using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductCharacteristicsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class ProductCharacteristicsRepository : IProductCharacteristicsRepository
{
    public ProductCharacteristicsRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAsync(int categoryId)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryId = categoryId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAsync(int categoryId, bool active)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {ActiveColumnName} = @active
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryId = categoryId,
            active = active,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAsync(IEnumerable<int> categoryIds)
    {
        const string getAllCharacteristicsByCategoryIdsQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} IN @categoryIds
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryIds = categoryIds,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdsQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAsync(IEnumerable<int> categoryIds, bool active)
    {
        const string getAllCharacteristicsByCategoryIdsQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} IN @categoryIds
            AND {ActiveColumnName} = @active
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryIds = categoryIds,
            active = active,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdsQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypeAsync(int categoryId, ProductCharacteristicType productCharacteristicType)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {KWPrChColumnName} = @productCharacteristicType
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryId = categoryId,
            productCharacteristicType = (int)productCharacteristicType,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypeAsync(int categoryId, ProductCharacteristicType productCharacteristicType, bool active)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {KWPrChColumnName} = @productCharacteristicType
            AND {ActiveColumnName} = @active
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryId = categoryId,
            productCharacteristicType = (int)productCharacteristicType,
            active = active,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypesAsync(int categoryId, IEnumerable<ProductCharacteristicType> productCharacteristicTypes)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {KWPrChColumnName} IN @productCharacteristicTypes
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryId = categoryId,
            productCharacteristicTypes = productCharacteristicTypes.Cast<int>(),
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdAndTypesAsync(
        int categoryId, IEnumerable<ProductCharacteristicType> productCharacteristicTypes, bool active)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {KWPrChColumnName} IN @productCharacteristicTypes
            AND {ActiveColumnName} = @active
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryId = categoryId,
            productCharacteristicTypes = productCharacteristicTypes.Cast<int>(),
            active = active,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAndTypesAsync(
        IEnumerable<int> categoryIds,
        IEnumerable<ProductCharacteristicType> productCharacteristicTypes)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} IN @categoryIds
            AND {KWPrChColumnName} IN @productCharacteristicTypes
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryIds = categoryIds,
            productCharacteristicTypes = productCharacteristicTypes.Cast<int>(),
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetAllByCategoryIdsAndTypesAsync(
        IEnumerable<int> categoryIds, IEnumerable<ProductCharacteristicType> productCharacteristicTypes, bool active)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} IN @categoryIds
            AND {KWPrChColumnName} IN @productCharacteristicTypes
            AND {ActiveColumnName} = @active
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            categoryIds = categoryIds,
            productCharacteristicTypes = productCharacteristicTypes.Cast<int>(),
            active = active,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getAllCharacteristicsByCategoryIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    
    public async Task<List<ProductCharacteristic>> GetSelectionByCategoryIdAndNamesAsync(int categoryId, IEnumerable<string> names)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {NameColumnName} IN @Names;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Names = names,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getByCategoryIdAndNameQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<List<ProductCharacteristic>> GetSelectionByCharacteristicIdsAsync(IEnumerable<int> ids)
    {
        const string getSelectionByIdsQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {IdColumnName} IN @ids;
            """;

        var parameters = new
        {
            ids = ids,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductCharacteristic> productCharacteristics = await dbConnection.QueryAsync<ProductCharacteristic>(
            getSelectionByIdsQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristics.AsList();
    }

    public async Task<ProductCharacteristic?> GetByIdAsync(int id)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new
        {
            id = id
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductCharacteristic? productCharacteristic = await dbConnection.QueryFirstOrDefaultAsync<ProductCharacteristic>(
            getByCategoryIdAndNameQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristic;
    }

    public async Task<ProductCharacteristic?> GetByCategoryIdAndNameAsync(int categoryId, string name)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {NameColumnName} = @Name;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = name
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductCharacteristic? productCharacteristic = await dbConnection.QueryFirstOrDefaultAsync<ProductCharacteristic>(
            getByCategoryIdAndNameQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristic;
    }

    public async Task<ProductCharacteristic?> GetByCategoryIdAndNameAndCharacteristicTypeAsync(
        int categoryId,
        string name,
        ProductCharacteristicType productCharacteristicType)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {ProductCharacteristicsTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {NameColumnName} = @Name
            AND {KWPrChColumnName} = @productCharacteristicType;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = name,
            productCharacteristicType = (int)productCharacteristicType
        };
        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ProductCharacteristic? productCharacteristic = await dbConnection.QueryFirstOrDefaultAsync<ProductCharacteristic>(
            getByCategoryIdAndNameQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return productCharacteristic;
    }
}