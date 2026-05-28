using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Sitemap;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Products.Contracts;
using System.Text;
using System.Xml;

namespace MOSTComputers.UI.Web.Blazor.Endpoints.Xml;

public static class SitemapEndpoints
{
    internal const string EndpointGroupRoute = "";

    internal const int MaxUrlsInStaticPagesSitemap = 25000;
    internal const int MaxUrlsInProductSitemap = 25000;
    private const string _sitemapIndexUrl = "/sitemap.xml";
    
    private const string _staticPageSitemapUrl = "/sitemap-static{page:int}.xml";
    private const string _productSitemapUrl = "/sitemap-products{page:int}.xml";

    internal static readonly IReadOnlyList<string> StaticPageUrlsInSitemap = [
        "",
        "downloadXml",
        "repairs",
        "contactUs",
        "account/login",
    ];

    private static string GetStaticPageSitemapUrl(int page)
    {
        return $"sitemap-static{page}.xml";
    }

    private static string GetProductSitemapUrl(int page)
    {
        return $"sitemap-products{page}.xml";
    }

    private const string _invalidXmlDefaultMessage = "Something went wrong";

    public static IEndpointConventionBuilder MapSitemapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet(_sitemapIndexUrl, GetSitemapIndexForAllResourcesAsync);
        endpointGroup.MapGet(_staticPageSitemapUrl, GetStaticPagesSitemapAsync);
        endpointGroup.MapGet(_productSitemapUrl, GetProductsSitemapAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetSitemapIndexForAllResourcesAsync(
        [FromServices] EndpointDataSource EndpointsDataSource,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService)
    {
        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall,
        };

        List<Product> products = await productSearchService.SearchProductsAsync(productSearchRequest);

        int staticPagesSitemapChunksCount = (StaticPageUrlsInSitemap.Count + MaxUrlsInProductSitemap - 1) / MaxUrlsInProductSitemap;
        int productSitemapChunksCount = (products.Count + MaxUrlsInProductSitemap - 1) / MaxUrlsInProductSitemap;

        HttpRequest httpRequest = httpContext.Request;

        string baseUrl = $"https://{httpRequest.Host}{httpRequest.PathBase}/";

        SitemapIndex sitemapIndex = new();

        for (int i = 0; i < staticPagesSitemapChunksCount; i++)
        {
            SitemapIndexEntry sitemapIndexEntry = new()
            {
                Loc = Path.Combine(baseUrl, GetStaticPageSitemapUrl(i + 1)),
            };

            sitemapIndex.Sitemaps.Add(sitemapIndexEntry);
        }

        for (int i = 0; i < productSitemapChunksCount; i++)
        {
            SitemapIndexEntry sitemapIndexEntry = new()
            {
                Loc = Path.Combine(baseUrl, EndpointGroupRoute, GetProductSitemapUrl(i + 1)),
            };

            sitemapIndex.Sitemaps.Add(sitemapIndexEntry);
        }

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await WriteXmlToStreamAsync(httpContext.Response.Body, sitemapIndex);

        return Results.Empty;
    }
    
    private static async Task<IResult> GetStaticPagesSitemapAsync(
        [FromRoute] int page,
        HttpContext httpContext)
    {
        if (page < 1)
        {
            return Results.NotFound();
        }

        int sitemapChunksCount = (StaticPageUrlsInSitemap.Count + MaxUrlsInProductSitemap - 1) / MaxUrlsInProductSitemap;

        if (page > sitemapChunksCount) return Results.NotFound();

        HttpRequest httpRequest = httpContext.Request;

        string baseUrl = $"https://{httpRequest.Host}{httpRequest.PathBase}/";

        int pageStartIndex = (page - 1) * MaxUrlsInStaticPagesSitemap;
        int pageEndIndex = pageStartIndex + MaxUrlsInStaticPagesSitemap;

        pageEndIndex = Math.Min(pageEndIndex, StaticPageUrlsInSitemap.Count - 1);

        Sitemap sitemap = new();

        for (int i = pageStartIndex;  i <= pageEndIndex; i++)
        {
            string staticPageUrl = StaticPageUrlsInSitemap[i];

            UrlEntry urlEntry = new()
            {
                Loc = Path.Combine(baseUrl, staticPageUrl),
            };

            sitemap.UrlEntries.Add(urlEntry);
        }

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await WriteXmlToStreamAsync(httpContext.Response.Body, sitemap);

        return Results.Empty;
    }

    private static async Task<IResult> GetProductsSitemapAsync(
        [FromRoute] int page,
        HttpContext httpContext,
        [FromServices] IProductSearchService productSearchService)
    {
        if (page < 1)
        {
            return Results.NotFound();
        }

        ProductSearchRequest productSearchRequest = new()
        {
            OnlyVisibleByEndUsers = true,
            ProductStatus = ProductStatusSearchOptions.AvailableAndCall,
        };

        List<Product> products = await productSearchService.SearchProductsAsync(productSearchRequest);

        int sitemapChunksCount = (products.Count + MaxUrlsInProductSitemap - 1) / MaxUrlsInProductSitemap;

        if (page > sitemapChunksCount) return Results.NotFound();

        HttpRequest httpRequest = httpContext.Request;

        string baseUrl = $"https://{httpRequest.Host}{httpRequest.PathBase}/";

        int pageStartIndex = (page - 1) * MaxUrlsInProductSitemap;
        int pageEndIndex = pageStartIndex + MaxUrlsInProductSitemap;

        pageEndIndex = Math.Min(pageEndIndex, products.Count - 1);

        Sitemap sitemap = new();

        for (int i = pageStartIndex;  i <= pageEndIndex; i++)
        {
            Product product = products[i];

            UrlEntry urlEntry = new()
            {
                Loc = Path.Combine(baseUrl, $"product/productData/{product.Id}"),
            };

            sitemap.UrlEntries.Add(urlEntry);
        }

        httpContext.Response.ContentType = "application/xml";
        httpContext.Response.Headers.TryAdd("Content-Disposition", "inline; filename=data.xml");

        await WriteXmlToStreamAsync(httpContext.Response.Body, sitemap);

        return Results.Empty;
    }

    private static async Task WriteXmlToStreamAsync(Stream outputStream, IXmlAsyncSerializable xmlAsyncSerializable)
    {
        XmlWriter? xmlWriter = null;

        try
        {
            xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings { Async = true, Indent = true });

            await xmlAsyncSerializable.WriteXmlAsync(xmlWriter, "data");
        }
        catch (InvalidOperationException)
        {
            if (xmlWriter is not null)
            {
                string errorMessage = _invalidXmlDefaultMessage;

                await xmlWriter.WriteElementStringAsync(null, "Error", null, errorMessage);
            }
        }
        finally
        {
            if (xmlWriter is not null)
            {
                await xmlWriter.DisposeAsync();
            }
        }
    }
}