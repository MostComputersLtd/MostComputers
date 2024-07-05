using Microsoft.AspNetCore.Identity;

namespace MOSTComputers.Services.Identity.Services;
public interface IIdentityService
{
    Task<IdentityResult> TryAddUserAsync(IdentityUser user, string password);
}