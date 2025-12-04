using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;

using static MOSTComputers.Services.PDF.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.PDF.Tests.Integration;
public class PdfWarrantyCardWithPricesFileGeneratorServiceTests
{
    public PdfWarrantyCardWithPricesFileGeneratorServiceTests(IPdfWarrantyCardWithPricesFileGeneratorService pdfWarrantyCardWithPricesFileGeneratorService)
    {
        _pdfWarrantyCardWithPricesFileGeneratorService = pdfWarrantyCardWithPricesFileGeneratorService;
    }

    private readonly IPdfWarrantyCardWithPricesFileGeneratorService _pdfWarrantyCardWithPricesFileGeneratorService;

    [Fact]
    public async Task CreateWarrantyCardPdfAndSaveAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        WarrantyCardWithPricesData warrantyCardWithPricesData = GetValidWarrantyDataWithPrices();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        await _pdfWarrantyCardWithPricesFileGeneratorService.CreateWarrantyCardPdfAndSaveAsync(warrantyCardWithPricesData, pdfInvoiceTestFileFullPath);
    }

    [Fact]
    public async Task CreateWarrantyCardPdfAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        WarrantyCardWithPricesData warrantyCardWithPricesData = GetValidWarrantyDataWithPrices();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        byte[] getPdfInvoicePdfResult = await _pdfWarrantyCardWithPricesFileGeneratorService.CreateWarrantyCardPdfAsync(warrantyCardWithPricesData);

        await File.WriteAllBytesAsync(pdfInvoiceTestFileFullPath, getPdfInvoicePdfResult);
    }
}