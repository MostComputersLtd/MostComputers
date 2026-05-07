namespace MOSTComputers.Services.ProductRegister.Models.Responses;

public sealed class GroupPromotionCreateResult
{
    public required int Id { get; set; }
    public List<int>? ImageIds { get; set; }
    public List<int>? ImageFileIds { get; set; }
}