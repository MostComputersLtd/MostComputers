using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Counters;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Counters.Contracts;
using OneOf;
using OneOf.Types;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Counters;
internal class SystemCountersRepository : ISystemCountersRepository
{
    public SystemCountersRepository([FromKeyedServices(ConfigureServices.ConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<SystemCounters?> GetSystemCountersAsync()
    {
        const string getFirstQuery =
            $"""
            SELECT TOP 1 * FROM {SystemCountersTableName}
            """;

        using TransactionScope transactionScope
            = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        SystemCounters? systemCounters
            = await dbConnection.QueryFirstOrDefaultAsync<SystemCounters>(getFirstQuery, new { }, commandType: CommandType.Text);

        transactionScope.Complete();

        return systemCounters;
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpdateAsync(SystemCounters systemCounters)
    {
        const string updateQuery =
            $"""
            UPDATE {SystemCountersTableName}
            SET {SystemCountersTable.OriginalChangesLastSearchedPKColumnName} = @OriginalChangesLastSearchedPK
            """;

        var parameters = new
        {
            OriginalChangesLastSearchedPK = systemCounters.OriginalChangesLastSearchedPK,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int result = await dbConnection.ExecuteAsync(updateQuery, parameters, commandType: CommandType.Text);

        if (result == 0) return new UnexpectedFailureResult();

        return new Success();
    }
}