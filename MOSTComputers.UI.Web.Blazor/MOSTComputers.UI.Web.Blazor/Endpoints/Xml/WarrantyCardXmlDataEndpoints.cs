using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.WarrantyCardData;
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
            .RequireAuthorization(Policies.ReadWarrantyCards)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadWarrantyCards]))
            .WithTags("Warranty Card XML")
            .WithName("GetXmlForWarrantyCard")
            .WithSummary("Returns XML data for a warranty card")
            .WithDescription("Returns the XML data for the specified warranty card.");

        endpointGroup.MapGet("/recent", GetXmlForRecentWarrantyCardsAsync)
            .RequireAuthorization(Policies.ReadWarrantyCards)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadWarrantyCards]))
            .WithTags("Warranty Card XML")
            .WithName("GetXmlForRecentWarrantyCards")
            .WithSummary("Returns XML data for recent warranty cards")
            .WithDescription("Returns the XML data for recent warranty cards.");

        endpointGroup.MapPost("/", GetXmlForWarrantyCardsAsync)
            .RequireAuthorization(Policies.ReadWarrantyCards)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadWarrantyCards]))
            .WithTags("Warranty Card XML")
            .WithName("GetXmlForWarrantyCards")
            .WithSummary("Returns XML data for warranty cards")
            .WithDescription("Returns the XML data for warranty cards matching the specified ids.")
            .Accepts<int[]>("application/json");

        return endpointGroup;
    }

    [ProducesResponseType<WarrantyCardXmlFullData>(200, "application/xml",
        Description = "The XML data for the specified warranty card.")]
    [ProducesResponseType(403, Description = "The authenticated user is not authorized to access the warranty card.")]
    [ProducesResponseType(404, Description = "The specified warranty card does not exist.")]
    private static async Task<IResult> GetXmlForWarrantyCardAsync(
        [FromRoute(Name = "warrantyCardOrderId")]
        [Description("The ID of the warranty card to retrieve.")]
        int warrantyCardOrderId,
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

    [ProducesResponseType<WarrantyCardXmlFullData>(200, "application/xml",
        Description = "The XML data for the recent warranty cards.")]
    [ProducesResponseType(400, Description = "The start time is missing or is more than one week in the past.")]
    [ProducesResponseType(403,
        Description = "The authenticated user is not authorized to access one or more warranty cards.")]
    [ProducesResponseType(404, Description = "No warranty cards exist in the specified time period.")]
    private static async Task<IResult> GetXmlForRecentWarrantyCardsAsync(
        [FromQuery(Name = "startTime")]
        [Description("The start date and time from which to retrieve recent warranty cards.")]
        DateTime? startTime,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IWarrantyCardRepository warrantyCardRepository,
        [FromServices] IWarrantyCardToXmlService warrantyCardToXmlService)
    {
        DateTime maxStartTime = DateTime.Today.AddDays(-7);

        if (startTime == null || startTime < maxStartTime)
        {
            return Results.BadRequest("Provide a valid start time");
        }

        int? clientId = InvoiceXmlDataEndpoints.GetClientIdFromRequest(httpContext.User);

        WarrantyCardSearchRequest warrantyCardSearchRequest = new()
        {
            WarrantyCardByDateFilterRequests = [
                new()
                {
                    SearchOption = WarrantyCardByDateSearchOptions.ByWarrantyCardDate,
                    FromDate = startTime,
                }
            ],
        };

        if (clientId != null)
        {
            warrantyCardSearchRequest.WarrantyCardByIdSearchRequests = [
                new()
                {
                    SearchOption = WarrantyCardByIdSearchOptions.ByCustomerBID,
                    Id = clientId.Value,
                }
            ];
        }

        List<WarrantyCard> warrantyCards = await warrantyCardRepository.GetAllMatchingAsync(warrantyCardSearchRequest);

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

    [ProducesResponseType<WarrantyCardXmlFullData>(200, "application/xml",
        Description = "The XML data for the specified warranty cards.")]
    [ProducesResponseType(403,
        Description = "The authenticated user is not authorized to access one or more warranty cards.")]
    [ProducesResponseType(404, Description = "None of the specified warranty cards exist.")]
    private static async Task<IResult> GetXmlForWarrantyCardsAsync(
        [FromBody]
        [Description("The IDs of the warranty cards to retrieve.")]
        List<int> warrantyCardOrderIds,
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
