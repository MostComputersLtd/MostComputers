using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using OneOf;

namespace MOSTComputers.Services.Authentication;

public sealed class ApiAuthenticationHandler : AuthenticationHandler<ApiAuthenticationSchemeOptions>
{
    public ApiAuthenticationHandler(
        IOptionsMonitor<ApiAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiTokenAuthService apiTokenAuthService) : base(options, logger, encoder, clock)
    {
        _apiTokenAuthService = apiTokenAuthService;
    }

    private const string _bearerTokenPrefix = "Bearer ";
    private const string _apiSecretIdClaim = "ApiSecretId";

    private readonly IApiTokenAuthService _apiTokenAuthService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authorizationHeader = Request.Headers[HeaderNames.Authorization];

        if (string.IsNullOrEmpty(authorizationHeader)
            || !authorizationHeader.StartsWith(_bearerTokenPrefix))
        {
            return AuthenticateResult.NoResult();
        }

        string token = authorizationHeader[_bearerTokenPrefix.Length..];

        OneOf<ApiToken, ApiTokenAuthFailure> authenticationResult
            = await _apiTokenAuthService.AuthenticateTokenAsync(token);

        return authenticationResult.Match(
            apiToken =>
            {
                Claim[] claims = [
                    new(ClaimTypes.NameIdentifier, apiToken.ClientId.ToString()),
                    new(_apiSecretIdClaim, apiToken.SecretId.ToString()),
                ];

                ClaimsIdentity claimsIdentity = new(claims, authenticationType: Scheme.Name);

                ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

                AuthenticationTicket authenticationTicket = new(claimsPrincipal, Scheme.Name);

                return AuthenticateResult.Success(authenticationTicket);
            },
            apiTokenAuthFailure =>
            {
                string failureMessage = apiTokenAuthFailure switch
                {
                    ApiTokenAuthFailure.NotFound => "Token Not Found",
                    ApiTokenAuthFailure.Expired => "Token Expired",
                    ApiTokenAuthFailure.Revoked => "Token Revoked",
                    _ => "Unknown Error"
                };

                return AuthenticateResult.Fail(failureMessage);
            });
    }
}
