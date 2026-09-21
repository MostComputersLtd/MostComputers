namespace MOSTComputers.Services.Authentication.Models;

public sealed class TokenCreatedResult
{
    public required string Token { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
}
