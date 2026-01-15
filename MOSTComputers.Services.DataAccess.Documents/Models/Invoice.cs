namespace MOSTComputers.Services.DataAccess.Documents.Models;
public class Invoice
{
    public int ExportId { get; init; }
    public DateTime? ExportDate { get; init; }
    public int? ExportUserId { get; init; }
    public string? ExportUser { get; init; }
    public int InvoiceId { get; init; }
    public int? FirmId { get; init; }
    public int? CustomerBID { get; init; }
    public InvoiceDirection? InvoiceDirection { get; init; }
    public string? CustomerName { get; init; }
    public string? MPerson { get; init; }
    public string CustomerAddress { get; init; }
    public DateTime? InvoiceDate { get; init; }
    public int? VatPercent { get; init; }
    public string? UserName { get; init; }
    public int? Status { get; init; }
    public string? InvoiceNumber { get; init; }
    public int? PayType { get; init; }
    public string? RPerson { get; init; }
    public DateTime? RDATE { get; init; }
    public string? Bulstat { get; init; }
    public int? PratkaId { get; init; }
    public int? InvType { get; init; }
    public string? InvBasis { get; init; }
    public string? RelatedInvoiceNumber { get; init; }
    public int? IsVATRegistered { get; init; }
    public decimal? PrintedNETAmount { get; init; }
    public DateTime? DueDate { get; init; }
    public string? BankNameAndId { get; init; }
    public string? BankIBAN { get; init; }
    public string? CustomerBankBIC { get; init; }
    public string? PaymentStatus { get; init; }
    public DateTime? PaymentStatusDate { get; init; }
    public string? PaymentStatusUserName { get; init; }
    public int? InvoiceCurrency { get; init; }

    public List<InvoiceItem>? InvoiceItems { get; init; }
}