using MOSTComputers.UI.Web.Models.Documents.Invoices;

namespace MOSTComputers.UI.Web.Pages.Shared.Documents;

public sealed class InvoiceTablePartialModel
{
    public List<InvoiceDisplayData>? Invoices { get; init; }
}