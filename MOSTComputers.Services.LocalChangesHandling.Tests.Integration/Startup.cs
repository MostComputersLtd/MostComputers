using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using MOSTComputers.Services.LocalChangesHandling.Configuration;
using Respawn;
using Respawn.Graph;
using MOSTComputers.Services.ProductRegister.Configuration;
using Microsoft.Extensions.Hosting;
using static MOSTComputers.Services.LocalChangesHandling.Tests.Integration.CommonTestElements;
using static MOSTComputers.Services.ProductImageFileManagement.Configuration.ConfigureServices;
using static MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration.ConfigureServices;

namespace MOSTComputers.Services.LocalChangesHandling.Tests.Integration;

public class Startup
{
#pragma warning disable CA1822 // Mark members as static
    // Reason: If its static, then xUnit wont execute it
    public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
    {
        string productImageFolderFilePath = ImageDirectoryRelativePath;

        if (!Path.IsPathFullyQualified(productImageFolderFilePath))
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            IHostEnvironment hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

            int indexOfProjectName = hostEnvironment.ContentRootPath.IndexOf(hostEnvironment.ApplicationName);

            if (indexOfProjectName < 0) throw new InvalidOperationException("Project name and root folder name do not match.");

            int afterAppNameIndex = indexOfProjectName + hostEnvironment.ApplicationName.Length;

            string projectFolderPath = hostEnvironment.ContentRootPath[..afterAppNameIndex];

            productImageFolderFilePath = Path.GetFullPath(productImageFolderFilePath, projectFolderPath)
                .Replace('\\', '/');

            ImageDirectoryFullPath = productImageFolderFilePath;
        }

        services.AddProductImageFileManagement(productImageFolderFilePath);

        services.AddProductHtmlService();

        services.AddProductServices(ConnectionString);

        services.AddLocalChangesHandlingWithoutBackgroundService();

    }

    internal static string ConnectionString { get; } = GetConnectionString();
    internal static string ImageDirectoryRelativePath { get; } = GetImageDirectoryPath();
    internal static string ImageDirectoryFullPath { get; private set; }

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

    private static string GetImageDirectoryPath()
    {
        ExeConfigurationFileMap configMap = new()
        {
            ExeConfigFilename = "./App.config"
        };

        System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

        return config.AppSettings.Settings["ProductImageDirectory"].Value;
    }
}