namespace MOSTComputers.Services.PDF.Models.Invoices;

public class FirmInvoiceData
{
    public string? FirmName { get; set; }
    public required string FirmAddress { get; set; }
    public string? MRPersonFullName { get; set; }
    public string? VatId { get; set; }
    public string? FirmOrPersonId { get; set; }
    public string? BankId { get; set; }
    public string? Iban { get; set; }
}