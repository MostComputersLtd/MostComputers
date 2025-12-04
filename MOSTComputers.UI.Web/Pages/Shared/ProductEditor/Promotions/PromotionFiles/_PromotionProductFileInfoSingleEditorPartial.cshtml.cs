using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.PromotionFiles;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.Promotions.PromotionFiles;
public sealed class PromotionProductFileInfoSingleEditorPartialModel
{
    public required ModalData ModalData { get; init; }
    public required int ProductId { get; init; }
    public PromotionProductFileInfo? ExistingPromotionProductFileInfo { get; init; }
    public List<PromotionFileInfoEditorDisplayData>? PossiblePromotionFileInfos { get; init; }

    public required string PromotionProductFilesTableContainerElementId { get; init; }
    public string? ProductPropertiesEditorContainerElementId { get; init; }
    public string? ProductTableContainerElementId { get; init; }
    public string? RelatedProductTableRowElementId { get; init; }
}