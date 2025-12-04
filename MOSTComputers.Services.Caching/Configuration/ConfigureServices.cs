using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.Caching.Services;
using MOSTComputers.Services.Caching.Services.Contracts;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.Caching.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddFusionCachingServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(30);
                options.SkipDistributedCacheRead = true;
                options.SkipDistributedCacheWrite = true;
            })
            .WithoutDistributedCache()
            .WithoutCacheKeyPrefix()
            .WithoutPlugins()
            .WithoutLogger();

        return services;
    }

    public static IServiceCollection AddMemoryCachingServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLazyCache();

        services.TryAddSingleton<ICache<string>, LazyCacheCache>();

        return services;
    }
}