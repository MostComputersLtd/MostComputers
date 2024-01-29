using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductRegister.Configuration;
using System.Configuration;
using MOSTComputers.Services.LocalChangesHandling.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MOSTComputers.Services.LocalChangesHandling.Tests.Integration;

public class Startup
{
#pragma warning disable CA1822 // Mark members as static
    public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
    {
        services.AddLocalChangesHandlingWithoutBackgroundService(ConnectionString);
    }

    internal static string ConnectionString { get; } = GetConnectionString();

    private static string GetConnectionString()
    {
        ExeConfigurationFileMap configMap = new()
        {
            ExeConfigFilename = "./App.config"
        };

        System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

        return config.ConnectionStrings.ConnectionStrings["Default"].ConnectionString;
    }
}