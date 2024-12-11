namespace MOSTComputers.Services.PDF.Models;

public class FirmInvoiceData
{
    public required string FirmName { get; set; }
    public required string FirmAddress { get; set; }
    public required string MRPersonFullName { get; set; }
    public required string VatId { get; set; }
    public required string FirmOrPersonId { get; set; }
    public string? BankId { get; set; }
    public string? Iban { get; set; }
}