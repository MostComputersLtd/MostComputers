using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MOSTComputers.Services.DataAccess.Documents.Models;
using OpenIddict.Validation.AspNetCore;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static OpenIddict.Abstractions.OpenIddictConstants;

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
            DateTime minSearchDateTime = DateTime.Now.AddMonths(-3);

            if (documentDate < minSearchDateTime)
            {
                context.Fail();

                return Task.CompletedTask;
            }

            bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
            bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");  

            if (isAdmin || isEmployee)
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
            ClaimsIdentity? remoteConnection = claimsPrincipal.Identities.FirstOrDefault(
                x => x.AuthenticationType == TokenValidationParameters.DefaultAuthenticationType);

            if (remoteConnection == null)
            {
                context.Fail();

                return Task.CompletedTask;
            }

            DateTime minSearchDateTime = DateTime.Now.AddDays(-14);

            if (documentDate < minSearchDateTime)
            {
                context.Fail();

                return Task.CompletedTask;
            }

            string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == Claims.Subject)?.Value;

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
