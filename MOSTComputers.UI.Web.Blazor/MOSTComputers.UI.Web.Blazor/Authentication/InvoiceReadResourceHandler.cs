using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.DataAccess.Documents.Models;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Authentication;

public sealed class InvoiceReadResourceRequirement : IAuthorizationRequirement
{
}

public sealed class InvoiceReadResourceHandler
    : AuthorizationHandler<InvoiceReadResourceRequirement, Invoice>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, InvoiceReadResourceRequirement requirement, Invoice resource)
    {
        return AuthorizeInvoiceOrWarrantyCard(context, requirement, resource.InvoiceDate, resource.CustomerBID);
    }

    public static Task AuthorizeInvoiceOrWarrantyCard<TRequirement>(
        AuthorizationHandlerContext context,
        TRequirement requirement,
        DateTime? documentDate,
        int? customerBID)
        where TRequirement : IAuthorizationRequirement
    {
        ClaimsPrincipal claimsPrincipal = context.User;

        bool isLoggedUser = claimsPrincipal.Identities.Any(x => x.AuthenticationType == IdentityConstants.ApplicationScheme);

        if (isLoggedUser)
        {
            bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");

            if (isAdmin)
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");

            DateTime minSearchDateTime = DateTime.Now.AddDays(-7);

            if (documentDate < minSearchDateTime)
            {
                context.Fail();

                return Task.CompletedTask;
            }

            if (isEmployee)
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(customerBIDAsString, out int customerBIDParsed)
                && customerBID == customerBIDParsed)
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            context.Fail();

            return Task.CompletedTask;
        }
        else
        {
            bool isRemoteConnection = claimsPrincipal.Identities.Any(x => x.AuthenticationType == ApiAuthenticationScheme);

            if (!isRemoteConnection)
            {
                context.Fail();

                return Task.CompletedTask;
            }

            DateTime minSearchDateTime = DateTime.Now.AddMonths(-3);

            if (documentDate < minSearchDateTime)
            {
                context.Fail();

                return Task.CompletedTask;
            }

            string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(customerBIDAsString, out int customerBIDParsed)
                && customerBID == customerBIDParsed)
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            context.Fail();

            return Task.CompletedTask;
        }
    }
}
