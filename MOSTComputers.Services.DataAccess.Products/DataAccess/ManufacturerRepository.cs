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
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ManufacturersTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class ManufacturerRepository : IManufacturerRepository
{
    public ManufacturerRepository(
        [FromKeyedServices(ConfigureServices.OriginalDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<Manufacturer>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT [{IdColumnName}] AS [{IdColumnAlias}],
                [{BGNameColumnName}],
                [{RealCompanyNameColumnName}],
                [{DisplayOrderColumnName}] AS [{DisplayOrderColumnAlias}],
                [{ActiveColumnName}]
            FROM {ManufacturersTableName} WITH (NOLOCK)
            ORDER BY {DisplayOrderColumnName}
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<Manufacturer> manufacturers = await dbConnection.QueryAsync<Manufacturer>(
            getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return manufacturers.AsList();
    }

    public async Task<List<Manufacturer>> GetAllWithActiveProductsAsync()
    {
        const string getAllQuery =
            $"""
            SELECT [{IdColumnName}] AS [{IdColumnAlias}],
                [{BGNameColumnName}],
                [{RealCompanyNameColumnName}],
                [{DisplayOrderColumnName}] AS [{DisplayOrderColumnAlias}],
                [{ActiveColumnName}]
            FROM {ManufacturersTableName} manufacturers WITH (NOLOCK)
            WHERE EXISTS (
                SELECT 1
                FROM {ProductsTableName} WITH (NOLOCK)
                WHERE {ProductsTableName}.{ProductsTable.ManufacturerIdColumnName} = manufacturers.{IdColumnName}
                AND {ProductsTableName}.{ProductsTable.StatusColumnName} != 1
                AND {ProductsTableName}.{ProductsTable.PlShowColumnName} != 2
            )
            ORDER BY {DisplayOrderColumnName}
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<Manufacturer> manufacturers = await dbConnection.QueryAsync<Manufacturer>(
            getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return manufacturers.AsList();
    }

    public async Task<Manufacturer?> GetByIdAsync(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 [{IdColumnName}] AS [{IdColumnAlias}],
                [{BGNameColumnName}],
                [{RealCompanyNameColumnName}],
                [{DisplayOrderColumnName}] AS [{DisplayOrderColumnAlias}],
                [{ActiveColumnName}]
            FROM {ManufacturersTableName} WITH (NOLOCK)
            WHERE {IdColumnName} = @id;
            """;

        var parameters = new { id = id };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        Manufacturer? manufacturer = await dbConnection.QueryFirstOrDefaultAsync<Manufacturer>(
            getByIdQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return manufacturer;
    }
}