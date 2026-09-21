using System.Security.Cryptography;
using System.Text;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Authentication.Models.Requests.ApiToken;
using OneOf;
using static MOSTComputers.Services.Authentication.CommonRules;

namespace MOSTComputers.Services.Authentication;

internal sealed class ApiTokenAuthService(
    IApiTokenService apiTokenService,
    IApiSecretService apiSecretService) : IApiTokenAuthService
{
    private const string _tokenCharacters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

    private static readonly TimeSpan _tokenExpiryWindow = TimeSpan.FromMinutes(30);

    private readonly IApiTokenService _apiTokenService = apiTokenService;
    private readonly IApiSecretService _apiSecretService = apiSecretService;

    public async Task<OneOf<TokenCreatedResult, SecretNotFoundResult, SecretNotUsableResult, DataAccessFailure>> CreateTokenForSecretAsync(
        string secret)
    {
        if (secret.Length != ApiSecretLength) return new SecretNotFoundResult();

        byte[] secretBytes = Encoding.UTF8.GetBytes(secret);

        byte[] secretHash = SHA256.HashData(secretBytes);

        ApiSecret? apiSecret = await _apiSecretService.GetByHashAsync(secretHash);

        if (apiSecret == null) return new SecretNotFoundResult();

        if (apiSecret.ExpiresAt != null && apiSecret.ExpiresAt <= DateTime.Now)
        {
            return SecretNotUsableResult.Expired;
        }

        if (apiSecret.RevokedAt != null)
        {
            return SecretNotUsableResult.Revoked;
        }

        string token = RandomNumberGenerator.GetString(_tokenCharacters, ApiTokenLength);

        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);

        byte[] tokenHash = SHA256.HashData(tokenBytes);

        DateTime createdAt = DateTime.Now;
        DateTime expiresAt = createdAt.Add(_tokenExpiryWindow);

        ApiTokenCreateRequest apiTokenCreateRequest = new()
        {
            ClientId = apiSecret.ClientId,
            SecretId = apiSecret.Id,
            TokenHash = tokenHash,
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
        };

        bool success = await _apiTokenService.InsertAsync(apiTokenCreateRequest);

        if (!success)
        {
            return new DataAccessFailure();
        }

        DateTime localCreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Local);
        DateTime localExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Local);

        TokenCreatedResult tokenCreatedResult = new()
        {
            Token = token,
            CreatedAt = new DateTimeOffset(localCreatedAt),
            ExpiresAt = new DateTimeOffset(localExpiresAt),
        };

        return tokenCreatedResult;
    }

    public async Task<OneOf<ApiToken, ApiTokenAuthFailure>> AuthenticateTokenAsync(string token)
    {
        if (token.Length != ApiTokenLength) return ApiTokenAuthFailure.NotFound;

        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);

        byte[] tokenHash = SHA256.HashData(tokenBytes);

        ApiToken? apiToken = await _apiTokenService.GetByHashAsync(tokenHash);

        if (apiToken == null) return ApiTokenAuthFailure.NotFound;

        if (apiToken.RevokedAt != null)
        {
            return ApiTokenAuthFailure.Revoked;
        }

        if (apiToken.ExpiresAt <= DateTime.Now)
        {
            return ApiTokenAuthFailure.Expired;
        }

        return apiToken;
    }
}
