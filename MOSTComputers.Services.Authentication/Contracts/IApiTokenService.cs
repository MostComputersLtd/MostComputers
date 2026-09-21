using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiToken;

namespace MOSTComputers.Services.Authentication.Contracts;

internal interface IApiTokenService
{
    Task<ApiToken?> GetByHashAsync(byte[] tokenHash);
    Task<bool> InsertAsync(ApiTokenCreateRequest createRequest);
}
