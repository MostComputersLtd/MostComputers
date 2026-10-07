using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Identity.DAL.Contracts;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Identity.Models.Customers;
using MOSTComputers.UI.Web.Blazor.Utils;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.Server.AspNetCore;
using static MOSTComputers.UI.Web.Blazor.Utils.OpenApiUtils;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Authentication;

public static class ApiAuthenticationEndpoints
{
    internal sealed class ApiClientRequest
    {
        [Required]
        [Description("The name of the created client")]
        public string name { get; init; }

        [Required]
        [Description("The permitted scopes of the created client")]
        public string[] Scopes { get; init; } = [];
    }

    internal sealed record ApiClientResponse(

        [property: Description("The client identifier.")]
        string client_id,

        [property: Description("The client secret.")]
        string client_secret);

    internal sealed record TokenRequest(

        [property: Description("The OAuth 2.0 grant type. Must be 'client_credentials'.")]
        string grant_type,

        [property: Description("The client identifier.")]
        string client_id,

        [property: Description("The client secret.")]
        string client_secret,

        [property: Description("The space-separated scopes requested for the access token.")]
        string scope);

    internal sealed record TokenSuccessResponse(
        [property: Description("The OAuth 2.0 Access Token.")]
        string access_token,

        [property: Description("The OAuth 2.0 Token Type, usually 'Bearer'")]
        string token_type,

        [property: Description("The time from now in seconds, during which the token expires")]
        int expires_in
    );

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "auth/";

    private static readonly string[] _defaultApplicationPermissions = new[]
    {
        Permissions.Endpoints.Token,
        Permissions.GrantTypes.ClientCredentials,

        //Permissions.Prefixes.Scope + AuthenticationUtils.Scopes.ReadInvoices,
        //Permissions.Prefixes.Scope + AuthenticationUtils.Scopes.ReadWarrantyCards,
        //Permissions.Prefixes.Scope + AuthenticationUtils.Scopes.ReadOrders,
    };

    private const string _authorizationHeaderName = "Authorization";
    private const string _basicTokenPrefix = "Basic ";
    private const char _usernameAndPasswordSplitCharacter = ':';

    public static IEndpointConventionBuilder MapApiAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapPost("/register/secret", RegisterAsync)
            .DisableCookieRedirect()
            .AllowAnonymous()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithTags("Auth")
            .WithName("Register")
            .WithSummary("Registers an API client")
            .WithDescription("Registers a new API client for third-party authentication.")
            .Accepts<ApiClientRequest>("application/json")
            .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
            {
                IOpenApiSchema? schema = operation.RequestBody?.Content!["application/json"].Schema;

                schema!.Properties!["scopes"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Enum =
                        [
                            AuthenticationUtils.Scopes.ReadInvoices,
                            AuthenticationUtils.Scopes.ReadWarrantyCards,
                            AuthenticationUtils.Scopes.ReadOrders,
                        ]
                    },
                    MinItems = 1,
                    UniqueItems = true
                };

                operation.Security ??= [];

                OpenApiSecuritySchemeReference schemeReference = new(
                    SecuritySchemes.ApiClientRegisterSchemeName, context.Document);

                OpenApiSecurityRequirement securityRequirement = new()
                {
                    [schemeReference] = [],
                };

                operation.Security.Add(securityRequirement);

