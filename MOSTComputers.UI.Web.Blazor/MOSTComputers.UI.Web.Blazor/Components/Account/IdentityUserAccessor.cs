using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.UI.Web.Blazor.Components.Account;
internal sealed class IdentityUserAccessor(UserManager<PasswordsTableOnlyUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<PasswordsTableOnlyUser> GetRequiredUserAsync(HttpContext context)
    {
        PasswordsTableOnlyUser? user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus(
                "Account/InvalidUser",
                $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.",
                context);
        }

        return user;
    }
}