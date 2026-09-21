using MOSTComputers.Services.Authentication.Models;
using OneOf;

namespace MOSTComputers.Services.Authentication.Contracts;

public interface IApiSecretAuthService
{
    Task<bool> SecretHasActivePermissionAsync(long secretId, ApiSecretPermissions permission);
    Task<OneOf<string, SecretForUserAlreadyExistsResult, DataAccessFailure>> CreateSecretForClientAsync(int clientId);
}
