using OneOf;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

public sealed class ProductImageWithFileForProductUpsertRequest
{
    public required int? ExistingImageId { get; init; }
    public required string FileExtension { get; init; }
    public required byte[] ImageData { get; init; }
    public OneOf<UpdateHtmlDataToMatchCurrentProductData, DoNotUpdateHtmlData, UpdateToCustomHtmlData> HtmlDataOptions { get; init; }
        = new UpdateHtmlDataToMatchCurrentProductData();

    public FileForImageForProductUpsertRequest? FileUpsertRequest { get; init; }
}

public sealed class FileForImageForProductUpsertRequest
{
    public int? CustomDisplayOrder { get; init; }
    public bool? Active { get; init; }
}