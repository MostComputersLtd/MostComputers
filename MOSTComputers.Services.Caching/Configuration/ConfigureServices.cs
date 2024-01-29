using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MOSTComputers.Services.Caching.Services;
using MOSTComputers.Services.Caching.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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