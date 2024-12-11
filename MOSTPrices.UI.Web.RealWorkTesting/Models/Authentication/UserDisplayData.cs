using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;

public class UserDisplayData
{
    public required PasswordsTableOnlyUser User { get; init; }
    public List<UserRoles>? UserRoles { get; init; }
}