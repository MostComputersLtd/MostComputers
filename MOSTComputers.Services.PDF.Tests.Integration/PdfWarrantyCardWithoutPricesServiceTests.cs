using MOSTComputers.Services.PDF.Services.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.PDF.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.PDF.Tests.Integration;
public class PdfWarrantyCardWithoutPricesServiceTests
{
    public PdfWarrantyCardWithoutPricesServiceTests(IPdfWarrantyCardWithoutPricesService pdfWarrantyCardService)
    {
        _pdfWarrantyCardService = pdfWarrantyCardService;
    }

    private readonly IPdfWarrantyCardWithoutPricesService _pdfWarrantyCardService;

    [Fact]
    public async Task GeneratePdfFromDataAndSaveAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        int warrantyCardId = GetValidWarrantyCardId();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfWarrantyCardTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithoutPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        await _pdfWarrantyCardService.GeneratePdfFromDataAndSaveAsync(warrantyCardId, pdfWarrantyCardTestFileFullPath);
    }

    [Fact]
    public async Task GeneratePdfFromDataAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        int warrantyCardId = GetValidWarrantyCardId();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfWarrantyCardTestFileFullPath = Path.Combine(Startup.PdfWarrantyCardWithoutPricesTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        OneOf<byte[], NotFound> generatePdfResult = await _pdfWarrantyCardService.GeneratePdfFromDataAsync(warrantyCardId);

        Assert.True(generatePdfResult.Match(
            pdfData => true,
            notFound => false));
    }
}