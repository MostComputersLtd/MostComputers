namespace MOSTComputers.Services.DataAccess.Documents.Models.DAO;
internal sealed class WarrantyCardDAO
{
    public int ExportId { get; set; }
    public DateTime? ExportDate { get; set; }
    public int? ExportUserId { get; set; }
    public string? ExportUser { get; set; }
    public int OrderId { get; set; }
    public int? CustomerBID { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? WarrantyCardDate { get; set; }
    public int? WarrantyCardTerm { get; set; }

    public List<WarrantyCardItemDAO>? WarrantyCardItems { get; set; }
}