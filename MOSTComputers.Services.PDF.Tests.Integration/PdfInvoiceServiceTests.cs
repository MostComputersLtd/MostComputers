using MOSTComputers.Services.PDF.Models;
using MOSTComputers.Services.PDF.Services.Contracts;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MOSTComputers.Services.PDF.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.PDF.Tests.Integration;
[Collection(DefaultTestCollection.Name)]
public class PdfInvoiceServiceTests
{
    public PdfInvoiceServiceTests(IPdfInvoiceService pdfInvoiceGeneratorService)
    {
        _pdfInvoiceGeneratorService = pdfInvoiceGeneratorService;
    }

    private readonly IPdfInvoiceService _pdfInvoiceGeneratorService;

    [Fact]
    public async Task GeneratePdfFromDataAndSaveAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        int invoiceId = GetValidInvoiceId();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfInvoiceTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        await _pdfInvoiceGeneratorService.GeneratePdfFromDataAndSaveAsync(invoiceId, pdfInvoiceTestFileFullPath);
    }

    [Fact]
    public async Task GeneratePdfFromDataAsync_ShouldSucceed_WhenDocumentIsLoadedAndDestinationFolderExistsAsync()
    {
        int invoiceId = GetValidInvoiceId();

        string pdfTestFileName = $"{Guid.NewGuid()}.pdf";
        string pdfInvoiceTestFileFullPath = Path.Combine(Startup.PdfInvoiceTestFolderFullPath, pdfTestFileName).Replace("\\", "/");

        OneOf<byte[], NotFound> generatePdfResult = await _pdfInvoiceGeneratorService.GeneratePdfFromDataAsync(invoiceId);

        Assert.True(generatePdfResult.Match(
            pdfData => true,
            notFound => false));
    }
}