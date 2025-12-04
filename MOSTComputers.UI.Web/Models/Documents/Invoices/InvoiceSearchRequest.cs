namespace MOSTComputers.UI.Web.Models.Documents.Invoices;

public sealed class InvoiceSearchRequest
{
    public int? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Keyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}