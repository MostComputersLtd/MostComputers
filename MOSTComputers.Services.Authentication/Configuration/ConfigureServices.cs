using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.DataAccess.Common;

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

        services.AddScoped<ICustomAuthenticationService, CustomersAndEmployeesAuthenticationService>();

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
}
