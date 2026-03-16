using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Caching.Models;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;
using OneOf;
using ZiggyCreatures.Caching.Fusion;
using static MOSTComputers.UI.Web.Blazor.Services.Xml.ProductToXmlService;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml.Cached;

public class CachedProductToXmlService : IProductToXmlService
{
    public CachedProductToXmlService(
        IProductToXmlService innerService,
        //ICache<string> cache,
        IFusionCache fusionCache)
    {
        _innerService = innerService;
        //_cache = cache;
        _fusionCache = fusionCache;
    }

    private readonly IProductToXmlService _innerService;
    //private readonly ICache<string> _cache;
    private readonly IFusionCache _fusionCache;

    private const string _xmlObjectDataCacheKey = "productToXmlService:Xml:ObjectData";

    private const string _allXmlProductsCacheKey = "productToXmlService:Xml:All";

    private static readonly TimeSpan _defaultCacheItemAbsoluteExpiration = TimeSpan.FromMinutes(10);

    //private readonly CustomCacheEntryOptions _cacheEntryOptions = new()
    //{
    //    AbsoluteExpirationRelativeToNow = _defaultCacheItemAbsoluteExpiration,
    //};

    public async Task<ProductsXmlFullData> GetXmlObjectDataForProductsAsync(List<XmlProduct> xmlProducts)
    {
        //XmlObjectData data = await _cache.GetOrAddAsync(_xmlObjectDataCacheKey,
        //    () => _innerService.GetXmlObjectDataForProductsAsync(xmlProducts), _cacheEntryOptions);

        ProductsXmlFullData data = await _fusionCache.GetOrSetAsync(_xmlObjectDataCacheKey,
            (_) => _innerService.GetXmlObjectDataForProductsAsync(xmlProducts), _defaultCacheItemAbsoluteExpiration);

        return new()
        {
            DateOfExport = data.DateOfExport,
            ExchangeRates = data.ExchangeRates,
        };
    }

    public Task TryGetXmlForAllPublicProductsAsync(Stream outputStream, ProductXmlOptions? productXmlOptions = null)
    {
        return _innerService.TryGetXmlForAllPublicProductsAsync(outputStream, productXmlOptions);
    }

    public async Task<OneOf<string, InvalidXmlResult>> TryGetXmlForProductsAsync(List<Product> products, ProductXmlOptions? productXmlOptions = null)
    {
        return await _innerService.TryGetXmlForProductsAsync(products, productXmlOptions);
    }

    public Task TryGetXmlForProductsAsync(Stream outputStream, List<Product> products, ProductXmlOptions? productXmlOptions = null)
    {
        return _innerService.TryGetXmlForProductsAsync(outputStream, products, productXmlOptions);
    }
}