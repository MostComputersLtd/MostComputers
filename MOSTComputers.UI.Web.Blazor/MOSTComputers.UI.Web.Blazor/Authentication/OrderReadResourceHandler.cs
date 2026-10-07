using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MOSTComputers.Services.DataAccess.Documents.Models;
using OpenIddict.Validation.AspNetCore;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MOSTComputers.UI.Web.Blazor.Authentication;

public sealed class OrderReadResourceRequirement : IAuthorizationRequirement
{
}

public sealed class OrderReadResourceHandler
    : AuthorizationHandler<OrderReadResourceRequirement, Order>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, OrderReadResourceRequirement requirement, Order resource)
    {
        ClaimsPrincipal claimsPrincipal = context.User;

        DateTime searchStartDateTime = DateTime.Today.AddDays(-14);

        if (resource.OrderDate < searchStartDateTime)
        {
            context.Fail();

            return Task.CompletedTask;
        }

        bool isLoggedUser = claimsPrincipal.Identities.Any(x => x.AuthenticationType == IdentityConstants.ApplicationScheme);

        if (!isLoggedUser)
        {
            return HandleApiAuthentication(context, requirement, resource, claimsPrincipal);
        }

        bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
        bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");
        bool isCustomer = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "CustomerOrderViewer");

        bool isAdminOrEmployee = isAdmin || isEmployee;

        int? clientId = null;

        if (!isAdminOrEmployee)
        {
            if (isCustomer)
            {
                string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(customerBIDAsString, out int customerBIDParsed))
                {
                    context.Fail();

                    return Task.CompletedTask;
                }

                clientId = customerBIDParsed;
            }
            else
            {
                context.Fail();

                return Task.CompletedTask;
            }
        }

        if (!isAdmin && isCustomer && resource.BusinessId != clientId)
        {
            context.Fail();

            return Task.CompletedTask;
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private static Task HandleApiAuthentication(
        AuthorizationHandlerContext context,
        OrderReadResourceRequirement requirement,
        Order resource,
        ClaimsPrincipal claimsPrincipal)
    {
        ClaimsIdentity? remoteConnection = claimsPrincipal.Identities.FirstOrDefault(
            x => x.AuthenticationType == TokenValidationParameters.DefaultAuthenticationType);

        if (remoteConnection == null)
        {
            context.Fail();

            return Task.CompletedTask;
        }

        string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == Claims.Subject)?.Value;

        if (int.TryParse(customerBIDAsString, out int customerBIDParsed)
            && resource.BusinessId == customerBIDParsed)
        {
            context.Succeed(requirement);

            return Task.CompletedTask;
        }

        context.Fail();

        return Task.CompletedTask;
    }
}
