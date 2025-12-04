using MOSTComputers.Services.PDF.Models.Invoices;
using MOSTComputers.Services.PDF.Services.Contracts;

using static MOSTComputers.Services.PDF.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.PDF.Tests.Integration;
[Collection(DefaultTestCollection.Name)]
public class PdfInvoiceFileGeneratorServiceTests
{
    public PdfInvoiceFileGeneratorServiceTests(IPdfInvoiceFileGeneratorService pdfInvoiceService)
    {
        _pdfInvoiceService = pdfInvoiceService;
    }

    private readonly IPdfInvoiceFileGeneratorService _pdfInvoiceService;

    [Fact]
    public async Task CreateInvoicePdfAndSaveAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        InvoiceData invoiceData = GetValidInvoiceData();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfInvoiceTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        await _pdfInvoiceService.CreateInvoicePdfAndSaveAsync(invoiceData, pdfInvoiceTestFileFullPath);
    }

    [Fact]
    public async Task CreateInvoicePdfAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        InvoiceData invoiceData = GetValidInvoiceData();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfInvoiceTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        byte[] getPdfInvoicePdfResult = await _pdfInvoiceService.CreateInvoicePdfAsync(invoiceData);

        await File.WriteAllBytesAsync(pdfInvoiceTestFileFullPath, getPdfInvoicePdfResult);
    }
}