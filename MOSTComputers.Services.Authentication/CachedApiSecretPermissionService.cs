using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecretPermission;
using OneOf;
using OneOf.Types;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.Authentication;

internal sealed class CachedApiSecretPermissionService(
    IApiSecretPermissionService apiSecretPermissionService,
    IFusionCache fusionCache) : IApiSecretPermissionService
{
    private const string _coreWord = "apiSecretPermission";

    private static readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(15);

    private readonly IApiSecretPermissionService _apiSecretPermissionService = apiSecretPermissionService;
    private readonly IFusionCache _fusionCache = fusionCache; 

    public async Task<bool> SecretHasActivePermissionAsync(long secretId, ApiSecretPermissions permission)
    {
        return await _fusionCache.GetOrSetAsync(GetHasBySecretIdAndPermissionKey(secretId, permission),
            async (cancellationToken) => await _apiSecretPermissionService.SecretHasActivePermissionAsync(secretId, permission),
            _expirationTime);
    }

    public async Task<OneOf<Success, DataAccessFailure>> InsertManyAsync(ApiSecretPermissionCreateManyRequest createRequest)
    {
        OneOf<Success, DataAccessFailure> result = await _apiSecretPermissionService.InsertManyAsync(createRequest);

        if (!result.IsT0)
        {
            foreach (ApiSecretPermissions permission in createRequest.Permissions)
            {
                string key = GetHasBySecretIdAndPermissionKey(createRequest.SecretId, permission);

                await _fusionCache.RemoveAsync(key);
            }
        }

        return result;
    }

    private static string GetHasBySecretIdAndPermissionKey(long secretId, ApiSecretPermissions permission)
    {
        int permissionValue = (int)permission;

        return GetHasBySecretIdAndPermissionKey(secretId, permissionValue);
    }

    private static string GetHasBySecretIdAndPermissionKey(long secretId, int permissionValue)
    {
        return $"{_coreWord}:Has:BySecretIdAndPermission:{secretId}:{permissionValue}";
    }
}
