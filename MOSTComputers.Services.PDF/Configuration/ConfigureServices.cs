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
    public static IServiceCollection AddPdfInvoiceServices(this IServiceCollection services, string htmlInvoiceTemplateFilePath)
    {
        services.AddScoped<IPdfInvoiceService, PdfInvoiceServiceWithHtmlTemplate>(config =>
        {
            PdfInvoiceServiceWithHtmlTemplate pdfInvoiceService = new(htmlInvoiceTemplateFilePath);

            return pdfInvoiceService;
        });

        return services;
    }
}