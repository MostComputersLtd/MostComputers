using MOSTComputers.Services.Authentication.Models;
using OneOf;

namespace MOSTComputers.Services.Authentication.Contracts;

public interface IApiTokenAuthService
{
    Task<OneOf<ApiToken, ApiTokenAuthFailure>> AuthenticateTokenAsync(string token);
    Task<OneOf<TokenCreatedResult, SecretNotFoundResult, SecretNotUsableResult, DataAccessFailure>> CreateTokenForSecretAsync(string secret);
}
