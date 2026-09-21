using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.DataAccess.Documents.Models;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

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

        DateTime? searchStartDateTime = DateTime.Today.AddDays(-7);

        if (searchStartDateTime != null
            && resource.OrderDate < searchStartDateTime)
        {
            context.Fail();

            return Task.CompletedTask;
        }

        bool isLoggedUser = claimsPrincipal.Identities.Any(x => x.AuthenticationType == IdentityConstants.ApplicationScheme);

        if (!isLoggedUser)
        {
            bool isRemoteConnection = claimsPrincipal.Identities.Any(x => x.AuthenticationType == ApiAuthenticationScheme);

            if (!isRemoteConnection)
            {
                context.Fail();

                return Task.CompletedTask;
            }

            string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(customerBIDAsString, out int customerBIDParsed)
                && resource.BusinessId == customerBIDParsed)
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            context.Fail();

            return Task.CompletedTask;
        }

        bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
        bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");
        bool isCustomer = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "CustomerInvoiceViewer");

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
}
