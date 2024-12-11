using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.Accounts.Admin;

public class ChangePasswordPopupPartialModel
{
    public required UserDisplayData UserData { get; init; }
    public required string ModalContainerId { get; init; }
    public string? NotificationBoxId { get; init; } = null;
}