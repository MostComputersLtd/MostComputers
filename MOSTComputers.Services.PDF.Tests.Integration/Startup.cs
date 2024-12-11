using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Microsoft.Extensions.Hosting;
using static MOSTComputers.Services.PDF.Configuration.ConfigureServices;
using MOSTComputers.Services.PDF.Configuration;

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

        string pdfTemplateFilePath = PdfTemplateFileRelativePath;

        if (!Path.IsPathFullyQualified(pdfTemplateFilePath))
        {
            pdfTemplateFilePath = Path.GetFullPath(pdfTemplateFilePath, projectFolderPath)
                .Replace('\\', '/');
        }

        PdfTemplateFileFullPath = pdfTemplateFilePath;

        string htmlTemplateFilePath = HtmlTemplateFileRelativePath;

        if (!Path.IsPathFullyQualified(htmlTemplateFilePath))
        {
            htmlTemplateFilePath = Path.GetFullPath(htmlTemplateFilePath, projectFolderPath)
                .Replace('\\', '/');
        }

        HtmlTemplateFileFullPath = htmlTemplateFilePath;

        //services.AddPdfInvoiceServices(PdfTemplateFileFullPath);
        services.AddHtmlInvoiceServices(HtmlTemplateFileFullPath);

        string pdfInvoicesTestFolderFilePath = PdfInvoiceTestFolderRelativePath;

        if (!Path.IsPathFullyQualified(pdfInvoicesTestFolderFilePath))
        {
            pdfInvoicesTestFolderFilePath = Path.GetFullPath(pdfInvoicesTestFolderFilePath, projectFolderPath)
                .Replace('\\', '/');
        }

        PdfInvoiceTestFolderFullPath = pdfInvoicesTestFolderFilePath;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    internal static string PdfTemplateFileRelativePath { get; } = GetConfigurationAppSettingsData("PdfInvoiceTemplate");
    internal static string PdfTemplateFileFullPath { get; private set; }

    internal static string HtmlTemplateFileRelativePath { get; } = GetConfigurationAppSettingsData("HtmlInvoiceTemplate");
    internal static string HtmlTemplateFileFullPath { get; private set; }

    internal static string PdfInvoiceTestFolderRelativePath { get; } = GetConfigurationAppSettingsData("PdfInvoiceTestFolder");
    internal static string PdfInvoiceTestFolderFullPath { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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