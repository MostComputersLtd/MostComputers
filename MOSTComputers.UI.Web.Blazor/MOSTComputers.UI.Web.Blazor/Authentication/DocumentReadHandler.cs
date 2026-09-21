using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.Models;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Authentication;

public sealed class DocumentReadRequirement : IAuthorizationRequirement
{
    public enum DocumentType
    {
        Invoice = 0,
        WarrantyCard = 1,
    }

    public required DocumentType Type { get; init; }
}

public sealed class DocumentReadHandler(IApiSecretAuthService apiSecretAuthService)
    : AuthorizationHandler<DocumentReadRequirement>
{
    private readonly IApiSecretAuthService _apiSecretAuthService = apiSecretAuthService;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, DocumentReadRequirement requirement)
    {
        ClaimsPrincipal claimsPrincipal = context.User;

        bool isLoggedUser = claimsPrincipal.Identities.Any(x => x.AuthenticationType == IdentityConstants.ApplicationScheme);

        if (!isLoggedUser)
        {
            ClaimsIdentity? remoteConnection = claimsPrincipal.Identities.FirstOrDefault(
                x => x.AuthenticationType == ApiAuthenticationScheme);

            if (remoteConnection == null)
            {
                context.Fail();

                return;
            }

            ApiSecretPermissions permissionToCheck = requirement.Type switch
            {
                DocumentReadRequirement.DocumentType.Invoice => ApiSecretPermissions.ReadInvoices,
                DocumentReadRequirement.DocumentType.WarrantyCard => ApiSecretPermissions.ReadWarrantyCards,
                _ => throw new NotSupportedException()
            };

            Claim apiSecretIdClaim = remoteConnection.Claims.First(x => x.Type == "ApiSecretId");

            long apiSecretId = long.Parse(apiSecretIdClaim.Value);

            bool hasPermission = await _apiSecretAuthService.SecretHasActivePermissionAsync(apiSecretId, permissionToCheck);

            if (!hasPermission)
            {
                context.Fail();

                return;
            }

            context.Succeed(requirement);

            return;
        }

        bool isAdminOrEmployee = claimsPrincipal.HasClaim(x =>
        {
            return x.Type == ClaimTypes.Role
                && (x.Value == "Admin" || x.Value == "Employee");
        });

        if (isAdminOrEmployee)
        {
            context.Succeed(requirement);

            return;
        }

        bool isCustomerInvoiceViewer = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "CustomerInvoiceViewer");

        if (!isCustomerInvoiceViewer)
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
