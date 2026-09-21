using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        endpointGroup.MapGet("/{orderId}", GetWarrantyCardPdfFromOrderIdAsync)
            .RequireAuthorization(Policies.ReadWarrantyCardsThatAllowApi)
            .DisableCookieRedirect();

        return endpointGroup;
    }

    private static async Task<IResult> GetWarrantyCardPdfFromOrderIdAsync(
        HttpContext httpContext,
        [FromRoute] int orderId,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IWarrantyCardRepository warrantyCardRepository,
        [FromServices] IPdfWarrantyCardDataService pdfWarrantyCardDataService,
        [FromServices] IPdfWarrantyCardWithoutPricesFileGeneratorService pdfWarrantyCardWithoutPricesFileGeneratorService)
    {
        WarrantyCard? warrantyCard = await warrantyCardRepository.GetWarrantyCardByOrderIdAsync(orderId); 

        if (warrantyCard is null)
        {
            return Results.NotFound($"Warranty card with id {orderId} was not found.");
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
