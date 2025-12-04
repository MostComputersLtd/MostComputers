using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.PromotionFiles;

namespace MOSTComputers.UI.Web.Pages.Shared.PromotionFiles;
public class PromotionFileSingleEditorPartialModel
{
    public required ModalData ModalData { get; init; }
    public required string PromotionFilesTableContainerElementId { get; init; }
    public PromotionFileInfoEditorDisplayData? ExistingPromotionFileInfo { get; init; }
}