                return Task.CompletedTask;
            });

        endpointGroup.MapPost("/connect/token", ExchangeAsync)
            .DisableCookieRedirect()
            .AllowAnonymous()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithTags("Auth")
            .WithName("Exchange")
            .WithSummary("Exchanges credentials for an access token")
            .WithDescription("Exchanges client credentials for an access token.")
            .Accepts<TokenRequest>("application/x-www-form-urlencoded");

        return endpointGroup;
    }

    [ProducesResponseType<ApiClientResponse>(200, "application/json",
        Description = "The client identifier and secret for the newly registered API client.")]
    [ProducesResponseType(400, Description = "The request must use HTTPS.")]
    [ProducesResponseType(401, Description = "The supplied credentials are missing or invalid.")]
    [ProducesResponseType(500, Description = "Internal server error")]
    private static async Task<IResult> RegisterAsync(
        [FromHeader(Name = _authorizationHeaderName)]
        [Description("The Authorization header containing the credentials required to register an API client.")]
        string authorizationHeader,
        [FromBody]
        [Description("The client information to request")]
        ApiClientRequest request,
        HttpContext httpContext,
        [FromServices] ICustomAuthenticationService customAuthenticationService,
        [FromServices] ICustomersViewLoginDataRepository customersViewLoginDataRepository,
        [FromServices] OpenIddictApplicationManager<ApiApplication> openIddictApplicationManager)
    {
        if (!httpContext.Request.IsHttps) return Results.BadRequest("HTTPS is required");

        if (request == null)
        {
            return Results.BadRequest("Request body cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.name))
        {
            return Results.BadRequest("Name cannot be empty.");
        }

        if (request.Scopes.Length == 0)
        {
            return Results.BadRequest("There must be at least 1 scope selected.");
        }

        foreach (string scope in request.Scopes)
        {
            bool isValidScope = ValidateScopeIsPartOfExistingScopes(scope);

            if (!isValidScope) return Results.BadRequest($"{scope} is not a valid scope.");
        }

        if (string.IsNullOrEmpty(authorizationHeader)
            || !authorizationHeader.StartsWith(_basicTokenPrefix))
        {
            return Results.Unauthorized();
        }

        string userCredentialsData = authorizationHeader[_basicTokenPrefix.Length..];

        byte[] unencodedUserCredentialsBytes = Convert.FromBase64String(userCredentialsData);

        string userCredentials = Encoding.UTF8.GetString(unencodedUserCredentialsBytes);

        int userCredentialsSplitIndex = userCredentials.IndexOf(_usernameAndPasswordSplitCharacter);

        string username = userCredentials[..userCredentialsSplitIndex];
        string password = userCredentials[(userCredentialsSplitIndex + 1)..];

        CheckPasswordResult checkPasswordResult
            = await customAuthenticationService.CheckIfCustomerCredentialsExistAsync(username, password);

        if (checkPasswordResult != CheckPasswordResult.Success)
        {
            return Results.Unauthorized();
        };

        CustomerLoginData? userInCustomersView = await customersViewLoginDataRepository.GetLoginDataByUsernameAsync(username);

        if (userInCustomersView == null)
        {
            return Results.Problem(statusCode: 500);
        }

        string clientId = Guid.NewGuid().ToString("N");
        string clientSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        int userId = userInCustomersView.Id;

        List<string> allApplicationPermissions = new(_defaultApplicationPermissions);

        foreach (string scope in request.Scopes)
        {
            string scopePermission = Permissions.Prefixes.Scope + scope;

            allApplicationPermissions.Add(scopePermission);
        }
 
        ApiApplication apiApplication = new()
        {
            UserId = userId,
            ClientId = clientId,
            ClientType = ClientTypes.Confidential,
            DisplayName = request.name,
            Permissions = JsonSerializer.Serialize(allApplicationPermissions),
        };

        await openIddictApplicationManager.CreateAsync(apiApplication, clientSecret);

        ApiClientResponse response = new(clientId, clientSecret);

        return Results.Ok(response);
    }

    [ProducesResponseType<TokenSuccessResponse>(200, "application/json",
        Description = "The access token response.")]
    [ProducesResponseType(400, Description = "The request must use HTTPS.")]
    [ProducesResponseType(401, Description = "The client credentials are missing or invalid.")]
    private static async Task<IResult> ExchangeAsync(
        HttpContext httpContext,
        OpenIddictApplicationManager<ApiApplication> openIddictApplicationManager)
    {
        if (!httpContext.Request.IsHttps) return Results.BadRequest("HTTPS is required");

        OpenIddictRequest? request = httpContext.GetOpenIddictServerRequest();

        if (request == null || request.ClientId == null) return Results.Unauthorized();

        if (request.IsClientCredentialsGrantType())
        {
            object? application = await openIddictApplicationManager.FindByClientIdAsync(request.ClientId)
                ?? throw new InvalidOperationException("The application cannot be found.");

            ApiApplication apiApplication = (ApiApplication)application;

            ClaimsIdentity identity = new (
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name,
                Claims.Role);

            identity.SetClaim(Claims.Subject, apiApplication.UserId.ToString());
            identity.SetClaim(Claims.Name, await openIddictApplicationManager.GetDisplayNameAsync(apiApplication));

            identity.SetDestinations(static claim => [Destinations.AccessToken]);

            identity.SetScopes(request.GetScopes());

            ClaimsPrincipal claimsPrincipal = new(identity);

            return Results.SignIn(
                claimsPrincipal,
                authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return Results.Unauthorized();
    }

    private static bool ValidateScopeIsPartOfExistingScopes(string scope)
    {
        return AuthenticationUtils.Scopes.AllScopes.Contains(scope);
    }
}
