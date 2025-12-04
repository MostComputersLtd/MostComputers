namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;

public sealed class WarrantyCardByStringSearchRequest
{
    public required string Keyword { get; set; }
    public required WarrantyCardByStringSearchOptions SearchOption { get; set; }
}