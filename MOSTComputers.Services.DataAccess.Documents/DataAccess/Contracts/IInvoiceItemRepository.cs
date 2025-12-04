using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface IInvoiceItemRepository
{
    Task<InvoiceItem?> GetInvoiceItemByIdAsync(int id);
    Task<List<InvoiceItem>> GetInvoiceItemsInExportAsync(int exportId);
    Task<List<InvoiceItem>> GetInvoiceItemsInInvoiceAsync(int invoiceId);
}