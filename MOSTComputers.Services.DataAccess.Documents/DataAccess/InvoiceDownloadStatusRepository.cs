using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.InvoiceDownloadStatus;
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.InvoiceDownloadStatusesTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;
internal sealed class InvoiceDownloadStatusRepository : IInvoiceDownloadStatusRepository
{
    public InvoiceDownloadStatusRepository([FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<InvoiceDownloadStatus>> GetLatestByInvoiceIdsAsync(List<int> invoiceIds)
    {
        if (invoiceIds.Count == 0)
        {
            return new List<InvoiceDownloadStatus>();
        }

        const string query =
            $"""
            SELECT {IdColumn},
                {ExportIdColumn},
            	{InvoiceIdColumn},
            	{ImportedStatusColumn},
            	{DateColumn},
            	{UserNameColumn}
            FROM {InvoiceDownloadStatusesTableName} WITH (NOLOCK)
            WHERE {InvoiceIdColumn} IN @invoiceIds
            ORDER BY {DateColumn} DESC
            """;

        var parameters = new
        {
            invoiceIds = invoiceIds,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        await sqlConnection.OpenAsync();

        IEnumerable<InvoiceDownloadStatus> downloadStatuses = await sqlConnection.QueryAsync<InvoiceDownloadStatus>(query, parameters);

        var a = downloadStatuses.ToList();

        transactionScope.Complete();

        return downloadStatuses
            .GroupBy(x => x.InvoiceId)
            .Select(x => x.First())
            .ToList();
    }

    public async Task<InvoiceDownloadStatus?> GetLatestByInvoiceIdAsync(int invoiceId)
    {
        if (invoiceId <= 0)
        {
            return null;
        }

        const string query =
            $"""
            SELECT TOP 1 {IdColumn},
                {ExportIdColumn},
            	{InvoiceIdColumn},
            	{ImportedStatusColumn},
            	{DateColumn},
            	{UserNameColumn}
            FROM {InvoiceDownloadStatusesTableName} WITH (NOLOCK)
            WHERE {InvoiceIdColumn} = @invoiceId
            ORDER BY {DateColumn} DESC
            """;

        var parameters = new
        {
            invoiceId = invoiceId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        await sqlConnection.OpenAsync();

        InvoiceDownloadStatus? data = await sqlConnection.QueryFirstOrDefaultAsync<InvoiceDownloadStatus>(query, parameters);

        transactionScope.Complete();

        return data;
    }

    public async Task<OneOf<Success, InsertFailedResult>> InsertAsync(InvoiceDownloadStatusCreateRequest createRequest)
    {
        const string query =
            $"""
            INSERT INTO {InvoiceDownloadStatusesTableName} ({ExportIdColumn}, {InvoiceIdColumn}, {ImportedStatusColumn}, {DateColumn}, {UserNameColumn})
            VALUES (@exportId, @invoiceId, @ImportedStatus, @Date, @UserName)
            """;

        var parameters = new
        {
            exportId = createRequest.ExportId,
            invoiceId = createRequest.InvoiceId,
            ImportedStatus = createRequest.ImportedStatus,
            Date = createRequest.Date,
            UserName = createRequest.UserName,
        };

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        await sqlConnection.OpenAsync();

        int rowsAffected = await sqlConnection.ExecuteAsync(query, parameters);

        return rowsAffected > 0 ? new Success() : new InsertFailedResult();
    }
}