using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.Promotions.PromotionFiles;
public class PromotionProductFileEditorPartialModel
{
    public required Product Product { get; init; }
    public List<Promotion>? Promotions { get; init; }
    public List<PromotionProductFileInfo>? PromotionProductFileInfos { get; init; }

    public required ModalData ModalData { get; init; }
    public required ModalData PromotionProductFileInfoSingleEditorPartialModalData { get; init; }
    public string? ProductPropertiesEditorContainerElementId { get; init; }
    public string? ProductTableContainerElementId { get; init; }
    public string? RelatedProductTableRowElementId { get; init; }
}