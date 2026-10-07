using System.ComponentModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
using MOSTComputers.UI.Web.Blazor.Utils;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

internal static class InvoiceXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "invoice/" + "xml";

    public static IEndpointConventionBuilder MapInvoiceXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/id={invoiceId:int}", GetXmlForInvoiceAsync)
            .RequireAuthorization(Policies.ReadInvoices)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([AuthenticationUtils.Scopes.ReadInvoices]))
            .WithTags("Invoice XML")
            .WithName("GetXmlForInvoice")
            .WithSummary("Returns XML data for an invoice")
            .WithDescription("Returns the XML data for the specified invoice.");

        endpointGroup.MapGet("/number={invoiceNumber:int}", GetXmlForInvoiceByNumberAsync)
            .RequireAuthorization(Policies.ReadInvoices)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([AuthenticationUtils.Scopes.ReadInvoices]))
            .WithTags("Invoice XML")
            .WithName("GetXmlForInvoiceByNumber")
            .WithSummary("Returns XML data for an invoice by number")
            .WithDescription("Returns the XML data for the invoice with the specified invoice number.");

        endpointGroup.MapGet("/recent", GetXmlForRecentInvoicesAsync)
            .RequireAuthorization(Policies.ReadInvoices)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([AuthenticationUtils.Scopes.ReadInvoices]))
            .WithTags("Invoice XML")
            .WithName("GetXmlForRecentInvoices")
            .WithSummary("Returns XML data for recent invoices")
            .WithDescription("Returns the XML data for recent invoices.");

        endpointGroup.MapPost("/", GetXmlForInvoicesAsync)
            .RequireAuthorization(Policies.ReadInvoices)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([AuthenticationUtils.Scopes.ReadInvoices]))
            .WithTags("Invoice XML")
            .WithName("GetXmlForInvoices")
            .WithSummary("Returns XML data for invoices")
            .WithDescription("Returns the XML data for invoices matching the specified ids.")
            .Accepts<int[]>("application/json");

        return endpointGroup; 
    }

    [ProducesResponseType<InvoiceXmlFullData>(200, "application/xml", Description = "The XML data for the specified invoice.")]
    [ProducesResponseType(403, Description = "The authenticated user is not authorized to access the invoice.")]
    [ProducesResponseType(404, Description = "The specified invoice does not exist.")]
    private static async Task<IResult> GetXmlForInvoiceAsync(
        [FromRoute(Name = "invoiceId")]
        [Description("The ID of the invoice.")]
        int invoiceId,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        Invoice? invoice = await invoiceRepository.GetInvoiceByIdAsync(invoiceId);

        if (invoice == null) return Results.NotFound();

        Console.WriteLine($"INVOICE ITEM COUNT FROM REPO: {invoice.InvoiceItems?.Count ?? 0}");

        return await GetXmlForInvoiceInternalAsync(httpContext, authorizationService, invoiceToXmlService, firmDataRepository, invoice);
    }

    [ProducesResponseType<InvoiceXmlFullData>(200, "application/xml", Description = "The XML data for the specified invoice.")]
    [ProducesResponseType(403, Description = "The authenticated user is not authorized to access the invoice.")]
    [ProducesResponseType(404, Description = "The specified invoice does not exist.")]
    private static async Task<IResult> GetXmlForInvoiceByNumberAsync(
        [FromRoute(Name = "invoiceNumber")]
        [Description("The Number of the invoice.")]
        int invoiceNumber,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        Invoice? invoice = await invoiceRepository.GetInvoiceByNumberWithoutPrefixAsync(invoiceNumber);

        if (invoice == null) return Results.NotFound();

        return await GetXmlForInvoiceInternalAsync(httpContext, authorizationService, invoiceToXmlService, firmDataRepository, invoice);
    }

    private static async Task<IResult> GetXmlForInvoiceInternalAsync(
        HttpContext httpContext,
        IAuthorizationService authorizationService,
        IInvoiceToXmlService invoiceToXmlService,
        IFirmDataRepository firmDataRepository,
        Invoice? invoice)
    {
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

    [ProducesResponseType<InvoiceXmlFullData>(200, "application/xml",
        Description = "The XML data for the recent invoices.")]
    [ProducesResponseType(400, Description = "The start time is missing or is more than one week in the past.")]
    [ProducesResponseType(403,
        Description = "The authenticated user is not authorized to access one or more invoices.")]
    [ProducesResponseType(404, Description = "No invoices exist in the specified time period.")]
    private static async Task<IResult> GetXmlForRecentInvoicesAsync(
        [FromQuery(Name = "startTime")]
        [Description("The start date and time from which to retrieve recent invoices.")]
        DateTime? startTime,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        DateTime maxStartTime = DateTime.Today.AddDays(-7);

        if (startTime == null || startTime < maxStartTime)
        {
            return Results.BadRequest("Provide a valid start time");
        }

        int? clientId = GetClientIdFromRequest(httpContext.User);

        InvoiceSearchRequest invoiceSearchRequest = new()
        {
            InvoiceByDateFilterRequests = [
                new()
                {
                    SearchOption = InvoiceByDateSearchOptions.ByInvoiceDate,
                    FromDate = startTime.Value,
                }
            ],
        };

        if (clientId != null)
        {
            invoiceSearchRequest.InvoiceByIdSearchRequests = [
                new()
                {
                    SearchOption = InvoiceByIdSearchOptions.ByCustomerBID,
                    Id = clientId.Value,
                }
            ];
        }

        List<Invoice> invoices = await invoiceRepository.GetAllMatchingAsync(invoiceSearchRequest);

        return await GetXmlForInvoicesInternalAsync(httpContext, authorizationService, invoiceToXmlService, firmDataRepository, invoices);
    }

    internal static int? GetClientIdFromRequest(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsIdentity? loggedInUserIdentity = claimsPrincipal.Identities.FirstOrDefault(
            x => x.AuthenticationType == IdentityConstants.ApplicationScheme);

        if (loggedInUserIdentity != null)
        {
            bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
            bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");

            if (isAdmin || isEmployee) return null;

            string customerBIDFromClaims = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value!;

            return int.Parse(customerBIDFromClaims);
        }

        string customerBIDFromToken = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == Claims.Subject)?.Value!;

        return int.Parse(customerBIDFromToken);
    }

    [ProducesResponseType<InvoiceXmlFullData>(200, "application/xml",
        Description = "The XML data for the specified invoices.")]
    [ProducesResponseType(403, Description = "The authenticated user is not authorized to access one or more invoices.")]
    [ProducesResponseType(404, Description = "None of the specified invoices exist.")]
    private static async Task<IResult> GetXmlForInvoicesAsync(
        [FromBody]
        [Description("The IDs of the invoices to retrieve.")]
        List<int> invoiceIds,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        invoiceIds = invoiceIds.Distinct().Where(x => x > 0).ToList();

        List<Invoice> invoices = await invoiceRepository.GetInvoicesByIdsAsync(invoiceIds);

        return await GetXmlForInvoicesInternalAsync(httpContext, authorizationService, invoiceToXmlService, firmDataRepository, invoices);
    }

    private static async Task<IResult> GetXmlForInvoicesInternalAsync(
        HttpContext httpContext,
        IAuthorizationService authorizationService,
        IInvoiceToXmlService invoiceToXmlService,
        IFirmDataRepository firmDataRepository,
        List<Invoice> invoices)
    {
        if (invoices.Count == 0) return Results.NotFound();

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

        await invoiceToXmlService.GetXmlForInvoicesAsync(httpContext.Response.Body, invoices, firmDatas);

        return Results.Empty;
    }
}
