using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.Services.Identity.Services;
public interface IPasswordsTableOnlyIdentityService
{
    Task<IdentityResult> TryAddUserAsync(PasswordsTableOnlyUser user, string password);
}