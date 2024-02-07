using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using MOSTComputers.Services.LocalChangesHandling.Configuration;
using Respawn;
using Respawn.Graph;
using static MOSTComputers.Services.LocalChangesHandling.Tests.Integration.CommonTestElements;

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

    internal static readonly RespawnerOptions RespawnerOptionsToIgnoreTablesThatShouldntBeWiped = new()
    {
        DbAdapter = DbAdapter.SqlServer,
        CheckTemporalTables = false,
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