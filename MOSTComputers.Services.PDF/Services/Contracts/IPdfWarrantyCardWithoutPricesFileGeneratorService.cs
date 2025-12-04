using MOSTComputers.Services.PDF.Models.WarrantyCards;

namespace MOSTComputers.Services.PDF.Services.Contracts;
public interface IPdfWarrantyCardWithoutPricesFileGeneratorService
{
    Task<Stream> CreateWarrantyCardPdfAndGetStreamAsync(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData);
    Task CreateWarrantyCardPdfAndSaveAsync(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData, string destinationFilePath);
    Task<byte[]> CreateWarrantyCardPdfAsync(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData);
}