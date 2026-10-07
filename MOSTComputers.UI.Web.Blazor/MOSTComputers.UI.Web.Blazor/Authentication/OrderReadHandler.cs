using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Authentication;

public sealed class OrderReadRequirement : IAuthorizationRequirement
{
}

public sealed class OrderReadHandler
    : AuthorizationHandler<OrderReadRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, OrderReadRequirement requirement)
    {
        ClaimsPrincipal claimsPrincipal = context.User;

        bool isLoggedUser = claimsPrincipal.Identities.Any(x => x.AuthenticationType == IdentityConstants.ApplicationScheme);

        if (!isLoggedUser)
        {
            ClaimsIdentity? remoteConnection = claimsPrincipal.Identities.FirstOrDefault(
                x => x.AuthenticationType == TokenValidationParameters.DefaultAuthenticationType);

            if (remoteConnection == null)
            {
                context.Fail();

                return;
            }

            bool hasPermission = remoteConnection.HasScope(Scopes.ReadOrders);

            if (!hasPermission)
            {
                context.Fail();

                return;
            }

            context.Succeed(requirement);

            return;
        }

        bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
        bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");

        if (isAdmin || isEmployee)
        {
            context.Succeed(requirement);

            return;
        }

        bool isCustomer = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "CustomerInvoiceViewer");

        if (!isCustomer)
        {
            context.Fail();

            return;
        }

        string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (customerBIDAsString == null
            || !int.TryParse(customerBIDAsString, out int customerBIDParsed))
        {
            context.Fail();

            return;
        }

        context.Succeed(requirement);

        return;
    }
}
