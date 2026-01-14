namespace MOSTComputers.Services.PDF.Models.Invoices;

public sealed class RecipientFirmInvoiceData
{
    public string? FirmName { get; set; }
    public required string FirmAddress { get; set; }
    public string? MRPersonFullName { get; set; }
    public string? VatId { get; set; }
    public string? FirmOrPersonId { get; set; }
}