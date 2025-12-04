using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Blazor.Models.Search.Product;
using MOSTComputers.UI.Web.Blazor.Services.Search.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.Xml;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

public static class ProductXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "product/" + "xml";
    
    public static IEndpointConventionBuilder MapProductXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/all", GetXmlForAllProductsAsync);
        endpointGroup.MapGet("/promotions", GetXmlForAllProductsWithPromotionsAsync);
        endpointGroup.MapGet("/manufacturerId={manufacturerId:int}", GetXmlForAllProductsInManufacturerAsync);
        endpointGroup.MapGet("/categoryId={categoryId:int}", GetXmlForAllProductsInCategoryAsync);
        endpointGroup.MapGet("/id={productId:int}", GetXmlForProductAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetXmlForAllProductsAsync(
        HttpContext httpContext,
        [FromQuery(Name = "currency")] string? currency,
        [FromServices] IProductToXmlService productXmlService)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ApplicationRootPath = baseUrl,
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForAllProductsAsync(httpContext.Response.Body, productXmlOptions);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForAllProductsWithPromotionsAsync(
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            PromotionSearchOptions = PromotionSearchOptions.All,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall
        };

        List<Product> products = await productSearchService.SearchProductsAsync(productSearchRequest);

        List<int> productIds = products.Select(x => x.Id).ToList();

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ApplicationRootPath = baseUrl,
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, productIds, productXmlOptions);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForAllProductsInManufacturerAsync(
        [FromRoute] int? manufacturerId,
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService)
    {
        if (manufacturerId == null) return Results.NotFound();

        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            ManufacturerId = manufacturerId,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall
        };

        List<Product> products = await productSearchService.SearchProductsAsync(productSearchRequest);

        List<int> productIds = products.Select(x => x.Id).ToList();

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ApplicationRootPath = baseUrl,
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, productIds, productXmlOptions);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForAllProductsInCategoryAsync(
        [FromRoute] int? categoryId,
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService)
    {
        if (categoryId == null) return Results.NotFound();

        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            CategoryId = categoryId,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall
        };

        List<Product> products = await productSearchService.SearchProductsAsync(productSearchRequest);

        List<int> productIds = products.Select(x => x.Id).ToList();

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ApplicationRootPath = baseUrl,
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, productIds, productXmlOptions);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForProductAsync(
        [FromRoute] int productId,
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductToXmlService productXmlService)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ApplicationRootPath = baseUrl,
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, [productId], productXmlOptions);

        return Results.Empty;
    }

    private static Currency? TryParseCurrencyFromQueryParameters(string? currency)
    {
        if (currency is null) return null;

        return currency switch
        {
            "bgn" or "BGN" => Currency.BGN,
            "eur" or "EUR" => Currency.EUR,
            "usd" or "USD" => Currency.USD,
            _ => null,
        };
    }
}