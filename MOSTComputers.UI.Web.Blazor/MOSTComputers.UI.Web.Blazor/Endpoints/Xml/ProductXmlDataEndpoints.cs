using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.XmlDownloads;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using MOSTComputers.UI.Web.Blazor.Services.Xml;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
using System.Security.Claims;
using static MOSTComputers.UI.Web.Blazor.Endpoints.PromotionPictureSource;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

public static class ProductXmlDataEndpoints
{
    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "product/" + "xml";

    private const string _rootResourceType = "products";

    private const string _allProductsResourceType = _rootResourceType + "/all";
    private const string _productsWithPromotionsResourceType = _rootResourceType + "/promotions";
    private static string GetProductsFromManufacturerResourceType(int manufacturerId)
    {
        return _rootResourceType + "/manufacturerId=" + manufacturerId.ToString();
    }

    private static string GetProductsInCategoryResourceType(int categoryId)
    {
        return _rootResourceType + "/categoryId=" + categoryId.ToString();
    }

    private static string GetProductResourceType(int productId)
    {
        return _rootResourceType + "/id=" + productId.ToString();
    }

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
        [FromServices] IProductToXmlService productXmlService,
        [FromServices] IXmlDownloadsRepository xmlDownloadsRepository)
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

        await RecordXmlDownloadAsync(xmlDownloadsRepository, httpContext, _allProductsResourceType);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForAllProductsWithPromotionsAsync(
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService,
        [FromServices] IXmlDownloadsRepository xmlDownloadsRepository)
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

        await RecordXmlDownloadAsync(xmlDownloadsRepository, httpContext, _productsWithPromotionsResourceType);

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForAllProductsInManufacturerAsync(
        [FromRoute] int? manufacturerId,
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService,
        [FromServices] IXmlDownloadsRepository xmlDownloadsRepository)
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

        await RecordXmlDownloadAsync(xmlDownloadsRepository, httpContext, GetProductsFromManufacturerResourceType(manufacturerId.Value));

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForAllProductsInCategoryAsync(
        [FromRoute] int? categoryId,
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService,
        [FromServices] IXmlDownloadsRepository xmlDownloadsRepository)
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

        await RecordXmlDownloadAsync(xmlDownloadsRepository, httpContext, GetProductsInCategoryResourceType(categoryId.Value));

        return Results.Empty;
    }

    private static async Task<IResult> GetXmlForProductAsync(
        [FromRoute] int productId,
        [FromQuery(Name = "currency")] string? currency,
        HttpContext httpContext,
        [FromServices] IProductService productService,
        [FromServices] IProductSearchService productSearchService,
        [FromServices] IProductToXmlService productXmlService,
        [FromServices] IXmlDownloadsRepository xmlDownloadsRepository)
    {
        HttpRequest request = httpContext.Request;

        string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

        Currency? prefferedCurrency = TryParseCurrencyFromQueryParameters(currency);

        Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound(productId);
        }

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall
        };

        List<Product> singleProductList = await productSearchService.SearchProductsAsync([product], productSearchRequest);

        if (singleProductList.Count == 0)
        {
            return Results.NotFound(productId);
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

        await RecordXmlDownloadAsync(xmlDownloadsRepository, httpContext, GetProductResourceType(productId));

        return Results.Empty;
    }

    private static async Task RecordXmlDownloadAsync(
        IXmlDownloadsRepository xmlDownloadsRepository,
        HttpContext httpContext,
        string xmlResourceType)
    {
        ClaimsPrincipal claimsPrincipal = httpContext.User;

        bool isAuthenticated = claimsPrincipal.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            XmlDownloadData xmlDownloadDataWithNoUser = new()
            {
                TimeStamp = DateTime.Now,
                XmlResourceType = xmlResourceType,
            };

            await xmlDownloadsRepository.InsertAsync(xmlDownloadDataWithNoUser);

            return;
        }

        string? userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        bool isCustomerUser = int.TryParse(userId, out int userIdParsed)
            && claimsPrincipal.IsInRole("CustomerInvoiceViewer");

        if (!isCustomerUser) return;

        string? username = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        string? contactPerson = claimsPrincipal.FindFirst("ContactPerson")?.Value;


        XmlDownloadData xmlDownloadData = new()
        {
            TimeStamp = DateTime.Now,
            XmlResourceType = xmlResourceType,
            BID = userIdParsed,
            UserName = username,
            ContactPerson = contactPerson,
        };

        await xmlDownloadsRepository.InsertAsync(xmlDownloadData);
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