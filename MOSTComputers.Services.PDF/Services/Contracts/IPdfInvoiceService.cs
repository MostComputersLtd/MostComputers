using MOSTComputers.Services.PDF.Models;
using Spire.Pdf;

namespace MOSTComputers.Services.PDF.Services.Contracts;
public interface IPdfInvoiceService
{
    PdfDocument CreateInvoicePdf(InvoiceData invoiceData, string destinationFilePath);
}