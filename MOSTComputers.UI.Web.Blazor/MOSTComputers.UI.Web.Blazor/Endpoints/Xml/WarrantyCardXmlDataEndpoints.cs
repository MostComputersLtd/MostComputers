using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

internal static class WarrantyCardXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "warrantyCard/" + "xml";

    public static IEndpointConventionBuilder MapWarrantyCardXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/id={warrantyCardOrderId:int}", GetXmlForWarrantyCardAsync)
            .RequireAuthorization(Policies.ReadWarrantyCardsThatAllowApi)
            .DisableCookieRedirect();

        endpointGroup.MapPost("/", GetXmlForWarrantyCardsAsync)
            .RequireAuthorization(Policies.ReadWarrantyCardsThatAllowApi)
            .DisableCookieRedirect();

        return endpointGroup;
    }

    private static async Task<IResult> GetXmlForWarrantyCardAsync(
        [FromRoute] int warrantyCardOrderId,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IWarrantyCardRepository warrantyCardRepository,
        [FromServices] IWarrantyCardToXmlService warrantyCardToXmlService)
    {
        WarrantyCard? warrantyCard = await warrantyCardRepository.GetWarrantyCardByOrderIdAsync(warrantyCardOrderId);

        if (warrantyCard == null) return Results.NotFound();

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            httpContext.User, warrantyCard, Policies.ReadWarrantyCardResource);

        if (!authorizationResult.Succeeded)
        {
            return Results.Forbid();
        }

        List<WarrantyCard> warrantyCardsInXml = warrantyCard is not null ? [warrantyCard] : [];

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await warrantyCardToXmlService.GetXmlForWarrantyCardsAsync(httpContext.Response.Body, warrantyCardsInXml);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForWarrantyCardsAsync(
        [FromBody] List<int> warrantyCardOrderIds,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IWarrantyCardRepository warrantyCardRepository,
        [FromServices] IWarrantyCardToXmlService warrantyCardToXmlService)
    {
        List<WarrantyCard> warrantyCards = await warrantyCardRepository.GetWarrantyCardByOrderIdsAsync(warrantyCardOrderIds);

        if (warrantyCards.Count == 0) return Results.NotFound();

        foreach (WarrantyCard warrantyCard in warrantyCards)
        {
            AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
                httpContext.User, warrantyCard, Policies.ReadWarrantyCardResource);

            if (!authorizationResult.Succeeded)
            {
                return Results.Forbid();
            }
        }

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await warrantyCardToXmlService.GetXmlForWarrantyCardsAsync(httpContext.Response.Body, warrantyCards);

        return Results.Empty;
    }
}
