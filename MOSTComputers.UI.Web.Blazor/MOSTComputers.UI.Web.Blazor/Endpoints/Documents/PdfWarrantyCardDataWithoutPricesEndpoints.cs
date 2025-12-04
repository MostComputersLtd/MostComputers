using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Documents;

public static class PdfWarrantyCardDataWithoutPricesEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "warrantyCard/" + "pdf";

    public static IEndpointConventionBuilder MapPdfWarrantyCardDataWithoutPricesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{orderId}", GetWarrantyCardPdfFromOrderIdAsync).RequireAuthorization();

        return endpointGroup;
    }

    private static async Task<IResult> GetWarrantyCardPdfFromOrderIdAsync(
        [FromRoute] int orderId,
        [FromServices] IPdfWarrantyCardDataService pdfWarrantyCardDataService,
        [FromServices] IPdfWarrantyCardWithoutPricesFileGeneratorService pdfWarrantyCardWithoutPricesFileGeneratorService)
    {
        WarrantyCardWithoutPricesData? warrantyCardDataWithoutPrices
            = await pdfWarrantyCardDataService.GetWarrantyCardDataWithoutPricesByOrderIdAsync(orderId);

        if (warrantyCardDataWithoutPrices is null)
        {
            return Results.NotFound($"Warranty card with id {orderId} was not found.");
        }

        Stream fileStream = await pdfWarrantyCardWithoutPricesFileGeneratorService.CreateWarrantyCardPdfAndGetStreamAsync(warrantyCardDataWithoutPrices);

        string contentType = "application/pdf";

        return Results.File(fileStream, contentType);
    }
}