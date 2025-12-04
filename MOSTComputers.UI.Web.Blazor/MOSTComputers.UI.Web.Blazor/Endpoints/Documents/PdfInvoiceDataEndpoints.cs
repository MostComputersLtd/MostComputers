using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.PDF.Models.Invoices;
using MOSTComputers.Services.PDF.Services.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Documents;

public static class PdfInvoiceDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "invoice/" + "pdf";

    public static IEndpointConventionBuilder MapPdfInvoiceDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{invoiceNumber}", GetInvoicePdfFromInvoiceNumberAsync).RequireAuthorization(options => options.RequireRole("Admin"));

        return endpointGroup;
    }

    private static async Task<IResult> GetInvoicePdfFromInvoiceNumberAsync(
        [FromRoute] string invoiceNumber,
        [FromServices] IPdfInvoiceDataService pdfInvoiceDataService,
        [FromServices] IPdfInvoiceFileGeneratorService pdfInvoiceFileGeneratorService)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            return Results.BadRequest("The invoice number cannot be null or empty.");
        }

        InvoiceData? invoiceData = await pdfInvoiceDataService.GetInvoiceDataByNumberAsync(invoiceNumber);

        if (invoiceData is null)
        {
            return Results.NotFound($"Invoice with number {invoiceNumber} was not found.");
        }

        Stream fileStream = await pdfInvoiceFileGeneratorService.CreateInvoicePdfAndGetStreamAsync(invoiceData);

        string contentType = "application/pdf";

        return Results.File(fileStream, contentType);
    }
}