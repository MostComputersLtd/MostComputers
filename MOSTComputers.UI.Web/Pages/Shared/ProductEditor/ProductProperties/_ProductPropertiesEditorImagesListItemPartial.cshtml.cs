using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductImages;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductProperties;

public sealed class ProductPropertiesEditorImagesListItemPartialModel
{
    public required int ElementIndex { get; init; }
    public required bool IncludeHtmlDataView { get; init; }
    public ProductImageDisplayData? ImageData { get; init; }
    public ProductImageFileDisplayData? ImageFileData { get; init; }
    public PromotionProductFileInfo? RelatedPromotionProductFile { get; init; }
    public string? ProductPropertiesEditorContainerElementId { get; init; }
    public string? ProductTableContainerElementId { get; init; }
    public string? RelatedProductTableRowElementId { get; init; }
}