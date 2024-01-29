using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.XMLDataOperations.Services;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Services.Mapping;
using System.Xml.Serialization;

namespace MOSTComputers.Services.XMLDataOperations.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddXmlDeserializeService(this IServiceCollection services)
    {
        services.AddScoped<XmlSerializerFactory>();

        services.AddScoped<IProductToXmlProductMappingService, ProductToXmlProductMappingService>();

        services.AddScoped<IProductDeserializeService, ProductDeserializeService>();

        return services;
    }
}