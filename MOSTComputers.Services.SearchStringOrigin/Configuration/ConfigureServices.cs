using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Services;

namespace MOSTComputers.Services.SearchStringOrigin.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddSearchStringOriginService(this IServiceCollection services)
    {
        services.AddScoped<ISearchStringOriginService, SearchStringOriginService>();

        return services;
    }
}