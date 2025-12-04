using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.ProductEditor;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor;

public sealed class ProductDataTableRowPartialModel
{
    public required ProductEditorProductData ProductData { get; init; }
    public required string ProductTableEntryElementIdPrefix { get; init; }

    public required int ElementIndex { get; init; }

    public required string PartialViewContainerElementId { get; init; }
    public required ModalData ProductPropertiesEditorPartialModalData { get; init; }
    public required ModalData SearchStringPartsPartialModalData { get; init; }
    public required ModalData XmlViewPartialModalData { get; init; }
    public required ModalData ProductPropertiesPartialModalData { get; init; }
    public required ModalData OldXmlPropertiesPartialModalData { get; init; }
    public required ModalData ImagesPartialModalData { get; init; }
    public required ModalData ImageFilesPartialModalData { get; init; }
    public required ModalData ImageFileNameInfosPartialModalData { get; init; }
    public required ModalData PromotionViewPartialModalData { get; init; }
    public required ModalData InfoPromotionViewPartialModalData { get; init; }
    public required ModalData PromotionProductFileEditorPartialModalData { get; init; }
}