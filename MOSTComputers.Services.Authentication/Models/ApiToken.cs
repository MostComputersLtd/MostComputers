namespace MOSTComputers.Services.Authentication.Models;

public sealed class ApiToken
{
    public required long Id { get; init; }
    public required long SecretId { get; init; }
    public required int ClientId { get; init; }
    public required byte[] TokenHash { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required DateTime? RevokedAt { get; init; }
    public required DateTime? LastUsedAt { get; init; }
}
