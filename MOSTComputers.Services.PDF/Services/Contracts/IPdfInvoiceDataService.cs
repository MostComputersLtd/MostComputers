using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.PDF.Models.Invoices;

namespace MOSTComputers.Services.PDF.Services.Contracts;
public interface IPdfInvoiceDataService
{
    Task<InvoiceData?> GetInvoiceDataByIdAsync(int invoiceId);
    Task<InvoiceData?> GetInvoiceDataByNumberAsync(string invoiceNumber);
    InvoiceData GetPdfInvoiceDataFromInvoice(Invoice invoice);
}