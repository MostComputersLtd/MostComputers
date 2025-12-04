using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface IWarrantyCardRepository
{
    Task<List<WarrantyCard>> GetAllMatchingAsync(WarrantyCardSearchRequest warrantyCardSearchRequest);
    Task<WarrantyCard?> GetWarrantyCardByOrderIdAsync(int warrantyCardId);
    Task<List<WarrantyCard>> GetWarrantyCardByOrderIdsAsync(List<int> warrantyCardOrderIds);
    Task<List<WarrantyCardCustomerData>> GetWarrantyCardCustomerInfosAsync(WarrantyCardSearchRequest warrantyCardSearchRequest);
    Task<List<WarrantyCardCustomerData>> GetWarrantyCardCustomerInfosByNameAsync(string keyword, WarrantyCardSearchRequest? warrantyCardSearchRequest = null);
}