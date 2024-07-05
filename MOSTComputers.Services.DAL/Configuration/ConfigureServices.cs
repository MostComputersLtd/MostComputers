using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DAL.DAL;
using MOSTComputers.Services.DAL.DAL.Repositories;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Mapping;

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

        AddMappings();

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

        services.AddScoped<IFailedPropertyNameOfProductRepository, FailedPropertyNameOfProductRepository>();

        services.AddScoped<IProductWorkStatusesRepository, ProductWorkStatusesRepository>();

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
            config.AddMap(new ProductStatusesEntityMap());
            config.AddMap(new LocalChangeDataEntityMap());
            config.AddMap(new ExternalChangeDataEntityMap());

            config.AddMap(new FailedPropertyNameOfProductEntityMap());

            config.AddMap(new ProductWorkStatusesEntityMap());
        });
    }
}