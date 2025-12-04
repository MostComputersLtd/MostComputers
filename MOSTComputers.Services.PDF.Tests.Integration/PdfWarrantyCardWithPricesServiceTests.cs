using MOSTComputers.Services.PDF.Services.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.PDF.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.PDF.Tests.Integration;
public class PdfWarrantyCardWithPricesServiceTests
{
    public PdfWarrantyCardWithPricesServiceTests(IPdfWarrantyCardWithPricesService pdfWarrantyCardWithPricesService)
    {
        _pdfWarrantyCardWithPricesService = pdfWarrantyCardWithPricesService;
    }

    private readonly IPdfWarrantyCardWithPricesService _pdfWarrantyCardWithPricesService;

    [Fact]
    public async Task GeneratePdfFromDataAndSaveAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        int warrantyCardId = GetValidWarrantyCardId();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfWarrantyCardTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithoutPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        await _pdfWarrantyCardWithPricesService.GeneratePdfFromDataAndSaveAsync(warrantyCardId, pdfWarrantyCardTestFileFullPath);
    }

    [Fact]
    public async Task GeneratePdfFromDataAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        int warrantyCardId = GetValidWarrantyCardId();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfWarrantyCardTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithoutPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        OneOf<byte[], NotFound> generatePdfResult = await _pdfWarrantyCardWithPricesService.GeneratePdfFromDataAsync(warrantyCardId);

        Assert.True(generatePdfResult.Match(
            pdfData => true,
            notFound => false));
    }
}