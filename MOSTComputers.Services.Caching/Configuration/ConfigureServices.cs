using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.Caching.Services;
using MOSTComputers.Services.Caching.Services.Contracts;

namespace MOSTComputers.Services.Caching.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddMemoryCachingServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLazyCache();

        services.TryAddSingleton<ICache<string>, LazyCacheCache>();

        return services;
    }
}