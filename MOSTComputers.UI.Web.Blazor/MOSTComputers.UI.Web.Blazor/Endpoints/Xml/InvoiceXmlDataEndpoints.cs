using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

internal static class InvoiceXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "invoice/" + "xml";

    public static IEndpointConventionBuilder MapInvoiceXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/id={invoiceId:int}", GetXmlForInvoiceAsync)
            .RequireAuthorization(Policies.ReadInvoicesThatAllowApi)
            .DisableCookieRedirect();

        endpointGroup.MapPost("/", GetXmlForInvoicesAsync)
            .RequireAuthorization(Policies.ReadInvoicesThatAllowApi)
            .DisableCookieRedirect();

        return endpointGroup;
    }

    private static async Task<IResult> GetXmlForInvoiceAsync(
        [FromRoute] int invoiceId,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        Invoice? invoice = await invoiceRepository.GetInvoiceByIdAsync(invoiceId);

        if (invoice == null) return Results.NotFound();

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            httpContext.User, invoice, Policies.ReadInvoiceResource);

        if (!authorizationResult.Succeeded)
        {
            return Results.Forbid();
        }

        List<Invoice> invoicesInXml = invoice is not null ? [invoice] : [];

        FirmData? firmData = null;

        if (invoice?.FirmId is not null)
        {
            firmData = await firmDataRepository.GetByIdAsync(invoice.FirmId.Value);
        }

        List<FirmData>? firmDatas = firmData is not null ? [firmData] : null;

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await invoiceToXmlService.GetXmlForInvoicesAsync(httpContext.Response.Body, invoicesInXml, firmDatas);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForInvoicesAsync(
        [FromBody] List<int> invoiceIds,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        invoiceIds = invoiceIds.Distinct().Where(x => x > 0).ToList();

        List<Invoice> invoices = await invoiceRepository.GetInvoicesByIdsAsync(invoiceIds);

        foreach (Invoice invoice in invoices)
        {
            AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
                httpContext.User, invoice, Policies.ReadInvoiceResource);

            if (!authorizationResult.Succeeded)
            {
                return Results.Forbid();
            }
        }

        List<FirmData> firmDatas = await firmDataRepository.GetAllAsync();

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await invoiceToXmlService.GetXmlForInvoicesAsync(httpContext.Response.Body, invoices);

        return Results.Empty;
    }
}
