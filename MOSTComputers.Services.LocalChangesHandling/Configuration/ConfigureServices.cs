using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.LocalChangesHandling.Services;
using MOSTComputers.Services.LocalChangesHandling.Services.BackgroundServices;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using static MOSTComputers.Services.XMLDataOperations.Configuration.ConfigureServices;
using static MOSTComputers.Services.Caching.Configuration.ConfigureServices;
using Microsoft.Extensions.Hosting;

namespace MOSTComputers.Services.LocalChangesHandling.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddLocalChangesHandlingBackgroundService(this IServiceCollection services)
    {
        services.AddMemoryCachingServices();

        services.AddXmlDeserializeService();

        services.AddScoped<IGetProductDataFromBeforeUpdateService, CachedGetProductDataFromBeforeUpdateService>();

        services.AddScoped<IProductChangesService, ProductChangesService>();

        services.AddHostedService<LocalChangesCheckingBackgroundService>();

        services.AddScoped<ILocalChangesCheckingImmediateExecutionService, LocalChangesCheckingImmediateExecutionService>(serviceProvider =>
        {
            LocalChangesCheckingBackgroundService localChangesCheckingBackgroundService
                = (LocalChangesCheckingBackgroundService)serviceProvider.GetServices<IHostedService>()
                .First(backgroundService => backgroundService is LocalChangesCheckingBackgroundService);

            return new(localChangesCheckingBackgroundService);
        });

        return services;
    }

    public static IServiceCollection AddLocalChangesHandlingWithoutBackgroundService(this IServiceCollection services)
    {
        services.AddMemoryCachingServices();

        services.AddXmlDeserializeService();

        services.AddScoped<IGetProductDataFromBeforeUpdateService, CachedGetProductDataFromBeforeUpdateService>();

        services.AddScoped<IProductChangesService, ProductChangesService>();

        return services;
    }
}