namespace MOSTComputers.Services.DataAccess.Documents.Models.DAO;

internal sealed class InvoiceDAO
{
    public int ExportId { get; set; }
    public DateTime? ExportDate { get; set; }
    public int? ExportUserId { get; set; }
    public string? ExportUser { get; set; }
    public int InvoiceId { get; set; }
    public int? FirmId { get; set; }
    public int? CustomerBID { get; set; }
    public int? InvoiceDirection { get; set; }
    public string? CustomerName { get; set; }
    public string? MPerson { get; set; }
    public string CustomerAddress { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public int? PDDC { get; set; }
    public string? UserName { get; set; }
    public int? Status { get; set; }
    public string? InvoiceNumber { get; set; }
    public int? PayType { get; set; }
    public string? RPerson { get; set; }
    public DateTime? RDATE { get; set; }
    public string? Bulstat { get; set; }
    public int? PratkaId { get; set; }
    public int? InvType { get; set; }
    public string? InvBasis { get; set; }
    public string? RelatedInvNo { get; set; }
    public int? IsVATRegistered { get; set; }
    public decimal? PrintedNETAmount { get; set; }
    public DateTime? DueDate { get; set; }
    public string? CustomerBankName { get; set; }
    public string? CustomerBankIBAN { get; set; }
    public string? CustomerBankBIC { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime? PaymentStatusDate { get; set; }
    public string? PaymentStatusUserName { get; set; }
    public int? InvoiceCurrency { get; set; }

    public List<InvoiceItemDAO>? InvoiceItems { get; set; }
}