namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;

public sealed class GroupPromotionImageFileDataUpdateRequest
{
    public required int Id { get; set; }
    public required string NewFileName { get; set; }
}