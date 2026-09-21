using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiToken;

namespace MOSTComputers.Services.Authentication.DataAccess.Contracts;

internal interface IApiTokenRepository
{
    Task<ApiToken?> GetByHashAsync(byte[] tokenHash);
    Task<bool> InsertAsync(ApiTokenCreateRequest createRequest);
}
