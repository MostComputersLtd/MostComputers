using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.Accounts.Admin;

public class UsersListPartialModel
{
    public UsersListPartialModel()
    {
    }

    public IReadOnlyList<UserDisplayData>? Users { get; init; }
    public required string ContainerElementId { get; init; }
    public string? NotificationBoxId { get; init; } = null;
}