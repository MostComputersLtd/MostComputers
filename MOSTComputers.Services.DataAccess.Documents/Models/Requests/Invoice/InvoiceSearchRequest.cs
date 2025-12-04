namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;
public sealed class InvoiceSearchRequest
{
    public List<InvoiceByIdSearchRequest>? InvoiceByIdSearchRequests { get; set; }
    public List<InvoiceByStringSearchRequest>? InvoiceByStringSearchRequests { get; set; }
    public List<InvoiceByDateFilterRequest>? InvoiceByDateFilterRequests { get; set; }
}