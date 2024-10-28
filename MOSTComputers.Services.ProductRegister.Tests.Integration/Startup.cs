using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductRegister.Configuration;
using Respawn.Graph;
using Respawn;
using System.Configuration;
using Microsoft.Extensions.Hosting;
using MOSTComputers.Services.Caching.Configuration;
using static MOSTComputers.Services.Caching.Configuration.ConfigureServices;
using static MOSTComputers.Services.ProductImageFileManagement.Configuration.ConfigureServices;
using static MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration.ConfigureServices;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

public class Startup
{
#pragma warning disable CA1822 // Mark members as static
    // Reason: If its static, then xUnit wont execute it
    public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
    {
        services.AddMemoryCachingServices();

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

        services.AddCachedProductServices(ConnectionString);
    }

    internal const string tableNameOfProductCharacteristicsTable = "ProductKeyword";
    internal const string tableNameOfCategoriesTable = "Categories";
    internal const string tableNameOfManifacturersTable = "Manufacturer";
    internal const string tableNameOfPromotionsTable = "Promotions";

    internal static string ConnectionString { get; } = GetConnectionString();
    internal static string ImageDirectoryRelativePath { get; } = GetImageDirectoryPath();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal static string ImageDirectoryFullPath { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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