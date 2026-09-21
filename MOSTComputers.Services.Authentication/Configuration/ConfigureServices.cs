using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Authentication.DataAccess;
using MOSTComputers.Services.Authentication.DataAccess.Contracts;
using MOSTComputers.Services.Authentication.DataAccess.Mapping;
using MOSTComputers.Services.DataAccess.Common;
using ZiggyCreatures.Caching.Fusion;

namespace MOSTComputers.Services.Authentication.Configuration;

public static class ConfigureServices
{
    public const string LocalDBConnectionStringProviderServiceKey = "MOSTComputers.Services.Authentication.ConnectionStringProvider.LocalDB";

    public const string ApiSecretServiceServiceKey = "MOSTComputers.Services.Authentication.ApiSecretServiceServiceKey";
    public const string ApiSecretPermissionServiceServiceKey = "MOSTComputers.Services.Authentication.ApiSecretPermissionServiceServiceKey";
    public const string ApiTokenServiceServiceKey = "MOSTComputers.Services.Authentication.ApiTokenServiceServiceKey";

    public static IServiceCollection AddUserAuthServices(this IServiceCollection services, string connectionString)
    {
        services.TryAddConnectionStringProvider(connectionString, LocalDBConnectionStringProviderServiceKey);

        services.TryAddScoped<ICustomAuthenticationService, CustomersAndEmployeesAuthenticationService>();

        return services;
    }

    public static IServiceCollection AddRemoteAuthServices(this IServiceCollection services, string connectionString)
    {
        services.TryAddConnectionStringProvider(connectionString, LocalDBConnectionStringProviderServiceKey);

        services.TryAddScoped<IApiSecretRepository, ApiSecretRepository>();
        services.TryAddScoped<IApiSecretPermissionRepository, ApiSecretPermissionRepository>();
        services.TryAddScoped<IApiTokenRepository, ApiTokenRepository>();

        AddRemoteAuthDapperMappings();

        services.AddKeyedScoped<IApiSecretService, ApiSecretService>(ApiSecretServiceServiceKey);
        services.AddScoped<IApiSecretService, CachedApiSecretService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IApiSecretService>(ApiSecretServiceServiceKey),
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddKeyedScoped<IApiSecretPermissionService, ApiSecretPermissionService>(ApiSecretPermissionServiceServiceKey);
        services.AddScoped<IApiSecretPermissionService, CachedApiSecretPermissionService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IApiSecretPermissionService>(
                    ApiSecretPermissionServiceServiceKey),
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddKeyedScoped<IApiTokenService, ApiTokenService>(ApiTokenServiceServiceKey);
        services.AddScoped<IApiTokenService, CachedApiTokenService>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredKeyedService<IApiTokenService>(ApiTokenServiceServiceKey),
                serviceProvider.GetRequiredService<IFusionCache>());
        });

        services.AddScoped<IApiSecretAuthService, ApiSecretAuthService>();
        services.AddScoped<IApiTokenAuthService, ApiTokenAuthService>();

        return services;
    }

    public static IServiceCollection TryAddConnectionStringProvider(
        this IServiceCollection services, string connectionString, string connectionStringKey)
    {
        services.AddKeyedScoped<IConnectionStringProvider, ConnectionStringProvider>(connectionStringKey, (_, _) =>
        {
            ConnectionStringProvider dapperDataAccess = new(connectionString);

            return dapperDataAccess;
        });

        return services;
    }

    public static void AddRemoteAuthDapperMappings()
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new ApiSecretEntityMap());
            config.AddMap(new ApiTokenEntityMap());
        });
    }
}
