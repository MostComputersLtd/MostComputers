using Dapper.FluentMap;
using Dapper.FluentMap.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ExternalXmlImport;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ExternalXmlImport;
using MOSTComputers.Services.DataAccess.Products.DataAccess;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Counters;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Counters.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ExternalXmlImport.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Counters;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductIdentifiers;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductProperties;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductImages;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Files;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.ProductIdentifiers;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;

namespace MOSTComputers.Services.DataAccess.Products.Configuration;
public static class ConfigureServices
{
    public const string LocalDBConnectionStringProviderServiceKey = "MOSTComputers.Services.DataAccess.ConnectionStringProvider.LocalDB";
    public const string OriginalDBConnectionStringProviderServiceKey = "MOSTComputers.Services.DataAccess.ConnectionStringProvider.OriginalDB";

    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString, string? readDBConnectionString = null)
    {
        services.AddConnectionStringProvider(connectionString, LocalDBConnectionStringProviderServiceKey);

        if (!string.IsNullOrWhiteSpace(readDBConnectionString))
        {
            services.AddConnectionStringProvider(readDBConnectionString, OriginalDBConnectionStringProviderServiceKey);
        }

        AddDapperMappings();

        return services;
    }

    public static IServiceCollection AddConnectionStringProvider(this IServiceCollection services, string connectionString, string connectionStringKey)
    {
        services.AddKeyedScoped<IConnectionStringProvider, ConnectionStringProvider>(connectionStringKey, (_, _) =>
        {
            ConnectionStringProvider dapperDataAccess = new(connectionString);

            return dapperDataAccess;
        });

        return services;
    }

    public static IServiceCollection AddAllRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
        services.AddScoped<IManufacturerRepository, ManufacturerRepository>();

        services.AddScoped<IProductImageRepository, ProductImageRepository>();

        services.AddScoped<IProductCharacteristicsRepository, ProductCharacteristicsRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IOriginalLocalChangesReadRepository, OriginalLocalChangesReadRepository>();

        services.AddScoped<IProductImageFileDataRepository, ProductImageFileDataRepository>();
        services.AddScoped<IProductPropertyRepository, ProductPropertyRepository>();

        services.AddScoped<IPromotionRepository, PromotionRepository>();

        services.AddScoped<IPromotionGroupsRepository, PromotionGroupsRepository>();
        services.AddScoped<IManufacturerToPromotionGroupRelationsRepository, ManufacturerToPromotionGroupRelationsRepository>();
        services.AddScoped<IGroupPromotionContentsRepository, GroupPromotionContentsRepository>();
        services.AddScoped<IGroupPromotionImagesRepository, GroupPromotionImagesRepository>();
        services.AddScoped<IGroupPromotionImageFileDatasRepository, GroupPromotionImageFileDatasRepository>();

        services.AddScoped<IPromotionFileInfoRepository, PromotionFileInfoRepository>();
        services.AddScoped<IPromotionProductFileInfoRepository, PromotionProductFileInfoRepository>();

        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();

        //services.AddScoped<IProductStatusesRepository, ProductStatusesRepository>();

        services.AddScoped<IToDoLocalChangesRepository, ToDoLocalChangesRepository>();

        services.AddScoped<IProductWorkStatusesRepository, ProductWorkStatusesRepository>();

        services.AddScoped<IProductGTINCodeRepository, ProductGTINCodeRepository>();

        services.AddScoped<IProductSerialNumberRepository, ProductSerialNumberRepository>();

        services.AddScoped<ISystemCountersRepository, SystemCountersRepository>();

        AddExternalXmlImportRepositories(services);

        return services;
    }

    private static void AddExternalXmlImportRepositories(IServiceCollection services)
    {
        services.AddScoped<IProductCharacteristicAndExternalXmlDataRelationRepository, ProductCharacteristicAndExternalXmlDataRelationRepository>();
        services.AddScoped<ProductCharacteristicAndImageHtmlRelationRepository>();
    }

    private static void AddDapperMappings()
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new CategoryEntityMap());
            config.AddMap(new SubCategoryEntityMap());

            config.AddMap(new ManufacturerEntityMap());

            config.AddMap(new ProductCharacteristicEntityMap());

            config.AddMap(new ProductPropertyEntityMap());
            config.AddMap(new ProductPropertiesForProductCountDataEntityMap());

            config.AddMap(new ProductImageEntityMap());
            config.AddMap(new ProductFirstImageEntityMap());

            config.AddMap(new ProductImageWithoutFileDataEntityMap());
            config.AddMap(new ProductImagesForProductCountDataEntityMap());

            config.AddMap(new ProductImageFileDataEntityMap());

            config.AddMap(new ProductDAOEntityMap());

            config.AddMap(new PromotionEntityMap());

            config.AddMap(new PromotionGroupEntityMap());
            config.AddMap(new ManufacturerToPromotionGroupRelationEntityMap());
            config.AddMap(new GroupPromotionContentEntityMap());
            config.AddMap(new GroupPromotionImageEntityMap());
            config.AddMap(new GroupPromotionImageWithoutFileEntityMap());
            config.AddMap(new GroupPromotionImageFileDataEntityMap());

            config.AddMap(new PromotionFileInfoEntityMap());
            config.AddMap(new PromotionProductFileInfoDAOEntityMap());
            config.AddMap(new PromotionProductFileInfoForProductCountDataEntityMap());

            config.AddMap(new ProductStatusesEntityMap());
            config.AddMap(new ProductWorkStatusesEntityMap());

            config.AddMap(new ProductGTINCodeDAOEntityMap());
            config.AddMap(new ProductSerialNumberEntityMap());

            config.AddMap(new LocalChangeDataEntityMap());

            config.AddMap(new ExchangeRateEntityMap());
            config.AddMap(new SystemCountersEntityMap());
            
            AddExternalXmlImportDapperMappings(config);
        });
    }

    private static void AddExternalXmlImportDapperMappings(FluentMapConfiguration config)
    {
        config.AddMap(new ProductCharacteristicAndExternalXmlDataRelationEntityMap());
        config.AddMap(new ProductCharacteristicAndImageHtmlRelationEntityMap());
    }
}