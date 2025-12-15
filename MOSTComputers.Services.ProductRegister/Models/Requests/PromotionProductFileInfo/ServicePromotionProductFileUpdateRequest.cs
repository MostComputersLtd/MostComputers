using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
public sealed class ServicePromotionProductFileUpdateRequest
{
    public required int Id { get; init; }
    public int? NewPromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required bool Active { get; init; }
    public ServicePromotionProductImageUpsertRequest? UpsertInProductImagesRequest { get; init; }
    public required string UpdateUserName { get; init; }
}

public sealed class ServicePromotionProductImageUpsertRequest
{
    public required ServicePromotionProductImageFileUpsertRequest ImageFileUpsertRequest { get; init; }
    public OneOf<UpdateHtmlDataToMatchCurrentProductData, DoNotUpdateHtmlData, UpdateToCustomHtmlData> HtmlDataOptions { get; init; }
        = new UpdateHtmlDataToMatchCurrentProductData();
}

public sealed class ServicePromotionProductImageFileUpsertRequest
{
    public bool? Active { get; init; }
    public int? CustomDisplayOrder { get; init; }
}