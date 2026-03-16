using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Products;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.UI.Web.Client.Endpoints.Images;
using MOSTComputers.UI.Web.Client.Services.Xml;
using MOSTComputers.UI.Web.Client.Services.Xml.Contracts;
using static MOSTComputers.UI.Web.Client.Endpoints.PromotionPictureSource;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Client.Endpoints.Xml;

public static class ProductXmlDataEndpoints
{
    public sealed class ProductSearchData
    {
        public string? SearchData { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public bool AvailableOnly { get; set; } = false;
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "product/" + "xml";

    public static IEndpointConventionBuilder MapProductXmlEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/all", GetXmlForAllProductsAsync);
        endpointGroup.MapGet("/promotions", GetXmlForAllProductsWithPromotionsAsync);
        endpointGroup.MapGet("/manufacturerId={manufacturerId:int}", GetXmlForAllProductsInManufacturerAsync);
        endpointGroup.MapGet("/categoryId={categoryId:int}", GetXmlForAllProductsInCategoryAsync);
        endpointGroup.MapGet("/id={productId:int}", GetXmlForProductAsync);

        endpointGroup.MapPost("", GetXmlForSelectionAsync);

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
            ImageFilesBasePath = CombinePathsWithSeparator('/', baseUrl, ProductImageFileDataEndpoints.EndpointGroupRoute),
            PromotionGroupImagesBasePath = CombinePathsWithSeparator('/', baseUrl, "promotionGroupImages"),
            GetPromotionPictureSourceUrlById = id => CombinePathsWithSeparator('/', baseUrl, GetPromotionPictureSource(id) ?? ""),
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForAllPublicProductsAsync(httpContext.Response.Body, productXmlOptions);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForSelectionAsync(
        HttpContext httpContext,
        [FromQuery(Name = "currency")] string? currency,
        [FromServices] IProductToXmlService productXmlService,
        [FromServices] IProductSearchService productSearchService,
        [FromBody] ProductSearchData? productSearchData = null)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            ProductStatus = productSearchData?.AvailableOnly == true ? ProductStatusSearchOptions.Available : ProductStatusSearchOptions.AvailableAndCall,
            CategoryId = productSearchData?.CategoryId,
            ManufacturerId = productSearchData?.ManufacturerId,
            UserInputString = productSearchData?.SearchData,
        };

        List<Product> products = await productSearchService.SearchProductsAsync(productSearchRequest);

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ImageFilesBasePath = CombinePathsWithSeparator('/', baseUrl, ProductImageFileDataEndpoints.EndpointGroupRoute),
            PromotionGroupImagesBasePath = CombinePathsWithSeparator('/', baseUrl, "promotionGroupImages"),
            GetPromotionPictureSourceUrlById = id => CombinePathsWithSeparator('/', baseUrl, GetPromotionPictureSource(id) ?? ""),
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, products, productXmlOptions);

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

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ImageFilesBasePath = CombinePathsWithSeparator('/', baseUrl, ProductImageFileDataEndpoints.EndpointGroupRoute),
            PromotionGroupImagesBasePath = CombinePathsWithSeparator('/', baseUrl, "promotionGroupImages"),
            GetPromotionPictureSourceUrlById = id => CombinePathsWithSeparator('/', baseUrl, GetPromotionPictureSource(id) ?? ""),
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, products, productXmlOptions);

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

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ImageFilesBasePath = CombinePathsWithSeparator('/', baseUrl, ProductImageFileDataEndpoints.EndpointGroupRoute),
            PromotionGroupImagesBasePath = CombinePathsWithSeparator('/', baseUrl, "promotionGroupImages"),
            GetPromotionPictureSourceUrlById = id => CombinePathsWithSeparator('/', baseUrl, GetPromotionPictureSource(id) ?? ""),
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, products, productXmlOptions);

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

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ImageFilesBasePath = CombinePathsWithSeparator('/', baseUrl, ProductImageFileDataEndpoints.EndpointGroupRoute),
            PromotionGroupImagesBasePath = CombinePathsWithSeparator('/', baseUrl, "promotionGroupImages"),
            GetPromotionPictureSourceUrlById = id => CombinePathsWithSeparator('/', baseUrl, GetPromotionPictureSource(id) ?? ""),
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, products, productXmlOptions);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForProductAsync(
        [FromRoute] int productId,
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductService productService,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        Product? product = await productService.GetByIdAsync(productId);

        List<Product> singleProductList;

        if (product != null)
        {
            ProductSearchRequest productSearchRequest = new()
            {
                OnlyVisibleByEndUsers = true,
                ProductStatus = ProductStatusSearchOptions.AvailableAndCall
            };

            singleProductList = await productSearchService.SearchProductsAsync([product], productSearchRequest);
        }
        else
        {
            singleProductList = [];
        }

        ProductToXmlService.ProductXmlOptions productXmlOptions = new()
        {
            ImageFilesBasePath = CombinePathsWithSeparator('/', baseUrl, ProductImageFileDataEndpoints.EndpointGroupRoute),
            PromotionGroupImagesBasePath = CombinePathsWithSeparator('/', baseUrl, "promotionGroupImages"),
            GetPromotionPictureSourceUrlById = id => CombinePathsWithSeparator('/', baseUrl, GetPromotionPictureSource(id) ?? ""),
            PrefferedPriceCurrency = prefferedCurrency,
        };

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await productXmlService.TryGetXmlForProductsAsync(httpContext.Response.Body, singleProductList, productXmlOptions);

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