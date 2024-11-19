using MOSTComputers.Services.Caching.Models;

namespace MOSTComputers.Services.Caching.Services.Contracts;

public interface ICache<TKey>
{
    bool Add<TValue>(TKey key, TValue value);
    bool Add<TValue>(TKey key, TValue value, CancellationToken cancellationToken);
    bool Add<TValue>(TKey key, TValue value, CustomMemoryCacheEntryOptions cacheEntryOptions);
    bool AddOrUpdate<TValue>(TKey key, TValue value);
    bool AddOrUpdate<TValue>(TKey key, TValue value, CustomMemoryCacheEntryOptions cacheEntryOptions);
    bool Contains<TValue>(TKey key);
    bool Evict(TKey key);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory, CancellationToken cancellationToken);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory, CustomMemoryCacheEntryOptions cacheEntryOptions);
    TValue GetOrAdd<TValue>(TKey key, Func<TValue> addItemFactory, CustomMemoryCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken);
    TValue? GetValueOrDefault<TValue>(TKey key);
}