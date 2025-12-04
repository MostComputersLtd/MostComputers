using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Files;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductProperties;
public class ProductPropertiesEditorPartialModel
{
    internal const string ImagesListPartialContainerElementId = "productEditorImagesContainer";

    public required Product Product { get; init; }
    public ProductWorkStatuses? ProductStatuses { get; init; }
    public required List<ProductPropertyEditorPropertyData> PropertyDataList { get; init; }
    public required List<ProductImageDisplayData> ProductImages { get; init; }
    public required List<ProductImageFileDisplayData> ProductImageFileNames { get; init; }
    public List<SearchStringPartOriginData>? SearchStringParts { get; init; }
    public List<PromotionProductFileInfo>? PromotionProductFileInfos { get; init; }
    public List<Promotion>? Promotions { get; init; }

    public required ModalData ModalData { get; init; }
    public required ModalData ProductFullDisplayPartialModalData { get; init; }
    public required ModalData ProductHtmlAndImagesPartialModelData { get; init; }
    public required ModalData PromotionProductFileEditorPartialModalData { get; init; }
    public required ModalData SearchStringPartsPartialModalData { get; init; }
    public string? ProductTableContainerElementId { get; init; }
    public string? RelatedProductTableRowElementId { get; init; }
}