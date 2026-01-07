using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Identity.DAL;
using MOSTComputers.Services.Identity.Models;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.Identity.Services;
internal sealed class PasswordsTableOnlyIdentityService : IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole>
{
    public PasswordsTableOnlyIdentityService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public async Task<List<PasswordsTableOnlyUser>> GetAllUsersAsync()
    {
        await using AsyncServiceScope serviceScope = _serviceScopeFactory.CreateAsyncScope();

        PasswordsTableOnlyAuthenticationDBContext dbContext = serviceScope.ServiceProvider.GetRequiredService<PasswordsTableOnlyAuthenticationDBContext>();

        return await dbContext.Users.AsNoTracking().ToListAsync();
    }

    public async Task<List<PasswordsTableOnlyUser>> GetUsersAsync(List<string> userIds)
    {
        if (userIds.Count <= 0) return new();

        string sql = "SELECT * FROM [Users] WHERE Id IN ";

        List<string> parameterNames = new();

        for (int i = 0; i < userIds.Count; i++)
        {
            parameterNames.Add("@userId" + i);
        }

        sql += $"({string.Join(",", parameterNames)})";

        List<SqlParameter> sqlParameters = new();

        for (int i = 0; i < parameterNames.Count; ++i)
        {
            sqlParameters.Add(new SqlParameter(parameterNames[i], userIds[i]));
        }

        await using AsyncServiceScope serviceScope = _serviceScopeFactory.CreateAsyncScope();

        PasswordsTableOnlyAuthenticationDBContext dbContext = serviceScope.ServiceProvider.GetRequiredService<PasswordsTableOnlyAuthenticationDBContext>();

        IQueryable<PasswordsTableOnlyUser> data = dbContext.Database.SqlQueryRaw<PasswordsTableOnlyUser>(sql, sqlParameters.ToArray());

        return await data.AsNoTracking().ToListAsync();
    }

    public async Task<PasswordsTableOnlyUser?> GetUserAsync(string userId)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        return await userManager.FindByIdAsync(userId);
    }

    public async Task<IdentityResult> TryAddUserAsync(PasswordsTableOnlyUser user, string password)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        IdentityResult result = await userManager.CreateAsync(user, password);

        return result;
    }

    public async Task<IList<string>> GetUserRoleNamesAsync(PasswordsTableOnlyUser user)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        return await userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> TryAddRoleAsync(PasswordsTableOnlyRole role)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        RoleManager<PasswordsTableOnlyRole> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<PasswordsTableOnlyRole>>();

        IdentityResult result = await roleManager.CreateAsync(role);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        IdentityResult result = await userManager.AddToRoleAsync(user, roleName);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(PasswordsTableOnlyUser user, params string[] roleNames)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        IdentityResult result = await userManager.AddToRolesAsync(user, roleNames);

        return result;
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(PasswordsTableOnlyUser user, IEnumerable<string> roleNames)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        IdentityResult result = await userManager.AddToRolesAsync(user, roleNames);

        return result;
    }

    public async Task<bool> IsInRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        return await userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        PasswordsTableOnlyUser? user = await userManager.FindByIdAsync(userId);

        if (user is null) return new NotFound();

        string token = await userManager.GeneratePasswordResetTokenAsync(user);

        return await userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<IdentityResult> TryRemoveUserFromRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        IdentityResult result = await userManager.RemoveFromRoleAsync(user, roleName);

        return result;
    }

    public async Task<IdentityResult> TryRemoveUserFromRolesAsync(PasswordsTableOnlyUser user, IEnumerable<string> roleName)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        IdentityResult result = await userManager.RemoveFromRolesAsync(user, roleName);

        return result;
    }

    public async Task<bool> TryDeleteUserAsync(string userId)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        UserManager<PasswordsTableOnlyUser> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        PasswordsTableOnlyUser? user = await userManager.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user is null) return false;

        IdentityResult result = await userManager.DeleteAsync(user);

        return true;
    }
}