using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.FirmsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;
internal sealed class FirmDataRepository : IFirmDataRepository
{
    public FirmDataRepository([FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<FirmData>> GetAllAsync()
    {
        const string query =
            $"""
            SELECT {IdColumn},
                {NameColumn},
                {OrderColumn},
                {InfoColumn},
                {InvoiceNumberColumn},
                {AddressColumn},
                {MPersonColumn},
                {TaxNumberColumn},
                {BulstatColumn}
                FROM {FirmsTableName}
            """;

        SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new { };

        IEnumerable<FirmData> data = await dbConnection.QueryAsync<FirmData>(query, parameters);

        return data.AsList();
    }

    public async Task<FirmData?> GetByIdAsync(int firmId)
    {
        const string query =
            $"""
            SELECT {IdColumn},
                {NameColumn},
                {OrderColumn},
                {InfoColumn},
                {InvoiceNumberColumn},
                {AddressColumn},
                {MPersonColumn},
                {TaxNumberColumn},
                {BulstatColumn}
                FROM {FirmsTableName}
                WHERE {IdColumn} = @firmId;
            """;

        SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        var parameters = new
        {
            firmId = firmId
        };

        FirmData? data = await dbConnection.QueryFirstOrDefaultAsync<FirmData>(query, parameters);

        return data;
    }
}