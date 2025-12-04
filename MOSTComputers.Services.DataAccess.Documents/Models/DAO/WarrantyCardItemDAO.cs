namespace MOSTComputers.Services.DataAccess.Documents.Models.DAO;
internal sealed class WarrantyCardItemDAO
{
    public int ExportedItemId { get; set; }
    public int? ExportId { get; set; }
    public int OrderId { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal? PriceInLeva { get; set; }
    public int? Quantity { get; set; }
    public string? SerialNumber { get; set; }
    public int? WarrantyCardItemTermInMonths { get; set; }
    public int? DisplayOrder { get; set; }
}