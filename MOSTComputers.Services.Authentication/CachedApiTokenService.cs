using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiToken;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.Authentication;

internal sealed class CachedApiTokenService(
    IApiTokenService apiTokenService,
    IFusionCache fusionCache) : IApiTokenService
{
    private const string _coreWord = "apiToken";

    private static readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(15);

    private readonly IApiTokenService _apiTokenService = apiTokenService;
    private readonly IFusionCache _fusionCache = fusionCache;

    public async Task<ApiToken?> GetByHashAsync(byte[] tokenHash)
    {
        return await _fusionCache.GetOrSetAsync(GetByHashKey(tokenHash),
            async (cancellationToken) => await _apiTokenService.GetByHashAsync(tokenHash),
            _expirationTime);
    }

    public async Task<bool> InsertAsync(ApiTokenCreateRequest createRequest)
    {
        bool success = await _apiTokenService.InsertAsync(createRequest);

        if (success)
        {
            await _fusionCache.RemoveAsync(GetByHashKey(createRequest.TokenHash));
        }

        return success;
    }

    private static string GetByHashKey(byte[] tokenHash)
    {
        string cacheKey = Convert.ToHexString(tokenHash);

        return $"{_coreWord}:ByHash:{cacheKey}";
    }
}
