namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;

public sealed class WarrantyCardByIdSearchRequest
{
    public required int Id { get; set; }
    public required WarrantyCardByIdSearchOptions SearchOption { get; set; }
}