using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using MOSTComputers.Services.Caching.Models;
using MOSTComputers.Services.Caching.Services.Contracts;

using static MOSTComputers.Services.Caching.Mapping.LazyCacheMappingUtils;

namespace MOSTComputers.Services.Caching.Services;
internal sealed class LazyCacheCache : ICache<string>
{
    public LazyCacheCache(IAppCache appCache)
    {
        _appCache = appCache;
    }

    private readonly IAppCache _appCache;

    private readonly MemoryCacheEntryOptions _defaultPolicy = new();

    public TValue? GetValueOrDefault<TValue>(string key)
    {
        bool success = _appCache.TryGetValue(key, out TValue value);

        return success ? value : default;
    }

    public bool Contains<TValue>(string key)
    {
        return _appCache.TryGetValue(key, out TValue _);
    }

    public TValue GetOrAdd<TValue>(string key, Func<TValue> addItemFactory)
    {
        return _appCache.GetOrAdd(key, addItemFactory);
    }

    public TValue GetOrAdd<TValue>(string key, Func<ICacheEntry, TValue> addItemFactory)
    {
        return _appCache.GetOrAdd(key, addItemFactory);
    }

    public async Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory)
    {
        return await _appCache.GetOrAddAsync(key, addItemFactory);
    }

    public async Task<TValue> GetOrAddAsync<TValue>(string key, Func<ICacheEntry, Task<TValue>> addItemFactory)
    {
        return await _appCache.GetOrAddAsync(key, addItemFactory);
    }

    public TValue GetOrAdd<TValue>(string key, Func<TValue> addItemFactory, CustomCacheEntryOptions cacheEntryOptions)
    {
        return _appCache.GetOrAdd(key, addItemFactory, Map(cacheEntryOptions));
    }

    public async Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory, CustomCacheEntryOptions cacheEntryOptions)
    {
        return await _appCache.GetOrAddAsync(key, addItemFactory, Map(cacheEntryOptions));
    }

    public TValue GetOrAdd<TValue>(string key, Func<TValue> addItemFactory, CancellationToken cancellationToken)
    {
        MemoryCacheEntryOptions options = new();

        options.AddExpirationToken(new CancellationChangeToken(cancellationToken));

        return _appCache.GetOrAdd(key, addItemFactory, options);
    }

    public async Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory, CancellationToken cancellationToken)
    {
        MemoryCacheEntryOptions options = new();

        options.AddExpirationToken(new CancellationChangeToken(cancellationToken));

        return await _appCache.GetOrAddAsync(key, addItemFactory, options);
    }

    public TValue GetOrAdd<TValue>(string key, Func<TValue> addItemFactory,
        CustomCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken)
    {
        MemoryCacheEntryOptions options = Map(cacheEntryOptions);

        options.AddExpirationToken(new CancellationChangeToken(cancellationToken));

        return _appCache.GetOrAdd(key, addItemFactory, options);
    }

    public async Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory,
        CustomCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken)
    {
        MemoryCacheEntryOptions options = Map(cacheEntryOptions);

        options.AddExpirationToken(new CancellationChangeToken(cancellationToken));

        return await _appCache.GetOrAddAsync(key, addItemFactory, options);
    }

    public bool Add<TValue>(string key, TValue value)
    {
        _appCache.Add(key, value);

        return true;
    }

    public bool Add<TValue>(string key, TValue value, CancellationToken cancellationToken)
    {
        MemoryCacheEntryOptions options = new();

        options.AddExpirationToken(new CancellationChangeToken(cancellationToken));

        _appCache.Add(key, value, options);

        return true;
    }

    public bool Add<TValue>(string key, TValue value, CustomCacheEntryOptions cacheEntryOptions)
    {
        _appCache.Add(key, value, Map(cacheEntryOptions));

        return true;
    }

    public bool AddOrUpdate<TValue>(string key, TValue value)
    {
        _appCache.CacheProvider.Set(key, value, _defaultPolicy);

        return true;
    }

    public bool AddOrUpdate<TValue>(string key, TValue value, CustomCacheEntryOptions cacheEntryOptions)
    {
        _appCache.CacheProvider.Set(key, value, Map(cacheEntryOptions));

        return true;
    }

    public bool Evict(string key)
    {
        _appCache.Remove(key);

        return true;
    }
}