using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;
using System.Data;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Documents.Mapping.ResponseMapper;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.WarrantyCardItemsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;
internal sealed class WarrantyCardItemRepository : IWarrantyCardItemRepository
{
    public WarrantyCardItemRepository([FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<WarrantyCardItem>> GetWarrantyCardItemsInExportAsync(int exportId)
    {
        const string query =
            $"""
            SELECT {ExportedItemIdColumn},
                {ExportIdColumn} AS {ExportIdAlias},
                {OrderIdColumn} AS {OrderIdAlias},
                {ProductIdColumn},
                {ProductNameColumn},
                {PriceInLevaColumn},
                {QuantityColumn},
                {SerialNumberColumn},
                {WarrantyCardItemTermInMonthsColumn} AS {WarrantyCardItemTermInMonthsAlias},
                {DisplayOrderColumn}
            FROM {WarrantyCardItemsTableName} WITH (NOLOCK)
            WHERE {ExportIdColumn} = @exportId;
            """;

        var parameters = new
        {
            exportId = exportId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<WarrantyCardItemDAO> output = await dbConnection.QueryAsync<WarrantyCardItemDAO>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<List<WarrantyCardItem>> GetWarrantyCardItemsInWarrantyCardAsync(int warrantyCardId)
    {
        const string query =
            $"""
            SELECT {ExportedItemIdColumn},
                {ExportIdColumn} AS {ExportIdAlias},
                {OrderIdColumn} AS {OrderIdAlias},
                {ProductIdColumn},
                {ProductNameColumn},
                {PriceInLevaColumn},
                {QuantityColumn},
                {SerialNumberColumn},
                {WarrantyCardItemTermInMonthsColumn} AS {WarrantyCardItemTermInMonthsAlias},
                {DisplayOrderColumn}
            FROM {WarrantyCardItemsTableName} WITH (NOLOCK)
            WHERE {OrderIdColumn} = @warrantyCardId;
            """;

        var parameters = new
        {
            warrantyCardId = warrantyCardId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<WarrantyCardItemDAO> output = await dbConnection.QueryAsync<WarrantyCardItemDAO>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<List<WarrantyCardItem>> GetWarrantyCardItemsWithProductIdAsync(int productId)
    {
        const string query =
            $"""
            SELECT {ExportedItemIdColumn},
                {ExportIdColumn} AS {ExportIdAlias},
                {OrderIdColumn} AS {OrderIdAlias},
                {ProductIdColumn},
                {ProductNameColumn},
                {PriceInLevaColumn},
                {QuantityColumn},
                {SerialNumberColumn},
                {WarrantyCardItemTermInMonthsColumn} AS {WarrantyCardItemTermInMonthsAlias},
                {DisplayOrderColumn}
            FROM {WarrantyCardItemsTableName} WITH (NOLOCK)
            WHERE {ProductIdColumn} = @productId;
            """;

        var parameters = new
        {
            productId = productId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<WarrantyCardItemDAO> output = await dbConnection.QueryAsync<WarrantyCardItemDAO>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }
}