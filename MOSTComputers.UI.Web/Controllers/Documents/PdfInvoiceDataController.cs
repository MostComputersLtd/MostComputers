using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.PDF.Models.Invoices;
using MOSTComputers.Services.PDF.Services.Contracts;

namespace MOSTComputers.UI.Web.Controllers.Documents;

[ApiController]
[Route(ControllerRoute)]
public sealed class PdfInvoiceDataController : ControllerBase
{
    public PdfInvoiceDataController(
        IPdfInvoiceDataService pdfInvoiceDataService,
        IPdfInvoiceFileGeneratorService pdfInvoiceFileGeneratorService)
    {
        _pdfInvoiceDataService = pdfInvoiceDataService;
        _pdfInvoiceFileGeneratorService = pdfInvoiceFileGeneratorService;
    }

    internal const string ControllerRoute = ControllerCommonElements.ControllerRoutePrefix + "documents/" + "invoiceData";

    private readonly IPdfInvoiceDataService _pdfInvoiceDataService;
    private readonly IPdfInvoiceFileGeneratorService _pdfInvoiceFileGeneratorService;

    [HttpGet("{invoiceNumber}")]
    public async Task<IActionResult> GetInvoicePdfFromInvoiceNumberAsync(string invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            return new BadRequestObjectResult("The invoice number cannot be null or empty.");
        }

        InvoiceData? invoiceData = await _pdfInvoiceDataService.GetInvoiceDataByNumberAsync(invoiceNumber);

        if (invoiceData is null)
        {
            return new NotFoundObjectResult($"Invoice with number {invoiceNumber} was not found.");
        }

        byte[] fileData = await _pdfInvoiceFileGeneratorService.CreateInvoicePdfAsync(invoiceData);

        string contentType = "application/pdf";

        return new FileContentResult(fileData, contentType);
    }
}