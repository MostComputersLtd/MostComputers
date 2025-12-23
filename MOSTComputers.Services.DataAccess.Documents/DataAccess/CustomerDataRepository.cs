using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using System.Data;
using System.Transactions;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.CustomerDataView;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;
internal class CustomerDataRepository : ICustomerDataRepository
{
    public CustomerDataRepository([FromKeyedServices(ConfigureServices.DataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<CustomerData>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT {IdColumn},
                {NameColumn},
                {ContactPersonNameColumn},
                {CountryColumn},
                {AddressColumn},
                {EmployeeIdColumn}
            FROM {CustomerDataViewName} WITH (NOLOCK)
            """;

        var parameters = new
        {
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<CustomerData> data = await dbConnection.QueryAsync<CustomerData>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<List<CustomerData>> GetByIdsAsync(List<int> ids)
    {
        const string query =
            $"""
            SELECT {IdColumn},
                {NameColumn},
                {ContactPersonNameColumn},
                {CountryColumn},
                {AddressColumn},
                {EmployeeIdColumn}
            FROM {CustomerDataViewName} WITH (NOLOCK)
            WHERE {IdColumn} IN @ids
            """;

        var parameters = new
        {
            ids = ids,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<CustomerData> data = await dbConnection.QueryAsync<CustomerData>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data.AsList();
    }

    public async Task<CustomerData?> GetByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT {IdColumn},
                {NameColumn},
                {ContactPersonNameColumn},
                {CountryColumn},
                {AddressColumn},
                {EmployeeIdColumn}
            FROM {CustomerDataViewName} WITH (NOLOCK)
            WHERE {IdColumn} = id
            """;

        var parameters = new
        {
            id = id,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        CustomerData? data = await dbConnection.QuerySingleOrDefaultAsync<CustomerData>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return data;
    }
}