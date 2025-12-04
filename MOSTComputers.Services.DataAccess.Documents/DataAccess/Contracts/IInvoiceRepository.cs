using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllMatchingAsync(InvoiceSearchRequest invoiceSearchRequest);
    Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
    Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);
    Task<List<InvoiceCustomerData>> GetInvoiceCustomerInfosAsync(InvoiceSearchRequest invoiceSearchRequest);
    Task<List<InvoiceCustomerData>> GetInvoiceCustomerInfosByNameAsync(string keyword, InvoiceSearchRequest? invoiceSearchRequest = null);
    Task<List<Invoice>> GetInvoicesByIdsAsync(List<int> invoiceIds);
}