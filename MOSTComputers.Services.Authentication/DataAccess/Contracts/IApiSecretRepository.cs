using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecret;
using OneOf;

namespace MOSTComputers.Services.Authentication.DataAccess.Contracts;
internal interface IApiSecretRepository
{
    Task<bool> ClientHasActiveSecretAsync(int clientId, DateTime now);
    Task<ApiSecret?> GetByHashAsync(byte[] secretHash);
    Task<OneOf<int, DataAccessFailure>> InsertAsync(ApiSecretCreateRequest createRequest);
}
