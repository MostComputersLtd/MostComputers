using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.PDF.Models.Invoices;
using MOSTComputers.Services.PDF.Services.Contracts;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Documents;

public static class PdfInvoiceDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "invoice/" + "pdf";

    public static IEndpointConventionBuilder MapPdfInvoiceDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{invoiceNumber}", GetInvoicePdfFromInvoiceNumberAsync)
            .RequireAuthorization(Policies.ReadInvoicesThatAllowApi)
            .DisableCookieRedirect();

        return endpointGroup;
    }

    private static async Task<IResult> GetInvoicePdfFromInvoiceNumberAsync(
        HttpContext httpContext,
        [FromRoute] string invoiceNumber,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IPdfInvoiceDataService pdfInvoiceDataService,
        [FromServices] IPdfInvoiceFileGeneratorService pdfInvoiceFileGeneratorService)
    {
        const char _invoiceNumberStartingChar1 = 'C';
        const char _invoiceNumberStartingChar2 = 'H';

        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            return Results.BadRequest("The invoice number cannot be null or empty.");
        }

        if (invoiceNumber.StartsWith(_invoiceNumberStartingChar1)
            || invoiceNumber.StartsWith(_invoiceNumberStartingChar2))
        {
            invoiceNumber = invoiceNumber[1..];
        }

        Invoice? invoice = await invoiceRepository.GetInvoiceByNumberAsync(invoiceNumber);

        if (invoice == null) return Results.NotFound();

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            httpContext.User, invoice, Policies.ReadInvoiceResource);

        if (!authorizationResult.Succeeded)
        {
            return Results.Forbid();
        }

        InvoiceData? invoiceData = await pdfInvoiceDataService.GetPdfInvoiceDataFromInvoiceAsync(invoice);

        if (invoiceData is null)
        {
            return Results.NotFound($"Invoice with number {invoiceNumber} was not found.");
        }

        Stream fileStream = await pdfInvoiceFileGeneratorService.CreateInvoicePdfAndGetStreamAsync(invoiceData);

        string contentType = "application/pdf";

        return Results.File(fileStream, contentType);
    }
}
