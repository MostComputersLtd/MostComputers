using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.UI.Web.Blazor.Endpoints;

namespace MOSTComputers.UI.Web.Blazor.Components.Orders;

internal static class OrdersPageComponentEndpoints
{
	public sealed class OrdersSearchRequest
	{
		public int? ClientId { get; set; }
		public string? UserInputString { get; set; }
	}

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "orders";

    public static IEndpointConventionBuilder MapProductDataComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/search", GetSearchResultsAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetSearchResultsAsync(
        HttpContext httpContext,
        [FromBody] OrdersSearchRequest ordersSearchRequest,
        [FromServices] IOrdersService ordersService)
    {
		ClaimsPrincipal claimsPrincipal = httpContext.User;

		if (ordersSearchRequest.ClientId == null
				&& !claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin"))
		{
			return Results.NotFound();
		}

        List<ProductDisplayData> productDisplayDatas = await SearchProductsAsync(
            productService, productSearchService, ordersSearchRequest);

        httpContext.Response.ContentType = "application/html";

        return new RazorComponentResult<OrdersSearchResult>(new
        {
            ProductDisplayDatas = productDisplayDatas,
            ordersSearchRequest.Currency,
            SecondaryCurrency = (Currency?)(ordersSearchRequest.Currency == Currency.EUR ? Currency.BGN : null),
            ShowImages = false
        });
    }
}
