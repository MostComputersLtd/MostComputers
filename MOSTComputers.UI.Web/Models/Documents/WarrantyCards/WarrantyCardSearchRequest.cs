namespace MOSTComputers.UI.Web.Models.Documents.WarrantyCards;

public sealed class WarrantyCardSearchRequest
{
    public string? Keyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}