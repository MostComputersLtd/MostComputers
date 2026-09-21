namespace MOSTComputers.Services.Authentication.Models.Requests.ApiToken;

public sealed class ApiTokenCreateRequest
{
    public required long SecretId { get; init; }
    public required int ClientId { get; init; }
    public required byte[] TokenHash { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
