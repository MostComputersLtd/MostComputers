using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecretPermission;

namespace MOSTComputers.Services.Authentication.DataAccess.Contracts;

internal interface IApiSecretPermissionRepository
{
    Task<bool> SecretHasActivePermissionAsync(long secretId, ApiSecretPermissions permission);
    Task<bool> InsertManyAsync(ApiSecretPermissionCreateManyRequest createRequest);
}
