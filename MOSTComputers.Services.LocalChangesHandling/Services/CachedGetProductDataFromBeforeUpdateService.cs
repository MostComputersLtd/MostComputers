using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Caching.Services.Contracts;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.Services.ProductRegister.StaticUtilities;

namespace MOSTComputers.Services.LocalChangesHandling.Services;

internal sealed class CachedGetProductDataFromBeforeUpdateService : IGetProductDataFromBeforeUpdateService
{
    public CachedGetProductDataFromBeforeUpdateService(ICache<string> cache)
    {
        _cache = cache;
    }

    private readonly ICache<string> _cache;

    public Product? GetProductBeforeUpdate(uint productId)
    {
        string key = CacheKeyUtils.Product.GetUpdatedByIdKey((int)productId);

        return _cache.GetValueOrDefault<Product>(key);
    }

    public bool HandleAfterUpdate(uint productId)
    {
        string key = CacheKeyUtils.Product.GetUpdatedByIdKey((int)productId);

        return _cache.Evict(key);
    }
}