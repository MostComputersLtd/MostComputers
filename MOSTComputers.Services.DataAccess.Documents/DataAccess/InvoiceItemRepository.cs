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
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.InvoiceItemsTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;
internal class InvoiceItemRepository : IInvoiceItemRepository
{
    public InvoiceItemRepository([FromKeyedServices(ConfigureServices.DataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<InvoiceItem>> GetInvoiceItemsInExportAsync(int exportId)
    {
        const string query =
            $"""
            SELECT {ExportedItemIdColumn},
                {ExportIdColumn} AS {ExportIdAlias},
                {IEIDColumn},
                {InvoiceIdColumn} AS {InvoiceIdAlias},
                {NameColumn},
                {PriceInLevaColumn},
                {QuantityColumn},
                {DisplayOrderColumn}
            FROM {InvoiceItemsTableName} WITH (NOLOCK)
            WHERE {ExportIdColumn} = @exportId;
            """;

        var parameters = new
        {
            exportId = exportId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<InvoiceItemDAO> output = await dbConnection.QueryAsync<InvoiceItemDAO>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsInInvoiceAsync(int invoiceId)
    {
        const string query =
            $"""
            SELECT {ExportedItemIdColumn},
                {ExportIdColumn} AS {ExportIdAlias},
                {IEIDColumn},
                {InvoiceIdColumn} AS {InvoiceIdAlias},
                {NameColumn},
                {PriceInLevaColumn},
                {QuantityColumn},
                {DisplayOrderColumn}
            FROM {InvoiceItemsTableName} WITH (NOLOCK)
            WHERE {InvoiceIdColumn} = @invoiceId;
            """;

        var parameters = new
        {
            invoiceId = invoiceId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<InvoiceItemDAO> output = await dbConnection.QueryAsync<InvoiceItemDAO>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return MapRange(output);
    }

    public async Task<InvoiceItem?> GetInvoiceItemByIdAsync(int id)
    {
        const string query =
            $"""
            SELECT {ExportedItemIdColumn},
                {ExportIdColumn} AS {ExportIdAlias},
                {IEIDColumn},
                {InvoiceIdColumn} AS {InvoiceIdAlias},
                {NameColumn},
                {PriceInLevaColumn},
                {QuantityColumn},
                {DisplayOrderColumn}
            FROM {InvoiceItemsTableName} WITH (NOLOCK)
            WHERE {IEIDColumn} = @id;
            """;

        var parameters = new
        {
            id = id,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        InvoiceItemDAO? output = await dbConnection.QueryFirstOrDefaultAsync<InvoiceItemDAO>(query, parameters, commandType: CommandType.Text);

        transactionScope.Complete();

        return (output is not null) ? Map(output) : null;
    }
}