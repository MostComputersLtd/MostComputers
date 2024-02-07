using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.LocalChangesHandling.Services;
using MOSTComputers.Services.LocalChangesHandling.Services.BackgroundServices;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.Services.LocalChangesHandling.Validation;
using static MOSTComputers.Services.ProductRegister.Configuration.ConfigureServices;
using static MOSTComputers.Services.XMLDataOperations.Configuration.ConfigureServices;
using static MOSTComputers.Services.Caching.Configuration.ConfigureServices;

namespace MOSTComputers.Services.LocalChangesHandling.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddLocalChangesHandlingBackgroundService(this IServiceCollection services, string connectionString)
    {
        services.AddMemoryCachingServices();

        services.AddCachedProductServices(connectionString);

        services.AddXmlDeserializeService();

        services.AddScoped<IGetProductDataFromBeforeUpdateService, CachedGetProductDataFromBeforeUpdateService>();

        services.AddScoped<IProductChangesService, ProductChangesService>();

        services.AddHostedService<LocalChangesCheckingBackgroundService>();

        return services;
    }

    public static IServiceCollection AddLocalChangesHandlingWithoutBackgroundService(this IServiceCollection services, string connectionString)
    {
        services.AddMemoryCachingServices();

        services.AddCachedProductServices(connectionString);

        services.AddXmlDeserializeService();

        services.AddScoped<IGetProductDataFromBeforeUpdateService, CachedGetProductDataFromBeforeUpdateService>();

        services.AddScoped<IProductChangesService, ProductChangesService>();

        return services;
    }
}