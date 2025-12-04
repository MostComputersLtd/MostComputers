using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.Authentication;

namespace MOSTComputers.UI.Web.Pages.Shared.Accounts.Admin;
public class UserListPartialModel
{
    public IReadOnlyList<UserDisplayData>? Users { get; init; }
    public required string ContainerElementId { get; init; }
    public required ModalData ChangePasswordModalData { get; init; }
    public required ModalData UserDeleteConfirmationModalData { get; init; }
    public string? NotificationBoxId { get; init; } = null;
}