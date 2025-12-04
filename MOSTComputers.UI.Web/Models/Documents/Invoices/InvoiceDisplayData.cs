namespace MOSTComputers.UI.Web.Models.Documents.Invoices;

public sealed class InvoiceDisplayData
{
    public int ExportId { get; init; }
    public DateTime? ExportDate { get; init; }
    public int? ExportUserId { get; init; }
    public string? ExportUser { get; init; }
    public int InvoiceId { get; init; }
    public int? FirmId { get; init; }
    public int? CustomerFirmBID { get; init; }
    public int? InvoiceDirection { get; init; }
    public string? CustomerName { get; init; }
    public string? MPerson { get; init; }
    public required string CustomerAddress { get; init; }
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
    public string? RelatedInvNo { get; init; }
    public int? IsVATRegistered { get; init; }
    public decimal? PrintedNETAmount { get; init; }
    public DateTime? DueDate { get; init; }
    public string? CustomerBankNameAndId { get; init; }
    public string? CustomerBankIBAN { get; init; }
    public string? CustomerBankBIC { get; init; }
    public string? PaymentStatus { get; init; }
    public DateTime? PaymentStatusDate { get; init; }
    public string? PaymentStatusUserName { get; init; }

    public List<InvoiceItemDisplayData>? InvoiceItems { get; init; }

    public decimal? TotalPrice { get; init; }
}