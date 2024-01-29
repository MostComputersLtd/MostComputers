using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.Caching.Services.Contracts;

public interface ICache<TKey>
{
    public TValue? GetValueOrDefault<TValue>(TKey key);
    public TValue GetOrAdd<TValue>(TKey key, Func<TValue> value);
    TValue GetOrAdd<TValue>(string key, Func<TValue> addItemFactory, CancellationToken cancellationToken);
    public bool Contains<TValue>(TKey key);
    public bool Add<TValue>(TKey key, TValue value);
    bool Add<TValue>(string key, TValue value, CancellationToken cancellationToken);
    public bool AddOrUpdate<TValue>(TKey key, TValue value);
    public bool Evict(TKey key);
}