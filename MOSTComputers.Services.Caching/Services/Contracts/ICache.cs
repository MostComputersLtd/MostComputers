using Microsoft.Extensions.Caching.Memory;
using MOSTComputers.Services.Caching.Models;

namespace MOSTComputers.Services.Caching.Services.Contracts;
public interface ICache<TKey>
{
    TValue? GetValueOrDefault<TValue>(TKey key);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory, CancellationToken cancellationToken);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory, CustomCacheEntryOptions cacheEntryOptions);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory, CustomCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken);
    Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory);
    Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory, CustomCacheEntryOptions cacheEntryOptions);
    Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory, CancellationToken cancellationToken);
    Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> addItemFactory, CustomCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken);
    bool Add<TValue>(TKey key, TValue value);
    bool Add<TValue>(TKey key, TValue value, CancellationToken cancellationToken);
    bool Add<TValue>(TKey key, TValue value, CustomCacheEntryOptions cacheEntryOptions);
    bool AddOrUpdate<TValue>(TKey key, TValue value);
    bool AddOrUpdate<TValue>(TKey key, TValue value, CustomCacheEntryOptions cacheEntryOptions);
    bool Contains<TValue>(TKey key);
    bool Evict(TKey key);
    TValue GetOrAdd<TValue>(string key, Func<ICacheEntry, TValue> addItemFactory);
    Task<TValue> GetOrAddAsync<TValue>(string key, Func<ICacheEntry, Task<TValue>> addItemFactory);
}