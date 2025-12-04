using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductImages;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductProperties;

public sealed class ProductPropertiesEditorImagesListPartialModel
{
    public required int ProductId { get; init; }
    public required List<ProductImageDisplayData> Images { get; init; }
    public List<ProductImageFileDisplayData>? RelatedImageFileData { get; init; }
    public List<PromotionProductFileInfo>? PromotionProductFileInfos { get; init; }
    public required bool IncludeHtmlDataView { get; init; }
    public string? ContainerElementId { get; init; }
    public string? ProductPropertiesEditorContainerElementId { get; init; }
    public string? ProductTableContainerElementId { get; init; }
    public string? RelatedProductTableRowElementId { get; init; }
}