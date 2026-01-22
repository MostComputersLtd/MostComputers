using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

internal static class InvoiceXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "invoice/" + "xml";

    public static IEndpointConventionBuilder MapInvoiceXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/id={invoiceId:int}", GetXmlForInvoiceAsync).RequireAuthorization();
        endpointGroup.MapPost("/", GetXmlForInvoicesAsync).RequireAuthorization();

        return endpointGroup;
    }

    private static async Task<IResult> GetXmlForInvoiceAsync(
        [FromRoute] int invoiceId,
        HttpContext httpContext,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        Invoice? invoice = await invoiceRepository.GetInvoiceByIdAsync(invoiceId);

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
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IInvoiceToXmlService invoiceToXmlService,
        [FromServices] IFirmDataRepository firmDataRepository)
    {
        invoiceIds = invoiceIds.Distinct().Where(x => x > 0).ToList();

        List<Invoice> invoices = await invoiceRepository.GetInvoicesByIdsAsync(invoiceIds);

        List<FirmData> firmDatas = await firmDataRepository.GetAllAsync();

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await invoiceToXmlService.GetXmlForInvoicesAsync(httpContext.Response.Body, invoices);

        return Results.Empty;
    }
}