using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCardDownloadStatus;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface IWarrantyCardDownloadStatusRepository
{
    Task<WarrantyCardDownloadStatus?> GetLatestByWarrantyCardIdAsync(int warrantyCardId);
    Task<List<WarrantyCardDownloadStatus>> GetLatestByWarrantyCardIdsAsync(List<int> warrantyCardIds);
    Task<OneOf<Success, InsertFailedResult>> InsertAsync(WarrantyCardDownloadStatusCreateRequest createRequest);
}