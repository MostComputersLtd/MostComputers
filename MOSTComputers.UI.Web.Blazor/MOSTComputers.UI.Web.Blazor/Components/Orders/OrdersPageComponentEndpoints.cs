using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Orders;
using MOSTComputers.Services.Orders.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.Orders;

internal static class OrdersPageComponentEndpoints
{
    public sealed class OrdersSearchRequest
    {
        public int? ClientId { get; set; }
        public int? OrderStatus { get; set; }
        public string? UserInputString { get; set; }
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "orders";

    public static IEndpointConventionBuilder MapOrdersPageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/order/{orderId:int?}", GetOrderAsync)
            .RequireAuthorization(Policies.ReadOrderPageComponents)
            .DisableCookieRedirect();

        endpointGroup.MapPost("/search", GetSearchResultsAsync)
            .RequireAuthorization(Policies.ReadOrderPageComponents)
            .DisableCookieRedirect();

        return endpointGroup;
    }

    private static async Task<IResult> GetSearchResultsAsync(
        HttpContext httpContext,
        [FromBody] OrdersSearchRequest ordersSearchRequest,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IOrdersService ordersService,
        [FromServices] ICurrencyVATPercentageProvider currencyVATPercentageProvider,
        [FromServices] ICurrencyVATService currencyVATService)
    {
        ClaimsPrincipal claimsPrincipal = httpContext.User;

        bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
        bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");

        bool isAdminOrEmployee = isAdmin || isEmployee;

        int? clientId = null;

        DateTime? searchStartDateTime = DateTime.Today.AddDays(-14);

        if (isAdminOrEmployee)
        {
            if (!isAdmin && ordersSearchRequest.ClientId == null)
            {
                return Results.NotFound();
            }

            clientId = ordersSearchRequest.ClientId;
        }
        else if (claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "CustomerOrderViewer"))
        {
            string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (customerBIDAsString == null
                || !int.TryParse(customerBIDAsString, out int customerBIDParsed))
            {
                return Results.Forbid();
            }

            clientId = customerBIDParsed;
        }
        else
        {
            return Results.Forbid();
        }

        string? searchByNameString = ordersSearchRequest.UserInputString;

        if (string.IsNullOrWhiteSpace(searchByNameString))
        {
            searchByNameString = null;
        }

        bool isSearchTextANumber = int.TryParse(searchByNameString, out int idToSearch);

        OrderSearchResults.OrderDisplayData? orderById = null;

        if (isSearchTextANumber)
        {
            orderById = await GetSingleOrderDisplayDataIfAllowedAsync(httpContext, authorizationService, ordersService, idToSearch);
        }

        int? orderStatus = ordersSearchRequest.OrderStatus;

        if (orderStatus == -1)
        {
            orderStatus = null;
        }

        OrderSearchRequest orderSearchRequestInner = new()
        {
            SearchByNameString = searchByNameString,
            CustomerId = clientId,
            Status = orderStatus,
            SearchStartDateTime = searchStartDateTime,
        };

        List<Order> orders = await ordersService.GetAllMatchingAsync(orderSearchRequestInner);

        httpContext.Response.ContentType = "application/html";

        List<OrderSearchResults.OrderDisplayData> orderDatas = new();

        decimal vatPercentageFraction = currencyVATPercentageProvider.GetDefaultVATPercentage();

        IOrderedEnumerable<Order> ordersOrdered = orders.OrderByDescending(x => x.Id);

