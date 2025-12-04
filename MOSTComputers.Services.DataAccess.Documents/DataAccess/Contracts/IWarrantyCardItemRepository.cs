using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface IWarrantyCardItemRepository
{
    Task<List<WarrantyCardItem>> GetWarrantyCardItemsInExportAsync(int exportId);
    Task<List<WarrantyCardItem>> GetWarrantyCardItemsInWarrantyCardAsync(int warrantyCardId);
    Task<List<WarrantyCardItem>> GetWarrantyCardItemsWithProductIdAsync(int productId);
}