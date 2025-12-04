using MOSTComputers.UI.Web.Models.Authentication;

namespace MOSTComputers.UI.Web.Pages.Shared.Accounts.Admin;
public class ChangePasswordPopupPartialModel
{
    public required UserDisplayData UserData { get; init; }
    public required string ModalContainerId { get; init; }
    public string? NotificationBoxId { get; init; } = null;
}