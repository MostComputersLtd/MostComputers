namespace MOSTComputers.Services.PDF.Models.Invoices;
public class InvoiceData
{
    public required int InvoiceNumber { get; set; }
    public required DateTime? Date { get; set; }
    public required DateTime? RDate { get; set; }
    public required DateTime? DueDate { get; set; }
    public int? FirmId { get; set; }

    public required FirmInvoiceData RecipientData { get; set; }
    public required FirmInvoiceData SupplierData { get; set; }
    public required List<PurchaseInvoiceData> Purchases { get; set; }

    public required decimal VatPercentageFraction { get; set; }
    public required decimal LevaToEuroExchangeRate { get; set; }
    public string? RecipientFullName { get; set; }
    public required string TypeOfPayment { get; set; }
    public string? AuthorFullName { get; set; }

    internal int? InvoiceDirection { get; set; }
    internal int? PratkaId { get; set; }
    internal int? InvType { get; set; }
    internal string? InvBasis { get; set; }
    internal string? RelatedInvNo { get; set; }
    internal int? IsVATRegistered { get; set; }
}