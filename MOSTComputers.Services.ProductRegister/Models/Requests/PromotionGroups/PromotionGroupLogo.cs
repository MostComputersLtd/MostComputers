namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;

public sealed class PromotionGroupLogo
{
    public required byte[] Image { get; set; }
    public required string ContentType { get; set; }
}