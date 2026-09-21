using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecret;
using OneOf;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.Authentication;

internal sealed class CachedApiSecretService(
    IApiSecretService apiSecretService,
    IFusionCache fusionCache) : IApiSecretService
{
    private const string _coreWord = "apiSecret";

    private static readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan _hasActiveExpirationTime = TimeSpan.FromMinutes(15);

    private readonly IApiSecretService _apiSecretService = apiSecretService;
    private readonly IFusionCache _fusionCache = fusionCache;

    public async Task<bool> ClientHasActiveSecretAsync(int clientId, DateTime now)
    {
        return await _fusionCache.GetOrSetAsync(GetHasActiveByClientIdKey(clientId),
            async (cancellationToken) => await _apiSecretService.ClientHasActiveSecretAsync(clientId, now),
            _hasActiveExpirationTime);
    }

    public async Task<ApiSecret?> GetByHashAsync(byte[] secretHash)
    {
        return await _fusionCache.GetOrSetAsync(GetByHashKey(secretHash),
            async (cancellationToken) => await _apiSecretService.GetByHashAsync(secretHash),
            _expirationTime);
    }

    public async Task<OneOf<int, DataAccessFailure>> InsertAsync(ApiSecretCreateRequest createRequest)
    {
        OneOf<int, DataAccessFailure> result = await _apiSecretService.InsertAsync(createRequest);

        if (!result.IsT0)
        {
            await _fusionCache.RemoveAsync(GetByHashKey(createRequest.SecretHash));
            await _fusionCache.RemoveAsync(GetHasActiveByClientIdKey(createRequest.ClientId));
        }

        return result;
    }

    private static string GetHasActiveByClientIdKey(int clientId)
    {
        return $"{_coreWord}:HasActive:ByUserId:{clientId}";
    }

    private static string GetByHashKey(byte[] secretHash)
    {
        string cacheKey = Convert.ToHexString(secretHash);

        return $"{_coreWord}:ByHash:{cacheKey}";
    }
}
