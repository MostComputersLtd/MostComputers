using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.PDF.Services;
using MOSTComputers.Services.PDF.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.PDF.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddPdfInvoiceServices(this IServiceCollection services, string pdfInvoiceTemplateFilePath)
    {
        services.TryAddScoped<PdfBasicOperationsService>();
        services.AddScoped<IPdfInvoiceService, PdfInvoiceService>(config =>
        {
            PdfInvoiceService pdfInvoiceService = new(config.GetRequiredService<PdfBasicOperationsService>(), pdfInvoiceTemplateFilePath);

            return pdfInvoiceService;
        });

        return services;
    }

    public static IServiceCollection AddHtmlInvoiceServices(this IServiceCollection services, string htmlInvoiceTemplateFilePath)
    {
        services.TryAddScoped<PdfBasicOperationsService>();
        services.AddScoped<IPdfInvoiceService, PdfInvoiceServiceWithHtmlTemplate>(config =>
        {
            PdfInvoiceServiceWithHtmlTemplate pdfInvoiceService = new(htmlInvoiceTemplateFilePath);

            return pdfInvoiceService;
        });

        return services;
    }
}