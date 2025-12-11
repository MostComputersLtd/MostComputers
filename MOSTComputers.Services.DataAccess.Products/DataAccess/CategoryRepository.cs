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
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.CategoriesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class CategoryRepository : ICategoryRepository
{
    public CategoryRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<Category>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {CategoriesTableName} WITH (NOLOCK)
            ORDER BY {DisplayOrderColumnName};
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<Category> data = await dbConnection.QueryAsync<Category>(getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT * FROM {CategoriesTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        Category? category = await dbConnection.QueryFirstOrDefaultAsync<Category>(getByIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return category;
    }
}