using Dapper.FluentMap;
using Dapper.FluentMap.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DAL.DAL;
using MOSTComputers.Services.DAL.DAL.Repositories;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Services.DAL.DAL.Repositories.ExternalXmlImport;
using MOSTComputers.Services.DAL.Mapping;
using MOSTComputers.Services.DAL.Mapping.ExternalXmlImport;

namespace MOSTComputers.Services.DAL.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IRelationalDataAccess, DapperDataAccess>(
            _ =>
            {
                DapperDataAccess dapperDataAccess = new(connectionString);

                return dapperDataAccess;
            });

        AddDapperMappings();

        return services;
    }

    public static IServiceCollection AddAllRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IManifacturerRepository, ManifacturerRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductImageFileNameInfoRepository, ProductImageFileNameInfoRepository>();
        services.AddScoped<IProductCharacteristicsRepository, ProductCharacteristicsRepository>();
        services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductStatusesRepository, ProductStatusesRepository>();
        services.AddScoped<ILocalChangesRepository, LocalChangesRepository>();
        services.AddScoped<IExternalChangesRepository, ExternalChangesRepository>();

        services.AddScoped<IToDoLocalChangesRepository, ToDoLocalChangesRepository>();

        services.AddScoped<IProductWorkStatusesRepository, ProductWorkStatusesRepository>();

        services.AddScoped<IFailedPropertyNameOfProductRepository, FailedPropertyNameOfProductRepository>();
        services.AddScoped<IProductCharacteristicAndExternalXmlDataRelationRepository, ProductCharacteristicAndExternalXmlDataRelationRepository>();

        AddExternalXmlImportRepositories(services);

        return services;
    }

    private static void AddExternalXmlImportRepositories(IServiceCollection services)
    {
        services.AddScoped<IXmlImportProductPropertyRepository, XmlImportProductPropertyRepository>();
        services.AddScoped<IXmlImportProductImageFileNameInfoRepository, XmlImportProductImageFileNameInfoRepository>();
        services.AddScoped<IXmlImportProductImageRepository, XmlImportProductImageRepository>();
    }

    private static void AddDapperMappings()
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new CategoryEntityMap());
            config.AddMap(new ManifacturerEntityMap());
            config.AddMap(new ProductCharacteristicEntityMap());
            config.AddMap(new ProductPropertyEntityMap());
            config.AddMap(new ProductImageEntityMap());
            config.AddMap(new ProductFirstImageEntityMap());
            config.AddMap(new ProductImageFileNameInfoEntityMap());
            config.AddMap(new ProductEntityMap());
            config.AddMap(new PromotionEntityMap());
            config.AddMap(new ProductStatusesEntityMap());
            config.AddMap(new LocalChangeDataEntityMap());
            config.AddMap(new ExternalChangeDataEntityMap());

            config.AddMap(new ProductWorkStatusesEntityMap());

            config.AddMap(new FailedPropertyNameOfProductEntityMap());
            config.AddMap(new ProductCharacteristicAndExternalXmlDataRelationEntityMap());

            AddExternalXmlImportDapperMappings(config);
        });
    }

    private static void AddExternalXmlImportDapperMappings(FluentMapConfiguration config)
    {
        config.AddMap(new XmlImportProductPropertyEntityMap());
        config.AddMap(new XmlImportProductImageFileNameInfoEntityMap());
        config.AddMap(new XmlImportProductFirstImageEntityMap());
        config.AddMap(new XmlImportProductImageEntityMap());
    }
}