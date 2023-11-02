using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DAL.DAL;
using MOSTComputers.Services.DAL.DAL.Repositories;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Mapping;
using MOSTComputers.Services.Mapping;

namespace MOSTComputers.Services.DAL.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddTransient<IRelationalDataAccess, DapperDataAccess>(
            _ =>
            {
                DapperDataAccess dapperDataAccess = new(connectionString);

                return dapperDataAccess;
            });

        AddMappings();

        return services;
    }

    public static IServiceCollection AddAllRepositories(this IServiceCollection services)
    {

        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IManifacturerRepository, ManifacturerRepository>();
        services.AddTransient<IProductImageRepository, ProductImageRepository>();
        services.AddTransient<IProductImageFileNameInfoRepository, ProductImageFileNameInfoRepository>();
        services.AddTransient<IProductCharacteristicsRepository, ProductCharacteristicsRepository>();
        services.AddTransient<IProductPropertyRepository, ProductPropertyRepository>();
        services.AddTransient<IPromotionRepository, PromotionRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();

        return services;
    }

    private static void AddMappings()
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
        });
    }
}