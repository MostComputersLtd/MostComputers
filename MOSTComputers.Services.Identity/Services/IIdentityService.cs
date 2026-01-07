using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.Identity.Services;
public interface IIdentityService<TUser, TRole>
{
    Task<List<TUser>> GetAllUsersAsync();
    Task<TUser?> GetUserAsync(string userId);
    Task<IdentityResult> TryAddUserAsync(TUser user, string password);
    Task<IdentityResult> TryAddRoleAsync(TRole role);
    Task<IdentityResult> TryAddUserToRoleAsync(TUser user, string roleName);
    Task<IdentityResult> TryAddUserToRolesAsync(TUser user, params string[] roleNames);
    Task<IdentityResult> TryAddUserToRolesAsync(TUser user, IEnumerable<string> roleNames);
    Task<bool> IsInRoleAsync(TUser user, string roleName);
    Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword);
    Task<bool> TryDeleteUserAsync(string userId);
    Task<IdentityResult> TryRemoveUserFromRoleAsync(TUser user, string roleName);
    Task<List<PasswordsTableOnlyUser>> GetUsersAsync(List<string> userIds);
    Task<IList<string>> GetUserRoleNamesAsync(PasswordsTableOnlyUser user);
    Task<IdentityResult> TryRemoveUserFromRolesAsync(PasswordsTableOnlyUser user, IEnumerable<string> roleName);
}