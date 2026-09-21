using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.DataAccess.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiToken;

namespace MOSTComputers.Services.Authentication;

internal sealed class ApiTokenService(IApiTokenRepository apiTokenRepository) : IApiTokenService
{
    private readonly IApiTokenRepository _apiTokenRepository = apiTokenRepository;

    public Task<ApiToken?> GetByHashAsync(byte[] tokenHash)
    {
        return _apiTokenRepository.GetByHashAsync(tokenHash);
    }

    public Task<bool> InsertAsync(ApiTokenCreateRequest createRequest)
    {
        return _apiTokenRepository.InsertAsync(createRequest);
    }
}
