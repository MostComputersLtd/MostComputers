namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;

public sealed class InvoiceByDateFilterRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public required InvoiceByDateSearchOptions SearchOption { get; set; }
}