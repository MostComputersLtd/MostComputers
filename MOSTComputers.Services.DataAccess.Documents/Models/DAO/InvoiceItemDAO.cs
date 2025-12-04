namespace MOSTComputers.Services.DataAccess.Documents.Models.DAO;
internal class InvoiceItemDAO
{
    public int ExportedItemId { get; set; }
    public int? ExportId { get; set; }
    public int IEID { get; set; }
    public int? InvoiceId { get; set; }
    public string? Name { get; set; }
    public decimal? PriceInLeva { get; set; }
    public int? Quantity { get; set; }
    public int? DisplayOrder { get; set; }
}