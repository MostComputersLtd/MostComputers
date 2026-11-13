using MOSTComputers.Services.PDF.Models.HomeOffers;

namespace MOSTComputers.Services.PDF.Services.Contracts;
public interface IPdfHomeOfferFileGeneratorService
{
    Task<Stream> CreateHomeOfferPdfAndGetStreamAsync(HomeOfferData homeOfferData);
    Task CreateHomeOfferPdfAndSaveAsync(HomeOfferData homeOfferData, string destinationFilePath);
    Task<byte[]> CreateHomeOfferPdfAsync(HomeOfferData homeOfferData);
}
