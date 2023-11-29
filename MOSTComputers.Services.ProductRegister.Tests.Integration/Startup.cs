using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductRegister.Configuration;
using System.Configuration;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

public class Startup
{
#pragma warning disable CA1822 // Mark members as static
    public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
    {
        services.AddProductServices(ConnectionString);
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