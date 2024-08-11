using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Mapping;
using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddXmlDeserializer(this IServiceCollection services)
    {
        services.TryAddScoped<XmlSerializerFactory>();

        services.TryAddScoped<IProductToXmlProductMappingService, ProductToXmlProductMappingService>();

        services.TryAddScoped<IProductDeserializeService, ProductDeserializeService>();

        services.TryAddScoped<IProductHtmlService, ProductHtmlService>();

        return services;
    }

    public static IServiceCollection AddProductHtmlService(this IServiceCollection services)
    {
        services.AddXmlDeserializer();

        services.AddScoped<IProductHtmlService, ProductHtmlService>();

        return services;
    }
}