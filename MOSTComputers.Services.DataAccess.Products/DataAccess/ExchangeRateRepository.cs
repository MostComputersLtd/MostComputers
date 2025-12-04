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
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ExchangeRatesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal class ExchangeRateRepository : IExchangeRateRepository
{
    public ExchangeRateRepository(
        [FromKeyedServices(ConfigureServices.ReadOnlyDBConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ExchangeRate>> GetAllAsync()
    {
        const string getAllQuery =
            $"""
            SELECT [{CurrencyFromIdColumnName}], [{CurrencyToIdColumnName}], [{RateColumnName}]
            FROM {ExchangeRatesTableName} WITH (NOLOCK)
            """;

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ExchangeRate> exchangeRates
            = await dbConnection.QueryAsync<ExchangeRate>(getAllQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return exchangeRates.AsList();
    }

    public async Task<ExchangeRate?> GetForCurrenciesAsync(Currency currencyFrom, Currency currencyTo)
    {
        const string getForCurrenciesQuery =
            $"""
            SELECT [{CurrencyFromIdColumnName}], [{CurrencyToIdColumnName}], [{RateColumnName}]
            FROM {ExchangeRatesTableName} WITH (NOLOCK)
            WHERE {CurrencyFromIdColumnName} = @CurrencyFrom
            AND {CurrencyToIdColumnName} = @CurrencyTo
            """;

        var parameters = new
        {
            CurrencyFrom = (int)currencyFrom,
            CurrencyTo = (int)currencyTo,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        ExchangeRate? exchangeRate = await dbConnection.QueryFirstOrDefaultAsync<ExchangeRate>(
            getForCurrenciesQuery, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return exchangeRate;
    }
}