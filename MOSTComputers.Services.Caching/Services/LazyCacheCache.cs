using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using MOSTComputers.Services.Caching.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public TValue GetOrAdd<TValue>(string key, Func<TValue> addItemFactory, CancellationToken cancellationToken)
    {
        MemoryCacheEntryOptions options = new();

        options.AddExpirationToken(new CancellationChangeToken(cancellationToken));

        return _appCache.GetOrAdd(key, addItemFactory, options);
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

    public bool AddOrUpdate<TValue>(string key, TValue value)
    {
        _appCache.CacheProvider.Set(key, value, _defaultPolicy);

        return true;
    }

    public bool Evict(string key)
    {
        _appCache.Remove(key);

        return true;
    }
}