using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Common;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataToXmlConversion.Models;
using MOSTComputers.Services.DataToXmlConversion.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.OrderData;
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
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "order/" + "xml";

    public static IEndpointConventionBuilder MapOrderXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/{orderId:int}", GetOrderXmlAsync)
            .RequireAuthorization(Policies.ReadOrdersThatAllowApi)
            .DisableCookieRedirect();

        endpointGroup.MapGet("/{orderId:int}/prices", GetXmlPricesForOrderAsync)
            .RequireAuthorization(Policies.ReadOrdersThatAllowApi)
            .DisableCookieRedirect();

        return endpointGroup;
    }

    private static async Task<IResult> GetOrderXmlAsync(
        HttpContext httpContext,
        [FromRoute] int orderId,
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

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await orderXmlService.TrySerializeXmlAsync(httpContext.Response.Body, xmlOrder);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlPricesForOrderAsync(
        HttpContext httpContext,
        [FromRoute] int orderId,
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

    // public static async Task<OneOf<Order, IResult>> GetOrderByIdIfUserIsAllowedAsync(
    //     IOrdersService ordersService,
    //     ClaimsPrincipal claimsPrincipal,
    //     int orderId)
    // {
    //     bool isAdmin = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Admin");
    //     bool isEmployee = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "Employee");
    //     bool isCustomer = claimsPrincipal.HasClaim(x => x.Type == ClaimTypes.Role && x.Value == "CustomerInvoiceViewer");

    //     bool isAdminOrEmployee = isAdmin || isEmployee;

    //     int? clientId = null;

    //     DateTime? searchStartDateTime = null;

    //     if (isAdminOrEmployee)
    //     {
    //         searchStartDateTime = DateTime.Today.AddDays(-7);
    //     }
    //     else if (isCustomer)
    //     {
    //         string? customerBIDAsString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

    //         if (customerBIDAsString == null)
    //         {
    //             return OneOf<Order, IResult>.FromT1(Results.StatusCode(500));
    //         }

    //         bool parseSuccess = int.TryParse(customerBIDAsString, out int customerBIDParsed);

    //         if (!parseSuccess)
    //         {
    //             return OneOf<Order, IResult>.FromT1(Results.BadRequest());
    //         }

    //         clientId = customerBIDParsed;

    //         searchStartDateTime = DateTime.Today.AddDays(-7);
    //     }
    //     else
    //     {
    //         return OneOf<Order, IResult>.FromT1(Results.Unauthorized());
    //     }

    //     Order? order = await ordersService.GetByIdAsync(orderId);

    //     if (order == null)
    //     {
    //         return OneOf<Order, IResult>.FromT1(Results.NotFound());
    //     }

    //     if (searchStartDateTime != null
    //         && order.OrderDate < searchStartDateTime)
    //     {
    //         return OneOf<Order, IResult>.FromT1(Results.NotFound());
    //     }

    //     if (!isAdmin && isCustomer && order.BusinessId != clientId)
    //     {
    //         return OneOf<Order, IResult>.FromT1(Results.NotFound());
    //     }

    //     return order;
    // }
}
