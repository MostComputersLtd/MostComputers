using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Documents;

public static class PdfWarrantyCardDataWithoutPricesEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "warrantyCard/" + "pdf";

    public static IEndpointConventionBuilder MapPdfWarrantyCardDataWithoutPricesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{warrantyCardOrderId}", GetWarrantyCardPdfFromOrderIdAsync)
            .RequireAuthorization(Policies.ReadWarrantyCards)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadWarrantyCards]))
            .WithTags("Warranty Card PDF")
            .WithName("GetWarrantyCardPdfFromOrderId")
            .WithSummary("Returns a warranty card PDF")
            .WithDescription("Returns the PDF document for the specified order.")
            .AddOpenApiOperationTransformer((operation, context, cancellationToken) =>
            {
                OpenApiResponses currentResponses = operation.Responses ?? new();

                operation.Responses = new()
                {
                    ["200"] = new OpenApiResponse()
                    {
                        Description = "The PDF for the specified warranty card.",
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

    [ProducesResponseType(403,
        Description = "The authenticated user is not authorized to access the warranty card.")]
    [ProducesResponseType(404, Description = "The specified warranty card does not exist.")]
    private static async Task<IResult> GetWarrantyCardPdfFromOrderIdAsync(
        HttpContext httpContext,
        [FromRoute(Name = "warrantyCardOrderId")]
        [Description("The ID of the warranty card to retrieve.")]
        int warrantyCardOrderId,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IWarrantyCardRepository warrantyCardRepository,
        [FromServices] IPdfWarrantyCardDataService pdfWarrantyCardDataService,
        [FromServices] IPdfWarrantyCardWithoutPricesFileGeneratorService pdfWarrantyCardWithoutPricesFileGeneratorService)
    {
        WarrantyCard? warrantyCard = await warrantyCardRepository.GetWarrantyCardByOrderIdAsync(warrantyCardOrderId); 

        if (warrantyCard is null)
        {
            return Results.NotFound($"Warranty card with id {warrantyCardOrderId} was not found.");
        }

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            httpContext.User, warrantyCard, Policies.ReadWarrantyCardResource);

        WarrantyCardWithoutPricesData warrantyCardDataWithoutPrices
            = pdfWarrantyCardDataService.GetWarrantyCardDataWithoutPrices(warrantyCard);

        if (!authorizationResult.Succeeded)
        {
            return Results.Forbid();
        }

        Stream fileStream = await pdfWarrantyCardWithoutPricesFileGeneratorService.CreateWarrantyCardPdfAndGetStreamAsync(
            warrantyCardDataWithoutPrices);

        string contentType = "application/pdf";

        return Results.File(fileStream, contentType);
    }
}
