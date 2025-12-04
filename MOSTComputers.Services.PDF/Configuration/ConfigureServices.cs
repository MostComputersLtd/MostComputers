using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.PDF.Services;
using MOSTComputers.Services.PDF.Services.Contracts;
using PuppeteerSharp;

namespace MOSTComputers.Services.PDF.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddPdfInvoiceServices(this IServiceCollection services, string htmlInvoiceTemplateFilePath)
    {
        services.TryAddPdfBrowserProviderService();

        services.AddScoped<IPdfInvoiceFileGeneratorService, PdfInvoiceFileGeneratorServiceWithHtmlTemplate>(serviceProvider =>
        {
            PdfInvoiceFileGeneratorServiceWithHtmlTemplate pdfInvoiceService = new(htmlInvoiceTemplateFilePath,
                serviceProvider.GetRequiredService<IBrowserProviderService>());

            return pdfInvoiceService;
        });

        return services;
    }

    public static IServiceCollection AddPdfInvoiceGeneratorFromDataServices(this IServiceCollection services, string htmlInvoiceTemplateFilePath)
    {
        services.TryAddPdfBrowserProviderService();

        services.TryAddScoped<IPdfInvoiceDataService, PdfInvoiceDataService>();

        services.AddScoped<IPdfInvoiceFileGeneratorService, PdfInvoiceFileGeneratorServiceWithHtmlTemplate>(serviceProvider =>
        {
            PdfInvoiceFileGeneratorServiceWithHtmlTemplate pdfInvoiceService = new(htmlInvoiceTemplateFilePath,
                serviceProvider.GetRequiredService<IBrowserProviderService>());

            return pdfInvoiceService;
        });

        return services;
    }

    public static IServiceCollection AddPdfWarrantyCardWithoutPricesServices(
        this IServiceCollection services, string htmlWarrantyCardWithoutPricesTemplateFilePath)
    {
        services.TryAddPdfBrowserProviderService();

        services.AddScoped<IPdfWarrantyCardWithoutPricesFileGeneratorService, PdfWarrantyCardWithoutPricesFileGeneratorServiceWithHtmlTemplate>(serviceProvider =>
        {
            PdfWarrantyCardWithoutPricesFileGeneratorServiceWithHtmlTemplate pdfWarrantyCardService = new(
                htmlWarrantyCardWithoutPricesTemplateFilePath,
                serviceProvider.GetRequiredService<IBrowserProviderService>());

            return pdfWarrantyCardService;
        });

        return services;
    }

    public static IServiceCollection AddPdfWarrantyCardWithoutPricesGeneratorFromDataServices(
        this IServiceCollection services, string htmlWarrantyCardWithoutPricesTemplateFilePath)
    {
        services.TryAddPdfBrowserProviderService();

        services.TryAddScoped<IPdfWarrantyCardDataService, PdfWarrantyCardDataService>();

        services.AddScoped<IPdfWarrantyCardWithoutPricesFileGeneratorService, PdfWarrantyCardWithoutPricesFileGeneratorServiceWithHtmlTemplate>(serviceProvider =>
        {
            PdfWarrantyCardWithoutPricesFileGeneratorServiceWithHtmlTemplate pdfWarrantyCardService = new(
                htmlWarrantyCardWithoutPricesTemplateFilePath,
                serviceProvider.GetRequiredService<IBrowserProviderService>());

            return pdfWarrantyCardService;
        });

        return services;
    }

    public static IServiceCollection AddPdfWarrantyCardWithPricesServices(
        this IServiceCollection services, string htmlWarrantyCardWithPricesTemplateFilePath)
    {
        services.TryAddPdfBrowserProviderService();

        services.AddScoped<IPdfWarrantyCardWithPricesFileGeneratorService, PdfWarrantyCardWithPricesFileGeneratorServiceWithHtmlTemplate>(serviceProvider =>
        {
            PdfWarrantyCardWithPricesFileGeneratorServiceWithHtmlTemplate pdfWarrantyCardService = new(
                htmlWarrantyCardWithPricesTemplateFilePath,
                serviceProvider.GetRequiredService<IBrowserProviderService>());

            return pdfWarrantyCardService;
        });

        return services;
    }

    public static IServiceCollection AddPdfWarrantyCardWithPricesGeneratorFromDataServices(
        this IServiceCollection services, string htmlWarrantyCardWithPricesTemplateFilePath)
    {
        services.TryAddPdfBrowserProviderService();

        services.TryAddScoped<IPdfWarrantyCardDataService, PdfWarrantyCardDataService>();

        services.AddScoped<IPdfWarrantyCardWithPricesFileGeneratorService, PdfWarrantyCardWithPricesFileGeneratorServiceWithHtmlTemplate>(serviceProvider =>
        {
            PdfWarrantyCardWithPricesFileGeneratorServiceWithHtmlTemplate pdfWarrantyCardService = new(
                htmlWarrantyCardWithPricesTemplateFilePath,
                serviceProvider.GetRequiredService<IBrowserProviderService>());

            return pdfWarrantyCardService;
        });

        return services;
    }

    private static IServiceCollection TryAddPdfBrowserProviderService(this IServiceCollection services)
    {
        ServiceDescriptor? existingBrowserProviderService = services.FirstOrDefault(serviceDescriptor =>
            serviceDescriptor.ServiceType == typeof(IBrowserProviderService));

        if (existingBrowserProviderService is not null) return services;

        services.AddSingleton<IBrowserProviderService, BrowserProviderService>(serviceProvider =>
        {
            return new BrowserProviderService(new LaunchOptions
            {
                Headless = true,
                Args =
                [
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-gpu",
                    "--disable-extensions",
                    "--disable-background-networking",
                    "--disable-sync",
                    "--disable-translate",
                    "--disable-default-apps",
                    "--mute-audio"
                ]
            });
        });

        return services;
    }
}