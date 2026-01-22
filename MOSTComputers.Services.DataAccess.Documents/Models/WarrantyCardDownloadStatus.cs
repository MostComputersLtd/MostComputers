using Microsoft.Data.SqlClient;
using System.Transactions;

namespace MOSTComputers.Services.DataAccess.Documents.Models;

public sealed class WarrantyCardDownloadStatus
{
    public int Id { get; init; }
    public int? ExportId { get; init; }
    public int? OrderId { get; init; }
    public string? ImportedStatus { get; init; }
    public DateTime? Date { get; init; }
    public string? UserName { get; init; }
}