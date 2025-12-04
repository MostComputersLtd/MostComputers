using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.PDF.Models.WarrantyCards;

namespace MOSTComputers.Services.PDF.Services.Contracts;
public interface IPdfWarrantyCardDataService
{
    WarrantyCardWithoutPricesData GetWarrantyCardDataWithoutPrices(WarrantyCard warrantyCard);
    Task<WarrantyCardWithoutPricesData?> GetWarrantyCardDataWithoutPricesByOrderIdAsync(int warrantyCardId);
    WarrantyCardWithPricesData GetWarrantyCardDataWithPrices(WarrantyCard warrantyCard);
}