        foreach (Order order in ordersOrdered)
        {
            //decimal totalPrice = 0M;
            //decimal totalPriceWithVAT = 0M;

            //foreach (OrderItem orderItem in order.Items)
            //{
            //    decimal totalItemPrice = 0M;
            //    decimal totalItemPriceWithVAT = 0M;

            //    if (orderItem.Price is not null
            //        && orderItem.Quantity is not null)
            //    {
            //        totalItemPrice = orderItem.Price.Value * orderItem.Quantity.Value;

            //        decimal vatPrice = currencyVATService.CalculateVAT(
            //            orderItem.Price.Value, orderItem.Quantity.Value, vatPercentageFraction);

            //        totalItemPriceWithVAT = totalItemPrice + vatPrice;
            //    }

            //    totalPrice += totalItemPrice;

            //    totalPriceWithVAT += totalItemPriceWithVAT;
            //}

            OrderSearchResults.OrderDisplayData orderData = new()
            {
                Id = order.Id,
                OrderName = order.OrderName,
                OrderDate = order.OrderDate,
                OrderStatus = order.Status,
                //TotalPrice = totalPriceWithVAT,
                //TotalPriceCurrency = order.Currency ?? Currency.EUR,
            };

            orderDatas.Add(orderData);
        }

        if (orderById != null)
        {
            orderDatas.Insert(0, orderById);
        }

        return new RazorComponentResult<OrderSearchResults>(new
        {
            Orders = orderDatas,
        });
    }

    private static async Task<OrderSearchResults.OrderDisplayData?> GetSingleOrderDisplayDataIfAllowedAsync(
        HttpContext httpContext,
        IAuthorizationService authorizationService,
        IOrdersService ordersService,
        int idToSearch)
    {
        Order? order = await ordersService.GetByIdAsync(idToSearch);

        if (order == null)
        {
            return null;
        }

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            httpContext.User, order, Policies.ReadOrderResource);

        if (!authorizationResult.Succeeded)
        {
            return null;
        }

        return new()
        {
            Id = order.Id,
            OrderName = order.OrderName,
            OrderDate = order.OrderDate,
            OrderStatus = order.Status,
            //TotalPrice = totalPriceWithVAT,
            //TotalPriceCurrency = order.Currency ?? Currency.EUR,
        };
    }

    private static async Task<IResult> GetOrderAsync(
        HttpContext httpContext,
        [FromRoute] int? orderId,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IOrdersService ordersService,
        [FromServices] IProductService productService)
    {
        if (orderId == null)
        {
            return Results.NotFound();
        }

        Order? order = await ordersService.GetByIdAsync(orderId.Value);

        if (order == null)
        {
            return Results.NotFound();
        }

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            httpContext.User, order, Policies.ReadOrderResource);

        if (!authorizationResult.Succeeded)
        {
            return Results.Forbid();
        }

        OrderDisplay.OrderDisplayData orderDisplayData = new()
        {
            Id = order.Id,
            Status = order.Status,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            OrderName = order.OrderName,
            BusinessId = order.BusinessId,
            DealId = order.DealId,
            Currency = order.Currency,
            Info = order.Info,
            Items = new(),
        };

        List<int> orderItemProductIds = new();

        foreach (OrderItem orderItem in order.Items)
        {
            if (orderItem.ProductId == null) continue;

            orderItemProductIds.Add(orderItem.ProductId.Value);
        }

        List<MOSTComputers.Models.Product.Models.Product> products = await productService.GetByIdsAsync(orderItemProductIds);

        foreach (OrderItem orderItem in order.Items)
        {
            MOSTComputers.Models.Product.Models.Product? product
                = products.FirstOrDefault(x => x.Id == orderItem.ProductId);

            if (product == null)
            {
                return Results.StatusCode(500);
            }

            OrderDisplay.OrderItemDisplayData orderItemDisplayData = new()
            {
                ProductId = orderItem.ProductId,
                ProductName = product.Name,
                ProductStatus = product.Status,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price,
                AdditionalWarranty = orderItem.AdditionalWarranty,
                PromotionPAmount = orderItem.PromotionPAmount,
                PromotionRAmount = orderItem.PromotionRAmount,
                ExternalInfo = orderItem.ExternalInfo,
            };

            orderDisplayData.Items.Add(orderItemDisplayData);
        }

        return new RazorComponentResult<OrderDisplay>(new
        {
            Order = orderDisplayData
        });
    }
}
