using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.Legacy.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddLegacyXmlServices(this IServiceCollection services)
    {
        services.TryAddScoped<XmlSerializerFactory>();

        services.TryAddScoped<ILegacyProductToXmlProductMappingService, LegacyProductToXmlProductMappingService>();

        services.TryAddScoped<ILegacyProductXmlValidationService, LegacyProductXmlValidationService>();

        services.TryAddScoped<ILegacyProductXmlService, LegacyProductXmlService>();

        return services;
    }

    public static IServiceCollection AddNewProductXmlServices(this IServiceCollection services)
    {
        services.TryAddScoped<XmlSerializerFactory>();

        services.TryAddScoped<IProductToXmlProductMappingService, ProductToXmlProductMappingService>();

        services.TryAddScoped<IProductXmlValidationService, ProductXmlValidationService>();

        services.TryAddScoped<IProductXmlService, ProductXmlService>();


        return services;
    }

    public static IServiceCollection AddGroupPromotionXmlServices(this IServiceCollection services)
    {
        services.TryAddScoped<IGroupPromotionXmlService, GroupPromotionXmlService>();

        return services;
    }

    public static IServiceCollection AddInvoiceXmlServices(this IServiceCollection services)
    {
        services.TryAddScoped<IInvoiceXmlService, InvoiceXmlService>();

        return services;
    }

    public static IServiceCollection AddWarrantyCardXmlServices(this IServiceCollection services)
    {
        services.TryAddScoped<IWarrantyCardXmlService, WarrantyCardXmlService>();

        return services;
    }

    public static IServiceCollection AddLegacyProductHtmlService(this IServiceCollection services)
    {
        services.TryAddScoped<ILegacyProductHtmlService, LegacyProductHtmlService>();

        return services;
    }

    public static IServiceCollection AddNewProductHtmlService(this IServiceCollection services, string fullPathToProductXslFile)
    {
        services.TryAddScoped<XmlSerializerFactory>();

        services.AddScoped<IProductHtmlService, ProductHtmlService>(serviceProvider =>
        {
            return new(
                serviceProvider.GetRequiredService<XmlSerializerFactory>(),
                fullPathToProductXslFile);
        });

        return services;
    }
}