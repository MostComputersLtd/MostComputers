using MOSTComputers.Services.PDF.Models.Invoices;

namespace MOSTComputers.Services.PDF.Services.Contracts;
public interface IPdfInvoiceFileGeneratorService
{
    Task<Stream> CreateInvoicePdfAndGetStreamAsync(InvoiceData invoiceData);
    Task CreateInvoicePdfAndSaveAsync(InvoiceData invoiceData, string destinationFilePath);
    Task<byte[]> CreateInvoicePdfAsync(InvoiceData invoiceData);
}