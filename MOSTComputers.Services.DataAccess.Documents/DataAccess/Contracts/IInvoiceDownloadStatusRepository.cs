using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.InvoiceDownloadStatus;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface IInvoiceDownloadStatusRepository
{
    Task<List<InvoiceDownloadStatus>> GetLatestByInvoiceIdsAsync(List<int> invoiceIds);
    Task<InvoiceDownloadStatus?> GetLatestByInvoiceIdAsync(int invoiceId);
    Task<OneOf<Success, InsertFailedResult>> InsertAsync(InvoiceDownloadStatusCreateRequest createRequest);
}