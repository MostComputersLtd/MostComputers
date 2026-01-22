using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
internal interface IInvoiceToXmlService
{
    Task GetXmlForInvoicesAsync(Stream outputStream, IEnumerable<Invoice> invoices, IEnumerable<FirmData>? firmDatas = null);
    Task GetXmlForInvoicesAsync(Stream outputStream, InvoiceSearchRequest invoiceSearchRequest);
}