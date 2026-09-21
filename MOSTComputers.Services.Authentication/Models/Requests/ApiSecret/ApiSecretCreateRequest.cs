namespace MOSTComputers.Services.Authentication.Models.Requests.ApiSecret;

public sealed class ApiSecretCreateRequest
{
    public required int ClientId { get; init; }
    public required byte[] SecretHash { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? ExpiresAt { get; init; }
}
