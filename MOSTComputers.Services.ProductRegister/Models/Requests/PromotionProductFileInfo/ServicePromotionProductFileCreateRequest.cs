namespace MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
public sealed class ServicePromotionProductFileCreateRequest
{
    public required int ProductId { get; init; }
    public required int PromotionFileInfoId { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public required bool Active { get; init; }
    public ServicePromotionProductImageCreateRequest? CreateInProductImagesRequest { get; init; }
    public required string CreateUserName { get; init; }
}

public sealed class ServicePromotionProductImageCreateRequest
{
    public required ServicePromotionProductImageFileCreateRequest ImageFileCreateRequest { get; init; }
    public string? HtmlData { get; init; }
}

public sealed class ServicePromotionProductImageFileCreateRequest
{
    public bool? Active { get; init; }
    public int? CustomDisplayOrder { get; init; }
}