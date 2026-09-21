using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecretPermission;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.Authentication.Contracts;

internal interface IApiSecretPermissionService
{
    Task<bool> SecretHasActivePermissionAsync(long secretId, ApiSecretPermissions permission);
    Task<OneOf<Success, DataAccessFailure>> InsertManyAsync(ApiSecretPermissionCreateManyRequest createRequest);
}
