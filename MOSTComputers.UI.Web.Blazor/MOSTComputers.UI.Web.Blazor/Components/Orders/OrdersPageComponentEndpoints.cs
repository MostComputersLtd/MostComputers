using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Common;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.Orders.Services;
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
        [FromServices] IOrdersService ordersService,
        [FromServices] ICurrencyVATPercentageProvider currencyVATPercentageProvider,
        [FromServices] ICurrencyVATService currencyVATService)
    {
        ClaimsPrincipal claimsPrincipal = httpContext.User;

        if (ordersSearchRequest.ClientId == null
            && !claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin"))
        {
            return Results.NotFound();
        }

        if (ordersSearchRequest.ClientId == null)
        {
            return Results.NotFound();
        }

        List<Order> orders = await ordersService.GetAllForUserAsync(ordersSearchRequest.ClientId.Value);

        httpContext.Response.ContentType = "application/html";

        List<OrderSearchResults.OrderDisplayData> orderDatas = new();

        decimal vatPercentageFraction = currencyVATPercentageProvider.GetDefaultVATPercentage();

        foreach (Order order in orders)
        {
            decimal totalPrice = 0M;
            decimal totalPriceWithVAT = 0M;

            foreach (OrderItem orderItem in order.Items)
            {
                decimal totalItemPrice = 0M;
                decimal totalItemPriceWithVAT = 0M;

                if (orderItem.Price is not null
                    && orderItem.Quantity is not null)
                {
                    totalItemPrice = orderItem.Price.Value * orderItem.Quantity.Value;

                    decimal vatPrice = currencyVATService.CalculateVAT(
                        orderItem.Price.Value, orderItem.Quantity.Value, vatPercentageFraction);

                    totalItemPriceWithVAT = totalItemPrice + vatPrice;
                }

                totalPrice += totalItemPrice;

                totalPriceWithVAT += totalItemPriceWithVAT;
            }

            OrderSearchResults.OrderDisplayData orderData = new()
            {
                Id = order.Id,
                OrderName = order.OrderName,
                OrderDate = order.OrderDate,
                TotalPrice = totalPriceWithVAT,
                TotalPriceCurrency = order.Currency ?? Currency.EUR,
            };
        }

        return new RazorComponentResult<OrderSearchResults>(new
        {
            Orders = orderDatas,
        });
    }
}
