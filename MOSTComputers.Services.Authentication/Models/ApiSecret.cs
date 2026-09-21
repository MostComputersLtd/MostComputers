namespace MOSTComputers.Services.Authentication.Models;

public sealed class ApiSecret
{
    public required long Id { get; init; }
    public required int ClientId { get; init; }
    public required byte[] SecretHash { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? ExpiresAt { get; init; }
    public required DateTime? RevokedAt { get; init; }
    public required DateTime? LastUsedAt { get; init; }
}
