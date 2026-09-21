using Microsoft.AspNetCore.Authorization;
using MOSTComputers.Services.DataAccess.Documents.Models;

namespace MOSTComputers.UI.Web.Blazor.Authentication;

public sealed class WarrantyCardReadResourceRequirement : IAuthorizationRequirement
{
}

public sealed class WarrantyCardReadResourceHandler
    : AuthorizationHandler<WarrantyCardReadResourceRequirement, WarrantyCard>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, WarrantyCardReadResourceRequirement requirement, WarrantyCard resource)
    {
        return InvoiceReadResourceHandler.AuthorizeInvoiceOrWarrantyCard(
            context, requirement, resource.WarrantyCardDate, resource.CustomerBID);
    }
}
