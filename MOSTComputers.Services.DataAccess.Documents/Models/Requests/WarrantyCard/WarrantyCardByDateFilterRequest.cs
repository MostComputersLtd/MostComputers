namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;

public sealed class WarrantyCardByDateFilterRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public required WarrantyCardByDateSearchOptions SearchOption { get; set; }
}