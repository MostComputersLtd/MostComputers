using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;

using static MOSTComputers.Services.PDF.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.PDF.Tests.Integration;
[Collection(DefaultTestCollection.Name)]
public class PdfWarrantyCardWithoutPricesFileGeneratorServiceTests
{
    public PdfWarrantyCardWithoutPricesFileGeneratorServiceTests(IPdfWarrantyCardWithoutPricesFileGeneratorService pdfWarrantyCardFileGeneratorService)
    {
        _pdfWarrantyCardFileGeneratorService = pdfWarrantyCardFileGeneratorService;
    }

    private readonly IPdfWarrantyCardWithoutPricesFileGeneratorService _pdfWarrantyCardFileGeneratorService;

    [Fact]
    public async Task CreateWarrantyCardPdfAndSaveAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        WarrantyCardWithoutPricesData warrantyCardWithoutPricesData = GetValidWarrantyDataWithoutPrices();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithoutPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        await _pdfWarrantyCardFileGeneratorService.CreateWarrantyCardPdfAndSaveAsync(warrantyCardWithoutPricesData, pdfInvoiceTestFileFullPath);
    }

    [Fact]
    public async Task CreateWarrantyCardPdfAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        WarrantyCardWithoutPricesData warrantyCardWithoutPricesData = GetValidWarrantyDataWithoutPrices();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithoutPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        byte[] getPdfInvoicePdfResult = await _pdfWarrantyCardFileGeneratorService.CreateWarrantyCardPdfAsync(warrantyCardWithoutPricesData);

        await File.WriteAllBytesAsync(pdfInvoiceTestFileFullPath, getPdfInvoicePdfResult);
    }
}