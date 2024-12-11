using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MOSTComputers.Services.Identity.Models;
using OneOf.Types;
using OneOf;
using MOSTComputers.Services.Identity.DAL;

namespace MOSTComputers.Services.Identity.Services;

internal sealed class IdentityService : IIdentityService<IdentityUser, IdentityRole>
{
    public IdentityService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, DefaultAuthenticationDBContext dBContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dBContext = dBContext;
    }

    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DefaultAuthenticationDBContext _dBContext;

    public async Task<List<IdentityUser>> GetAllUsersAsync()
    {
        return await _dBContext.Users.ToListAsync();
    }

    public async Task<IdentityUser?> GetUserAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> TryAddUserAsync(IdentityUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return result;
    }

    public async Task<IdentityResult> TryAddRoleAsync(IdentityRole role)
    {
        IdentityResult result = await _roleManager.CreateAsync(role);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRoleAsync(IdentityUser user, string roleName)
    {
        IdentityResult result = await _userManager.AddToRoleAsync(user, roleName);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(IdentityUser user, params string[] roleNames)
    {
        IdentityResult result = await _userManager.AddToRolesAsync(user, roleNames);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(IdentityUser user, IEnumerable<string> roleNames)
    {
        IdentityResult result = await _userManager.AddToRolesAsync(user, roleNames);

        return result;
    }

    public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(userId);

        if (user is null) return new NotFound();

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<IdentityResult> TryRemoveUserFromRoleAsync(IdentityUser user, string roleName)
    {
        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, roleName);

        return result;
    }

    public async Task<bool> TryDeleteUserAsync(string userId)
    {
        IdentityUser? user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null) return false;

        IdentityResult result = await _userManager.DeleteAsync(user);

        return true;
    }
}