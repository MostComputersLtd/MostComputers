using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductRegister.Configuration;
using Respawn.Graph;
using Respawn;
using System.Configuration;
using MOSTComputers.Services.Caching.Configuration;
using static MOSTComputers.Services.Caching.Configuration.ConfigureServices;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

public class Startup
{
#pragma warning disable CA1822 // Mark members as static
    public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
    {
        services.AddMemoryCachingServices();

        services.AddCachedProductServices(ConnectionString);
    }

    internal const string tableNameOfProductCharacteristicsTable = "ProductKeyword";
    internal const string tableNameOfCategoriesTable = "Categories";
    internal const string tableNameOfManifacturersTable = "Manufacturer";
    internal const string tableNameOfPromotionsTable = "Promotions";

    internal static string ConnectionString { get; } = GetConnectionString();

    internal static readonly RespawnerOptions RespawnerOptionsToIgnoreTablesThatShouldntBeWiped = new()
    {
        DbAdapter = DbAdapter.SqlServer,
        TablesToIgnore = new Table[]
        {
            tableNameOfCategoriesTable,
            tableNameOfManifacturersTable,
            tableNameOfProductCharacteristicsTable,
            tableNameOfPromotionsTable,
        }
    };

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