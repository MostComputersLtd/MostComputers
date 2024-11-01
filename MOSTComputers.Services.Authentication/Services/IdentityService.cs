using Microsoft.AspNetCore.Identity;

namespace MOSTComputers.Services.Identity.Services;

internal sealed class IdentityService : IIdentityService
{
    public IdentityService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    private readonly UserManager<IdentityUser> _userManager;

    public async Task<IdentityResult> TryAddUserAsync(IdentityUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return result;
    }
}