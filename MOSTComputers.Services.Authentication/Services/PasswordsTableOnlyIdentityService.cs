using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MOSTComputers.Services.Identity.DAL;
using MOSTComputers.Services.Identity.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.Identity.Services;

internal sealed class PasswordsTableOnlyIdentityService : IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole>
{
    public PasswordsTableOnlyIdentityService(
        PasswordsTableOnlyAuthenticationDBContext dBContext,
        UserManager<PasswordsTableOnlyUser> userManager,
        RoleManager<PasswordsTableOnlyRole> roleManager)
    {
        _dBContext = dBContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    private readonly PasswordsTableOnlyAuthenticationDBContext _dBContext;
    private readonly UserManager<PasswordsTableOnlyUser> _userManager;
    private readonly RoleManager<PasswordsTableOnlyRole> _roleManager;

    public async Task<List<PasswordsTableOnlyUser>> GetAllUsersAsync()
    {
        return await _dBContext.Users.ToListAsync();
    }

    public async Task<PasswordsTableOnlyUser?> GetUserAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> TryAddUserAsync(PasswordsTableOnlyUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return result;
    }

    public async Task<IdentityResult> TryAddRoleAsync(PasswordsTableOnlyRole role)
    {
        IdentityResult result = await _roleManager.CreateAsync(role);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        IdentityResult result = await _userManager.AddToRoleAsync(user, roleName);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(PasswordsTableOnlyUser user, params string[] roleNames)
    {
        IdentityResult result = await _userManager.AddToRolesAsync(user, roleNames);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(PasswordsTableOnlyUser user, IEnumerable<string> roleNames)
    {
        IdentityResult result = await _userManager.AddToRolesAsync(user, roleNames);

        return result;
    }

    public async Task<bool> IsInRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword)
    {
        PasswordsTableOnlyUser? user = await _userManager.FindByIdAsync(userId);

        if (user is null) return new NotFound();

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<IdentityResult> TryRemoveUserFromRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, roleName);

        return result;
    }

    public async Task<bool> TryDeleteUserAsync(string userId)
    {
        PasswordsTableOnlyUser? user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null) return false;

        IdentityResult result = await _userManager.DeleteAsync(user);

        return true;
    }
}