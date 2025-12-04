using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

internal static class WarrantyCardXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "warrantyCard/" + "xml";

    public static IEndpointConventionBuilder MapWarrantyCardXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/id={warrantyCardOrderId:int}", GetXmlForWarrantyCardAsync).RequireAuthorization();
        endpointGroup.MapPost("/", GetXmlForWarrantyCardsAsync).RequireAuthorization();

        return endpointGroup;
    }

    private static async Task<IResult> GetXmlForWarrantyCardAsync(
        [FromRoute] int warrantyCardOrderId,
        HttpContext httpContext,
        [FromServices] IWarrantyCardRepository warrantyCardRepository,
        [FromServices] IWarrantyCardToXmlService warrantyCardToXmlService)
    {
        WarrantyCard? warrantyCard = await warrantyCardRepository.GetWarrantyCardByOrderIdAsync(warrantyCardOrderId);

        List<WarrantyCard> warrantyCardsInXml = warrantyCard is not null ? [warrantyCard] : [];

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await warrantyCardToXmlService.GetXmlForWarrantyCardsAsync(httpContext.Response.Body, warrantyCardsInXml);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForWarrantyCardsAsync(
        [FromBody] List<int> warrantyCardOrderIds,
        HttpContext httpContext,
        [FromServices] IWarrantyCardRepository warrantyCardRepository,
        [FromServices] IWarrantyCardToXmlService warrantyCardToXmlService)
    {
        List<WarrantyCard> warrantyCards = await warrantyCardRepository.GetWarrantyCardByOrderIdsAsync(warrantyCardOrderIds);

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await warrantyCardToXmlService.GetXmlForWarrantyCardsAsync(httpContext.Response.Body, warrantyCards);

        return Results.Empty;
    }
}