using MOSTComputers.Services.PDF.Models.WarrantyCards;

namespace MOSTComputers.Services.PDF.Services.Contracts;
public interface IPdfWarrantyCardWithPricesFileGeneratorService
{
    Task CreateWarrantyCardPdfAndSaveAsync(WarrantyCardWithPricesData warrantyCardWithPricesData, string destinationFilePath);
    Task<byte[]> CreateWarrantyCardPdfAsync(WarrantyCardWithPricesData warrantyCardWithPricesData);
}