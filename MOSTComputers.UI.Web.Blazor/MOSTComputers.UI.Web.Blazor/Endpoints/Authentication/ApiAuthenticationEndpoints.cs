using System.Buffers.Text;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using MOSTComputers.Services.Identity.DAL.Contracts;
using MOSTComputers.Services.Identity.Models.Customers;
using OneOf;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Authentication;

public static class ApiAuthenticationEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "auth/";

    private const string _basicTokenPrefix = "Basic ";
    private const string _bearerTokenPrefix = "Bearer ";
    private const char _usernameAndPasswordSplitCharacter = ':';

    public static IEndpointConventionBuilder MapApiAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapPost("/secret", CreateSecretAsync)
            .DisableCookieRedirect()
            .AllowAnonymous();

        endpointGroup.MapPost("/token", CreateTokenAsync)
            .DisableCookieRedirect()
            .AllowAnonymous();

        return endpointGroup;
    }

    private static async Task<IResult> CreateSecretAsync(
        HttpContext httpContext,
        [FromServices] ICustomAuthenticationService customAuthenticationService,
        [FromServices] ICustomersViewLoginDataRepository customersViewLoginDataRepository,
        [FromServices] IApiSecretAuthService apiSecretAuthService)
    {
        if (!httpContext.Request.IsHttps) return Results.BadRequest("HTTPS is required");
        
        string? authorizationHeader = httpContext.Request.Headers[HeaderNames.Authorization];

        if (string.IsNullOrEmpty(authorizationHeader)
            || !authorizationHeader.StartsWith(_basicTokenPrefix))
        {
            return Results.Unauthorized();
        }

        string userCredentialsData = authorizationHeader[_basicTokenPrefix.Length..];

        Console.WriteLine($"authheader: [{userCredentialsData}]");

        byte[] unencodedUserCredentialsBytes = Convert.FromBase64String(userCredentialsData);

        string userCredentials = Encoding.UTF8.GetString(unencodedUserCredentialsBytes);

        int userCredentialsSplitIndex = userCredentials.IndexOf(_usernameAndPasswordSplitCharacter);

        string username = userCredentials[..userCredentialsSplitIndex];
        string password = userCredentials[(userCredentialsSplitIndex + 1)..];

        Console.WriteLine($"username: {username} password: {password}");

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

        int customerBIDParsed = userInCustomersView.Id;

        OneOf<string, SecretForUserAlreadyExistsResult, DataAccessFailure> createSecretResult
            = await apiSecretAuthService.CreateSecretForClientAsync(customerBIDParsed);

        return createSecretResult.Match(
            secret =>
            {
                var data = new {
                    secret = secret
                };

                return Results.Json(data, statusCode: StatusCodes.Status201Created);
            },
            secretForUserAlreadyExistsResult => Results.Conflict("An active API secret already exists."),
            dataAccessFailure => Results.Problem("An error has occured", statusCode: 500));
    }

    private static async Task<IResult> CreateTokenAsync(
        HttpContext httpContext,
        [FromServices] IApiTokenAuthService apiTokenAuthService) 
    {
        if (!httpContext.Request.IsHttps) return Results.BadRequest("HTTPS is required");

        string? authorizationHeader = httpContext.Request.Headers[HeaderNames.Authorization];

        if (string.IsNullOrEmpty(authorizationHeader)
            || !authorizationHeader.StartsWith(_bearerTokenPrefix))
        {
            return Results.Unauthorized();
        }

        string secret = authorizationHeader[_bearerTokenPrefix.Length..];

        OneOf<TokenCreatedResult, SecretNotFoundResult, SecretNotUsableResult, DataAccessFailure> result
            = await apiTokenAuthService.CreateTokenForSecretAsync(secret);

        return result.Match(
            tokenCreatedResult =>
            {
                var data = new
                {
                    token = tokenCreatedResult.Token,
                    createdAt = tokenCreatedResult.CreatedAt.ToString("o"),
                    expiresAt = tokenCreatedResult.ExpiresAt.ToString("o"),
                };

                return Results.Json(data, statusCode: 201);
            },
            secretNotFoundResult =>
            {
                httpContext.Response.Headers.Append("WWW-Authenticate", "Bearer");

                return Results.Unauthorized();
            },
            secretNotUsableResult =>
            {
                httpContext.Response.Headers.Append("WWW-Authenticate", "Bearer");

                return Results.Unauthorized();
            },
            dataAccessFailure => Results.Problem("An Error has occured", statusCode: 500));
    }
}
