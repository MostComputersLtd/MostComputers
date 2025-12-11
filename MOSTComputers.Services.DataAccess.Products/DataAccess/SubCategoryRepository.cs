using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.SubCategoriesTable;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class SubCategoryRepository : ISubCategoryRepository
{
    public SubCategoryRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<SubCategory>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT {IdColumnName},
                {CategoryIdColumnName} AS {CategoryIdAlias},
                {NameColumnName} AS {NameAlias},
                {DisplayOrderColumnName} AS {DisplayOrderAlias},
                {ActiveColumnName} AS {ActiveAlias}
            FROM {SubCategoriesTableName} WITH (NOLOCK)
            ORDER BY {DisplayOrderColumnName}
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<SubCategory> subCategories = await dbConnection.QueryAsync<SubCategory>(
            query, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return subCategories.AsList();
    }

    public async Task<List<SubCategory>> GetAllInCategoryAsync(int categoryId)
    {
        const string query =
            $"""
            SELECT {IdColumnName},
                {CategoryIdColumnName} AS {CategoryIdAlias},
                {NameColumnName} AS {NameAlias},
                {DisplayOrderColumnName} AS {DisplayOrderAlias},
                {ActiveColumnName} AS {ActiveAlias}
            FROM {SubCategoriesTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            ORDER BY {DisplayOrderColumnName}
            """;

        var parameters = new
        {
            categoryId = categoryId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<SubCategory> subCategories = await dbConnection.QueryAsync<SubCategory>(
            query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return subCategories.AsList();
    }

    public async Task<List<SubCategory>> GetAllByActivityAsync(bool active)
    {
        const string query =
            $"""
            SELECT {IdColumnName},
                {CategoryIdColumnName} AS {CategoryIdAlias},
                {NameColumnName} AS {NameAlias},
                {DisplayOrderColumnName} AS {DisplayOrderAlias},
                {ActiveColumnName} AS {ActiveAlias}
            FROM {SubCategoriesTableName} WITH (NOLOCK)
            WHERE {ActiveColumnName} = @active
            ORDER BY {DisplayOrderColumnName}
            """;

        int activeAsInt = active ? 1 : 0;

        var parameters = new
        {
            active = activeAsInt,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<SubCategory> subCategories = await dbConnection.QueryAsync<SubCategory>(
            query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return subCategories.AsList();
    }

    public async Task<List<SubCategory>> GetInCategoryByActivityAsync(int categoryId, bool active)
    {
        const string query =
            $"""
            SELECT {IdColumnName},
                {CategoryIdColumnName} AS {CategoryIdAlias},
                {NameColumnName} AS {NameAlias},
                {DisplayOrderColumnName} AS {DisplayOrderAlias},
                {ActiveColumnName} AS {ActiveAlias}
            FROM {SubCategoriesTableName} WITH (NOLOCK)
            WHERE {CategoryIdColumnName} = @categoryId
            AND {ActiveColumnName} = @active
            ORDER BY {DisplayOrderColumnName}
            """;

        int activeAsInt = active ? 1 : 0;

        var parameters = new
        {
            categoryId = categoryId,
            active = activeAsInt,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<SubCategory> subCategories = await dbConnection.QueryAsync<SubCategory>(
            query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return subCategories.AsList();
    }

    public async Task<List<SubCategory>> GetByIdsAsync(List<int> ids)
    {
        const string query =
            $"""
            SELECT TOP 1 {IdColumnName},
                {CategoryIdColumnName} AS {CategoryIdAlias},
                {NameColumnName} AS {NameAlias},
                {DisplayOrderColumnName} AS {DisplayOrderAlias},
                {ActiveColumnName} AS {ActiveAlias}
            FROM {SubCategoriesTableName} WITH (NOLOCK)
            WHERE {IdColumnName} IN @ids
            """;

        var parameters = new
        {
            ids = ids,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<SubCategory> subCategories = await ExecuteListQueryWithParametersInChunksAsync(
           idsChunk =>
           {
               var parameters = new
               {
                   ids = idsChunk
               };

               return dbConnection.QueryAsync<SubCategory>(query, parameters, commandType: CommandType.Text);
           },
           ids);

        transactionScope.Complete();

        return subCategories;
    }

    public async Task<SubCategory?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT TOP 1 {IdColumnName},
                {CategoryIdColumnName} AS {CategoryIdAlias},
                {NameColumnName} AS {NameAlias},
                {DisplayOrderColumnName} AS {DisplayOrderAlias},
                {ActiveColumnName} AS {ActiveAlias}
            FROM {SubCategoriesTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id
            """;

        var parameters = new
        {
            id = id,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        SubCategory? subCategory = await dbConnection.QueryFirstOrDefaultAsync<SubCategory>(
            query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return subCategory;
    }

    public async Task<SubCategory?> GetByNameAsync(string name)
    {
        const string query =
            $"""
            SELECT TOP 1 {IdColumnName},
                {CategoryIdColumnName} AS {CategoryIdAlias},
                {NameColumnName} AS {NameAlias},
                {DisplayOrderColumnName} AS {DisplayOrderAlias},
                {ActiveColumnName} AS {ActiveAlias}
            FROM {SubCategoriesTableName} WITH (NOLOCK)
            WHERE {NameColumnName} = @name
            """;

        var parameters = new
        {
            name = name,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        SubCategory? subCategory = await dbConnection.QueryFirstOrDefaultAsync<SubCategory>(
            query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return subCategory;
    }
}