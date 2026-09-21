using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecret;
using OneOf;

namespace MOSTComputers.Services.Authentication.Contracts;

internal interface IApiSecretService
{
    Task<bool> ClientHasActiveSecretAsync(int clientId, DateTime now);
    Task<ApiSecret?> GetByHashAsync(byte[] secretHash);
    Task<OneOf<int, DataAccessFailure>> InsertAsync(ApiSecretCreateRequest createRequest);
}
