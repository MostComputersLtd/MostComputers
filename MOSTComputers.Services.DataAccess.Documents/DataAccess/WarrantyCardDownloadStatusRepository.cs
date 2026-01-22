using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.Configuration;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCardDownloadStatus;
using OneOf;
using OneOf.Types;
using System.Transactions;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.WarrantyCardDownloadStatusesTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess;

internal sealed class WarrantyCardDownloadStatusRepository : IWarrantyCardDownloadStatusRepository
{
    public WarrantyCardDownloadStatusRepository([FromKeyedServices(ConfigureServices.DocumentsDataAccessServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<WarrantyCardDownloadStatus>> GetLatestByWarrantyCardIdsAsync(List<int> warrantyCardIds)
    {
        const string query =
            $"""
            SELECT {IdColumn},
                {ExportIdColumn},
            	{OrderIdColumn},
            	{ImportedStatusColumn},
            	{DateColumn},
            	{UserNameColumn}
            FROM {WarrantyCardDownloadStatusesTableName} WITH (NOLOCK)
            WHERE {OrderIdColumn} IN @warrantyCardIds
            ORDER BY {DateColumn} DESC
            """;

        var parameters = new
        {
            warrantyCardIds = warrantyCardIds,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        await sqlConnection.OpenAsync();

        IEnumerable<WarrantyCardDownloadStatus> data = await sqlConnection.QueryAsync<WarrantyCardDownloadStatus>(query, parameters);

        transactionScope.Complete();

        return data
            .GroupBy(x => x.OrderId)
            .Select(x => x.First())
            .ToList();
    }

    public async Task<WarrantyCardDownloadStatus?> GetLatestByWarrantyCardIdAsync(int warrantyCardId)
    {
        const string query =
            $"""
            SELECT TOP 1 {IdColumn},
                {ExportIdColumn},
            	{OrderIdColumn},
            	{ImportedStatusColumn},
            	{DateColumn},
            	{UserNameColumn}
            FROM {WarrantyCardDownloadStatusesTableName} WITH (NOLOCK)
            WHERE {OrderIdColumn} = @warrantyCardId
            ORDER BY {DateColumn} DESC
            """;

        var parameters = new
        {
            warrantyCardId = warrantyCardId,
        };

        using TransactionScope transactionScope = new(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

        using SqlConnection sqlConnection = new(_connectionStringProvider.ConnectionString);

        await sqlConnection.OpenAsync();

        WarrantyCardDownloadStatus? data = await sqlConnection.QueryFirstOrDefaultAsync<WarrantyCardDownloadStatus>(query, parameters);

        transactionScope.Complete();

        return data;
    }

    public async Task<OneOf<Success, InsertFailedResult>> InsertAsync(WarrantyCardDownloadStatusCreateRequest createRequest)
    {
        const string query =
            $"""
            INSERT INTO {WarrantyCardDownloadStatusesTableName} ({ExportIdColumn}, {OrderIdColumn}, {ImportedStatusColumn}, {DateColumn}, {UserNameColumn})
            VALUES (@exportId, @orderId, @ImportedStatus, @Date, @UserName)
            """;

        var parameters = new
        {
            exportId = createRequest.ExportId,
            orderId = createRequest.OrderId,
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