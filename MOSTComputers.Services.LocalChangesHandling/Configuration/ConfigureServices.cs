using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.LocalChangesHandling.Services;
using MOSTComputers.Services.LocalChangesHandling.Services.BackgroundServices;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using static MOSTComputers.Services.ProductRegister.Configuration.ConfigureServices;

namespace MOSTComputers.Services.LocalChangesHandling.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddLocalChangesHandlingBackgroundService(this IServiceCollection services, string connectionString)
    {
        services.AddProductServices(connectionString);

        services.AddScoped<IProductChangesService, ProductChangesService>();

        services.AddHostedService<LocalChangesCheckingBackgroundService>();

        return services;
    }

    public static IServiceCollection AddLocalChangesHandlingWithoutBackgroundService(this IServiceCollection services, string connectionString)
    {
        services.AddProductServices(connectionString);

        services.AddScoped<IProductChangesService, ProductChangesService>();

        return services;
    }
}