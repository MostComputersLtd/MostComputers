namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;

public sealed class InvoiceByStringSearchRequest
{
    public required string Keyword { get; set; }
    public required InvoiceByStringSearchOptions SearchOption { get; set; }
    public required InvoiceByStringSearchType SearchType { get; set; }
}