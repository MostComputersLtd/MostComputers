namespace MOSTComputers.UI.Web.Pages.Shared.Accounts.Admin;
public sealed class UserDeleteConfirmationPartialModel
{
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public required string ModalContainerId { get; set; }
    public required string UserListModalContainerId { get; set; }
    public required string NotificationBoxId { get; set; }
}