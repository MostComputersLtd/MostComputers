using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.Services.Identity.Services;

internal sealed class PasswordsTableOnlyIdentityService : IPasswordsTableOnlyIdentityService
{
    public PasswordsTableOnlyIdentityService(UserManager<PasswordsTableOnlyUser> userManager)
    {
        _userManager = userManager;
    }

    private readonly UserManager<PasswordsTableOnlyUser> _userManager;

    public async Task<IdentityResult> TryAddUserAsync(PasswordsTableOnlyUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return result;
    }
}