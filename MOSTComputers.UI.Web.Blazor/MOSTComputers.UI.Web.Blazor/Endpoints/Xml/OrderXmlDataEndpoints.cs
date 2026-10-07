using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using MOSTComputers.Models.Common;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Orders;
using MOSTComputers.Services.DataToXmlConversion.Models;
using MOSTComputers.Services.DataToXmlConversion.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.OrderData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.Orders.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints.Html;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using static MOSTComputers.UI.Web.Blazor.Endpoints.PromotionPictureSource;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

public static class OrderXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "documents/" + "order/" + "xml";

    public static IEndpointConventionBuilder MapOrderXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{orderId:int}", GetOrderXmlAsync)
            .RequireAuthorization(Policies.ReadOrders)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadOrders]))
            .WithTags("Order XML")
            .WithName("GetOrderXml")
            .WithSummary("Returns XML data for an order")
            .WithDescription("Returns the XML data for the specified order.");

        endpointGroup.MapGet("/{orderId:int}/prices", GetXmlPricesForOrderAsync)
            .RequireAuthorization(Policies.ReadOrders)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadOrders]))
            .WithTags("Order XML")
            .WithName("GetXmlPricesForOrder")
            .WithSummary("Returns XML price data for an order")
            .WithDescription("Returns the XML price data for the specified order.");

        endpointGroup.MapGet("/recent", GetXmlForRecentOrdersAsync)
            .RequireAuthorization(Policies.ReadOrders)
            .DisableCookieRedirect()
            .WithMetadata(new IncludeInOpenApiSpecMetadata())
            .WithMetadata(new OpenApiOAuthEndpointDescriptionMetadata([Scopes.ReadOrders]))
            .WithTags("Order XML")
            .WithName("GetXmlForRecentOrders")
            .WithSummary("Returns XML data for recent orders")
            .WithDescription("Returns the XML data for recent orders.");

        return endpointGroup;
    }

    [ProducesResponseType<XmlOrder>(200, "application/xml",
        Description = "The XML data for the specified order.")]
    [ProducesResponseType(403, Description = "The authenticated user is not authorized to access the order.")]
    [ProducesResponseType(404, Description = "The specified order does not exist.")]
    private static async Task<IResult> GetOrderXmlAsync(
        HttpContext httpContext,
        [FromRoute(Name = "orderId")]
        [Description("The ID of the order.")]
        int orderId,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IOrdersService ordersService,
        [FromServices] IOrderXmlService orderXmlService)
    {
        Order? order = await ordersService.GetByIdAsync(orderId);

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

        XmlOrder xmlOrder = MapToXmlOrder(order);

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await orderXmlService.TrySerializeXmlAsync(httpContext.Response.Body, xmlOrder);

        return Results.Empty;
    } 

    [ProducesResponseType<ProductsXmlFullData>(200, "application/xml",
        Description = "The XML price data for the specified order.")]
    [ProducesResponseType(403, Description = "The authenticated user is not authorized to access the order.")]
    [ProducesResponseType(404, Description = "The specified order does not exist.")]
    private static async Task<IResult> GetXmlPricesForOrderAsync(
        HttpContext httpContext,
        [FromRoute(Name = "orderId")]
        [Description("The ID of the order.")]
        int orderId,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IOrdersService ordersService,
        [FromServices] IProductService productService,
        [FromServices] IProductToXmlService productToXmlService)
    {
        Order? order = await ordersService.GetByIdAsync(orderId);

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

        List<int> orderItemProductIds = new();

        foreach (OrderItem orderItem in order.Items)
        {
            if (orderItem.ProductId == null) continue;

            orderItemProductIds.Add(orderItem.ProductId.Value);
        }

        List<Product> products = await productService.GetByIdsAsync(orderItemProductIds);

        List<Product> productsWithNewPrices = new();

        foreach (Product product in products)
        {
            OrderItem orderItem = order.Items.First(x => x.ProductId == product.Id);

            Product productWithNewPrice = new()
            {
                Id = product.Id,
                Name = product.Name,
                AdditionalWarrantyPrice = product.AdditionalWarrantyPrice,
                AdditionalWarrantyTermMonths = product.AdditionalWarrantyTermMonths,
                StandardWarrantyPrice = product.StandardWarrantyPrice,
                StandardWarrantyTermMonths = product.StandardWarrantyTermMonths,
                DisplayOrder = product.DisplayOrder,
                Status = product.Status,
                PlShow = product.PlShow,
                Price = orderItem.Price,
                Currency = product.Currency,
                RowGuid = product.RowGuid,
                PromotionPid = product.PromotionPid,
                PromotionRid = product.PromotionRid,
                PromotionPictureId = product.PromotionPictureId,
                PromotionExpireDate = product.PromotionExpireDate,
                AlertPictureId = product.AlertPictureId,
                AlertExpireDate = product.AlertExpireDate,
                PriceListDescription = product.PriceListDescription,
                PartNumber1 = product.PartNumber1,
                PartNumber2 = product.PartNumber2,
                SearchString = product.SearchString,
                CategoryId = product.CategoryId,
                Category = product.Category,
                ManufacturerId = product.ManufacturerId,
                Manufacturer = product.Manufacturer,
                SubCategoryId = product.SubCategoryId,
            };

            productsWithNewPrices.Add(productWithNewPrice);
        }

        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = order.Currency;

        ProductXmlOptions productXmlOptions = new()
        {
            ImageFilesBasePath = CombinePathsWithSeparator('/', baseUrl, ProductImageFileDataEndpoints.EndpointGroupRoute),
            GroupPromotionsBasePath = CombinePathsWithSeparator('/', baseUrl, GroupPromotionHtmlEndpoints.EndpointGroupRoute),
            PromotionGroupsBasePath = CombinePathsWithSeparator('/', baseUrl, "promotionGroupImages"),
            GetPromotionPictureSourceUrlById = id => CombinePathsWithSeparator(
                '/', baseUrl, GetPromotionPictureSource(id) ?? ""),
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productToXmlService.TryGetXmlForProductsAsync(
            httpContext.Response.Body, productsWithNewPrices, productXmlOptions);

        //await RecordXmlDownloadAsync(xmlDownloadsRepository, httpContext, _allProductsResourceType);

        return Results.Empty;
    }

    [ProducesResponseType<List<XmlOrder>>(200, "application/xml",
        Description = "The XML data for the recent orders.")]
    [ProducesResponseType(400, Description = "The start time is missing or is more than one week in the past.")]
    [ProducesResponseType(403,
        Description = "The authenticated user is not authorized to access one or more orders.")]
    [ProducesResponseType(404, Description = "No orders exist in the specified time period.")]
    private static async Task<IResult> GetXmlForRecentOrdersAsync(
        [FromQuery(Name = "startTime")]
        [Description("The start date and time from which to retrieve recent orders.")]
        DateTime? startTime,
        HttpContext httpContext,
        [FromServices] IAuthorizationService authorizationService,
        [FromServices] IOrdersService ordersService,
        [FromServices] IOrderXmlService orderXmlService)
    {
        DateTime maxStartTime = DateTime.Today.AddDays(-7);

        if (startTime == null || startTime < maxStartTime)
        {
            return Results.BadRequest("Provide a valid start time");
        }

        int? clientId = InvoiceXmlDataEndpoints.GetClientIdFromRequest(httpContext.User);

        OrderSearchRequest orderSearchRequest = new()
        {
            SearchStartDateTime = startTime,
            CustomerId = clientId,
        };

        List<Order> orders = await ordersService.GetAllMatchingAsync(orderSearchRequest);

        if (orders.Count == 0) return Results.NotFound();

        foreach (Order order in orders)
        {
            AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
                httpContext.User, order, Policies.ReadOrderResource);

            if (!authorizationResult.Succeeded)
            {
                return Results.Forbid();
            }
        }

        List<XmlOrder> xmlOrders = [];

        foreach (Order order in orders)
        {
            XmlOrder xmlOrder = MapToXmlOrder(order);

            xmlOrders.Add(xmlOrder);
        }

        OrderXmlFullData orderXmlFullData = new()
        {
            Orders = xmlOrders,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml"); 

        await orderXmlService.TrySerializeXmlAsync(httpContext.Response.Body, orderXmlFullData);

        return Results.Empty;
    }

    private static XmlOrder MapToXmlOrder(Order order)
    {
        XmlOrder xmlOrder = new()
        {
            Id = order.Id,
            Status = order.Status,
            QuoteId = order.QuoteId,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            OrderName = order.OrderName,
            BusinessId = order.BusinessId,
            DealId = order.DealId,
            Currency = order.Currency,
            Info = order.Info,
        };

        foreach (OrderItem orderItem in order.Items)
        {
            XmlOrderItem xmlOrderItem = new()
            {
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price,
                AdditionalWarranty = orderItem.AdditionalWarranty,
                PromotionPAmount = orderItem.PromotionPAmount,
                PromotionRAmount = orderItem.PromotionRAmount,
                ExternalInfo = orderItem.ExternalInfo,
            };

            xmlOrder.Items.Add(xmlOrderItem);
        }

        return xmlOrder;
    }
}
