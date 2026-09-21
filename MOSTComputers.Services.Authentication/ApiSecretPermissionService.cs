using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.DataAccess.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiSecretPermission;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.Authentication;

internal sealed class ApiSecretPermissionService(IApiSecretPermissionRepository apiSecretPermissionRepository) : IApiSecretPermissionService
{
    public Task<bool> SecretHasActivePermissionAsync(long secretId, ApiSecretPermissions permission)
    {
        return apiSecretPermissionRepository.SecretHasActivePermissionAsync(secretId, permission);
    }

    public async Task<OneOf<Success, DataAccessFailure>> InsertManyAsync(ApiSecretPermissionCreateManyRequest createRequest)
    {
        bool success = await apiSecretPermissionRepository.InsertManyAsync(createRequest);

        return success ? new Success() : new DataAccessFailure();
    }
}
