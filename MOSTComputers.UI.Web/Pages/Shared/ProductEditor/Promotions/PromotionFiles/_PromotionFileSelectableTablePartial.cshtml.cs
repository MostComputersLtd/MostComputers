using MOSTComputers.UI.Web.Models.PromotionFiles;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.Promotions.PromotionFiles;

public sealed class PromotionFileSelectableTablePartialModel
{
    public required List<PromotionFileInfoEditorDisplayData> PromotionFiles { get; init; }
    public int? SelectedPromotionFileIndex { get; init; }
}