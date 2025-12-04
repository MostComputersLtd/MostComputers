namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
public sealed class GroupPromotionImageFileDataCreateRequest
{
    public int? PromotionId { get; set; }
    public required int ImageId { get; set; }
    public required string FileName { get; set; }
}