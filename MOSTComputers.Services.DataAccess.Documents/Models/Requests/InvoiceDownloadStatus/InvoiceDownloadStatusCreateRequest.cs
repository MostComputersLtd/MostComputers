namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.InvoiceDownloadStatus;
public sealed class InvoiceDownloadStatusCreateRequest
{
    public int? ExportId { get; init; }
    public int? InvoiceId { get; init; }
    public string? ImportedStatus { get; init; }
    public DateTime? Date { get; init; }
    public string? UserName { get; init; }
}