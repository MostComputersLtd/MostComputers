using MOSTComputers.Models.Product.Models.Promotions.Files;

namespace MOSTComputers.Services.DataAccess.Products.Models.DAOs.PromotionProductFileInfo;
internal sealed class PromotionProductFileInfoDAO
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public bool Active { get; init; }
    public int? ProductImageId { get; init; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public string CreateUserName { get; init; }
    public DateTime CreateDate { get; init; }
    public string LastUpdateUserName { get; init; }
    public DateTime LastUpdateDate { get; init; }
    public int PromotionFileInfoId { get; init; }
    public PromotionFileInfo PromotionFileInfo { get; internal set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}