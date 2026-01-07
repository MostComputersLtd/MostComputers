using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models;
using OneOf.Types;
using OneOf;
using MOSTComputers.Services.Caching.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using MOSTComputers.Services.Identity.Confuguration;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.Identity.Services.Cached;
internal sealed class CachedPasswordsTableOnlyIdentityService : IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole>
{
    public CachedPasswordsTableOnlyIdentityService(
        [FromKeyedServices(Configuration.IdentityServiceKey)] IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> identityService,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _identityService = identityService;
        //_cache = cache;
        _fusionCache = fusionCache;
    }

    private readonly IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> _identityService;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    public async Task<List<PasswordsTableOnlyUser>> GetAllUsersAsync()
    {
        //return await _cache.GetOrAdd(_getAllKey, () => _identityService.GetAllUsersAsync());

        List<PasswordsTableOnlyUser> users = await _fusionCache.GetOrSetAsync(_getAllKey, (_) => _identityService.GetAllUsersAsync());

        return users.ToList();
    }

    public async Task<List<PasswordsTableOnlyUser>> GetUsersAsync(List<string> userIds)
    {
        //List<PasswordsTableOnlyUser>? cachedUsers = _cache.GetValueOrDefault<List<PasswordsTableOnlyUser>>(_getAllKey);
        MaybeValue<List<PasswordsTableOnlyUser>> cachedUsers = await _fusionCache.TryGetAsync<List<PasswordsTableOnlyUser>>(_getAllKey);

        if (cachedUsers.HasValue)
        {
            List<PasswordsTableOnlyUser> neededUsers = cachedUsers.Value.Where(x => userIds.Contains(x.Id))
                .ToList();

            foreach (PasswordsTableOnlyUser user in neededUsers)
            {
                string userKey = GetKeyById(user.Id);

                //_cache.AddOrUpdate(userKey, user);

                await _fusionCache.SetAsync(userKey, user);
            }

            return neededUsers;
        }

        userIds = userIds.Distinct().ToList();

        List<PasswordsTableOnlyUser>? individualCachedUsers = new();

        for (int i = 0; i < userIds.Count; i++)
        {
            string userId = userIds[i];

            string userKey = GetKeyById(userId);

            //PasswordsTableOnlyUser? cachedUser = _cache.GetValueOrDefault<PasswordsTableOnlyUser>(userKey);

            MaybeValue<PasswordsTableOnlyUser?> cachedUser = await _fusionCache.TryGetAsync<PasswordsTableOnlyUser?>(userKey);

            if (!cachedUser.HasValue || cachedUser.Value is null) continue;

            individualCachedUsers.Add(cachedUser.Value);

            userIds.RemoveAt(i);

            i--;
        }

        if (userIds.Count <= 0) return individualCachedUsers;

        List<PasswordsTableOnlyUser> notCachedUsers = await _identityService.GetUsersAsync(userIds);

        foreach (PasswordsTableOnlyUser user in notCachedUsers)
        {
            string userKey = GetKeyById(user.Id);

            //_cache.AddOrUpdate(userKey, user);

            await _fusionCache.SetAsync(userKey, user);
        }

        notCachedUsers.AddRange(individualCachedUsers);

        return notCachedUsers;
    }

    public async Task<PasswordsTableOnlyUser?> GetUserAsync(string userId)
    {
        string key = GetKeyById(userId);

        //return await _cache.GetOrAddAsync(key, () =>
        //{
        //    List<PasswordsTableOnlyUser>? cachedUsers = _cache.GetValueOrDefault<List<PasswordsTableOnlyUser>>(_getAllKey);

        //    if (cachedUsers is not null)
        //    {
        //        PasswordsTableOnlyUser? cachedUserInAll = cachedUsers.FirstOrDefault(x => x.Id == userId);

        //        return cachedUserInAll;
        //    }

        //    return _identityService.GetUser(userId);
        //});

        return await _fusionCache.GetOrSetAsync(key, async (cancellationToken) =>
        {
            MaybeValue<List<PasswordsTableOnlyUser>> cachedUsers = await _fusionCache.TryGetAsync<List<PasswordsTableOnlyUser>>(_getAllKey, token: cancellationToken);

            if (cachedUsers.HasValue)
            {
                PasswordsTableOnlyUser? cachedUserInAll = cachedUsers.Value.FirstOrDefault(x => x.Id == userId);

                return cachedUserInAll;
            }

            return await _identityService.GetUserAsync(userId);
        });
    }

    public async Task<IdentityResult> TryAddUserAsync(PasswordsTableOnlyUser user, string password)
    {
        IdentityResult addUserResult = await _identityService.TryAddUserAsync(user, password);

        if (addUserResult.Succeeded)
        {
            //_cache.Evict(_getAllKey);
            //_cache.Evict(GetKeyById(user.Id));

            await _fusionCache.RemoveAsync(_getAllKey);
            await _fusionCache.RemoveAsync(GetKeyById(user.Id));
        }

        return addUserResult;
    }

    public async Task<IList<string>> GetUserRoleNamesAsync(PasswordsTableOnlyUser user)
    {
        return await _identityService.GetUserRoleNamesAsync(user);
    }

    public async Task<IdentityResult> TryAddRoleAsync(PasswordsTableOnlyRole role)
    {
        return await _identityService.TryAddRoleAsync(role);
    }

    public async Task<IdentityResult> TryAddUserToRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        return await _identityService.TryAddUserToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(PasswordsTableOnlyUser user, params string[] roleNames)
    {
        return await _identityService.TryAddUserToRolesAsync(user, roleNames);
    }

    public async Task<IdentityResult> TryAddUserToRolesAsync(PasswordsTableOnlyUser user, IEnumerable<string> roleNames)
    {
        return await _identityService.TryAddUserToRolesAsync(user, roleNames);
    }

    public async Task<bool> IsInRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        return await _identityService.IsInRoleAsync(user, roleName);
    }

    public async Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword)
    {
        OneOf<IdentityResult, NotFound> changePasswordResult = await _identityService.ChangePasswordForUserAsync(userId, newPassword);

        if (changePasswordResult.IsT0
            && changePasswordResult.AsT0.Succeeded)
        {
            //_cache.Evict(_getAllKey);
            //_cache.Evict(GetKeyById(userId));

            await _fusionCache.RemoveAsync(_getAllKey);
            await _fusionCache.RemoveAsync(GetKeyById(userId));
        }

        return changePasswordResult;
    }

    public async Task<IdentityResult> TryRemoveUserFromRoleAsync(PasswordsTableOnlyUser user, string roleName)
    {
        return await _identityService.TryRemoveUserFromRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> TryRemoveUserFromRolesAsync(PasswordsTableOnlyUser user, IEnumerable<string> roleNames)
    {
        return await _identityService.TryRemoveUserFromRolesAsync(user, roleNames);
    }

    public async Task<bool> TryDeleteUserAsync(string userId)
    {
        bool success = await _identityService.TryDeleteUserAsync(userId);

        if (success)
        {
            //_cache.Evict(_getAllKey);
            //_cache.Evict(GetKeyById(userId));

            await _fusionCache.RemoveAsync(_getAllKey);
            await _fusionCache.RemoveAsync(GetKeyById(userId));
        }

        return success;
    }

    private const string _userKeysPrefix = "users";
    private const string _getAllKey = _userKeysPrefix;

    private static string GetKeyById(string userId)
    {
        return $"{_userKeysPrefix}/{userId}";
    }
}