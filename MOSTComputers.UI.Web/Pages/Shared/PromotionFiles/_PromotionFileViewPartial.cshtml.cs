using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.PromotionFiles;

namespace MOSTComputers.UI.Web.Pages.Shared.PromotionFiles;
public class PromotionFileViewPartialModel
{
    public required string ContainerElementId { get; init; }
    public string? NotificationBoxId { get; init; }
    public IReadOnlyList<PromotionFileInfoEditorDisplayData>? PromotionFiles { get; init; }
    public required ModalData PromotionFileSingleEditorModalData { get; init; }
}