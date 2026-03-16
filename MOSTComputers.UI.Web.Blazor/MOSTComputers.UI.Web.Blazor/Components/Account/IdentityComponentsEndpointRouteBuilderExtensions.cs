using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.UI.Web.Blazor.Components.Account.Pages;
using MOSTComputers.UI.Web.Blazor.Components.Account.Pages.Manage;
using System.Security.Claims;
using System.Text.Json;

namespace MOSTComputers.UI.Web.Blazor.Components.Account;
internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        RouteGroupBuilder accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapGet("/force-logout", async (
            ClaimsPrincipal user,
            SignInManager<PasswordsTableOnlyUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return TypedResults.LocalRedirect($"~/Account/Login");
            });


        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            SignInManager<PasswordsTableOnlyUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        RouteGroupBuilder manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        return accountGroup;
    }
}