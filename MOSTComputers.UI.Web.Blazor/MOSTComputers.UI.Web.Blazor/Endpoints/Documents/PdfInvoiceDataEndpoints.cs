using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
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
            .RequireAuthorization(Policies.ReadInvoices)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadInvoices]))
            .WithTags("Invoice PDF")
            .WithName("GetInvoicePdfFromInvoiceNumber")
            .WithSummary("Returns an invoice PDF")
            .WithDescription("Returns the PDF document for the specified invoice number.")
            .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
            {
                OpenApiResponses currentResponses = operation.Responses ?? new();

                operation.Responses = new()
                {
                    ["200"] = new OpenApiResponse()
                    {
                        Description = "The PDF document for the specified invoice.",
                        Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            ["image/*"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                    Format = "binary"
                                }
                            }
                        }
                    }
                };

                foreach (KeyValuePair<string, IOpenApiResponse> kvp in currentResponses)
                {
                    operation.Responses.Add(kvp.Key, kvp.Value);
                }

                return Task.CompletedTask;
            });

        return endpointGroup;
    }

    [ProducesResponseType(400, Description = "The invoice number is missing or invalid.")]
    [ProducesResponseType(403, Description = "The authenticated user is not authorized to access the invoice.")]
    [ProducesResponseType(404, Description = "The specified invoice does not exist.")]
    private static async Task<IResult> GetInvoicePdfFromInvoiceNumberAsync(
        HttpContext httpContext,
        [FromRoute(Name = "invoiceNumber")]
        [Description("The invoice number.")]
        string invoiceNumber,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IInvoiceRepository invoiceRepository,
        [FromServices] IPdfInvoiceDataService pdfInvoiceDataService,
        [FromServices] IPdfInvoiceFileGeneratorService pdfInvoiceFileGeneratorService)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            return Results.BadRequest("The invoice number cannot be null or empty.");
        }

        int? invoiceNumberParsed = invoiceRepository.GetInvoiceNumberWithoutPrefix(invoiceNumber);

        if (invoiceNumberParsed == null)
        {
            return Results.BadRequest("The invoice number is invalid.");
        }

        Invoice? invoice = await invoiceRepository.GetInvoiceByNumberWithoutPrefixAsync(invoiceNumberParsed.Value);

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
