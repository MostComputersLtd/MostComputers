namespace MOSTComputers.Services.Authentication.Models.Requests.ApiSecretPermission;

public sealed class ApiSecretPermissionCreateManyRequest
{
    public required long SecretId { get; init; }
    public required ApiSecretPermissions[] Permissions { get; init; }
}
