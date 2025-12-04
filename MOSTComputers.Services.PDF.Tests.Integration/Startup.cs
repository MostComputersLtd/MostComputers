using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MOSTComputers.Services.DAL.Documents.Configuration;
using MOSTComputers.Services.PDF.Configuration;
using System.Configuration;
using static MOSTComputers.Services.PDF.Configuration.ConfigureServices;

namespace MOSTComputers.Services.PDF.Tests.Integration;
public class Startup
{
#pragma warning disable CA1822 // Mark members as static
    // Reason: If its static, then xUnit wont execute it
    public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        IHostEnvironment hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

        int indexOfProjectName = hostEnvironment.ContentRootPath.IndexOf(hostEnvironment.ApplicationName);

        if (indexOfProjectName < 0) throw new InvalidOperationException("Project name and root folder name do not match.");

        int afterAppNameIndex = indexOfProjectName + hostEnvironment.ApplicationName.Length;

        string projectFolderPath = hostEnvironment.ContentRootPath[..afterAppNameIndex];

        GenerateTemplateFullPaths(projectFolderPath);

        services.AddDocumentRepositories(DocumentDBConnectionString);

        services.AddPdfInvoiceGeneratorFromDataServices(HtmlInvoiceTemplateFileFullPath);

        services.AddPdfWarrantyCardWithoutPricesGeneratorFromDataServices(HtmlWarrantyCardWithoutPricesTemplateFileFullPath);
        services.AddPdfWarrantyCardWithPricesGeneratorFromDataServices(HtmlWarrantyCardWithPricesTemplateFileFullPath);

        GenerateTestResultFolderFullPaths(projectFolderPath);
    }

    private static void GenerateTemplateFullPaths(string projectFolderPath)
    {
        string htmlInvoiceTemplateFullPath = GetFullPathFromRelativeOneThatStartsFromProjectFolder(HtmlInvoiceTemplateFileRelativePath, projectFolderPath);

        HtmlInvoiceTemplateFileFullPath = htmlInvoiceTemplateFullPath;

        string htmlWarrantyCardWithPricesTemplateFullPath = GetFullPathFromRelativeOneThatStartsFromProjectFolder(HtmlWarrantyCardWithPricesTemplateFileRelativePath, projectFolderPath);

        HtmlWarrantyCardWithPricesTemplateFileFullPath = htmlWarrantyCardWithPricesTemplateFullPath;

        string htmlWarrantyCardWithoutPricesTemplateFullPath = GetFullPathFromRelativeOneThatStartsFromProjectFolder(HtmlWarrantyCardWithoutPricesTemplateFileRelativePath, projectFolderPath);

        HtmlWarrantyCardWithoutPricesTemplateFileFullPath = htmlWarrantyCardWithoutPricesTemplateFullPath;
    }

    private static void GenerateTestResultFolderFullPaths(string projectFolderPath)
    {
        string pdfInvoicesTestFolderFullPath = GetFullPathFromRelativeOneThatStartsFromProjectFolder(PdfInvoiceTestFolderRelativePath, projectFolderPath);

        PdfInvoiceTestFolderFullPath = pdfInvoicesTestFolderFullPath;

        string pdfWarrantyCardWithPricesTestFolderFullPath = GetFullPathFromRelativeOneThatStartsFromProjectFolder(PdfWarrantyCardWithPricesTestFolderRelativePath, projectFolderPath);

        PdfWarrantyCardWithPricesTestFolderFullPath = pdfWarrantyCardWithPricesTestFolderFullPath;

        string pdfWarrantyCardWithoutPricesTestFolderFullPath = GetFullPathFromRelativeOneThatStartsFromProjectFolder(PdfWarrantyCardWithoutPricesTestFolderRelativePath, projectFolderPath);

        PdfWarrantyCardWithoutPricesTestFolderFullPath = pdfWarrantyCardWithoutPricesTestFolderFullPath;
    }

    private static string GetFullPathFromRelativeOneThatStartsFromProjectFolder(string relativePath, string projectFolderPath)
    {
        if (!Path.IsPathFullyQualified(relativePath))
        {
            return Path.GetFullPath(relativePath, projectFolderPath)
                .Replace('\\', '/');
        }

        return relativePath;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    internal static string DocumentDBConnectionString { get; } = GetConfigurationConnectionStringData("DocumentDBConnectionString");

    internal static string HtmlInvoiceTemplateFileRelativePath { get; } = GetConfigurationAppSettingsData("HtmlInvoiceTemplate");
    internal static string HtmlInvoiceTemplateFileFullPath { get; private set; }

    internal static string HtmlWarrantyCardWithPricesTemplateFileRelativePath { get; } = GetConfigurationAppSettingsData("HtmlWarrantyCardWithPricesTemplate");
    internal static string HtmlWarrantyCardWithPricesTemplateFileFullPath { get; private set; }

    internal static string HtmlWarrantyCardWithoutPricesTemplateFileRelativePath { get; } = GetConfigurationAppSettingsData("HtmlWarrantyCardWithoutPricesTemplate");
    internal static string HtmlWarrantyCardWithoutPricesTemplateFileFullPath { get; private set; }

    internal static string PdfInvoiceTestFolderRelativePath { get; } = GetConfigurationAppSettingsData("PdfInvoiceTestFolder");
    internal static string PdfInvoiceTestFolderFullPath { get; private set; }

    internal static string PdfWarrantyCardWithPricesTestFolderRelativePath { get; } = GetConfigurationAppSettingsData("PdfWarrantyCardWithPricesTestFolder");
    internal static string PdfWarrantyCardWithPricesTestFolderFullPath { get; private set; }

    internal static string PdfWarrantyCardWithoutPricesTestFolderRelativePath { get; } = GetConfigurationAppSettingsData("PdfWarrantyCardWithoutPricesTestFolder");
    internal static string PdfWarrantyCardWithoutPricesTestFolderFullPath { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private static string GetConfigurationConnectionStringData(string key)
    {
        ExeConfigurationFileMap configMap = new()
        {
            ExeConfigFilename = "./App.config"
        };

        System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

        return config.ConnectionStrings.ConnectionStrings[key].ConnectionString;
    }

    private static string GetConfigurationAppSettingsData(string key)
    {
        ExeConfigurationFileMap configMap = new()
        {
            ExeConfigFilename = "./App.config"
        };

        System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

        return config.AppSettings.Settings[key].Value;
    }
}