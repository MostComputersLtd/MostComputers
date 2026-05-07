namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
public sealed class GroupPromotionImageFileUpdateRequest
{
    public required int Id { get; set; }
    public required byte[] Image { get; set; }
    public required string FileName { get; set; }
}
