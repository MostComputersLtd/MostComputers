using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductRegister.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.DAL.Configuration.ConfigureServices;

namespace MOSTComputers.Services.ProductRegister.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddProductServices(this IServiceCollection services, string connectionString)
    {
        services.AddDataAccess(connectionString)
            .AddAllRepositories();

        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IManifacturerService, ManifacturerService>();
        services.AddTransient<IProductImageService, ProductImageService>();
        services.AddTransient<IProductImageFileNameInfoService, ProductImageFileNameInfoService>();
        services.AddTransient<IProductCharacteristicService, ProductCharacteristicService>();
        services.AddTransient<IProductPropertyService, ProductPropertyService>();
        services.AddTransient<IPromotionService, PromotionService>();
        services.AddTransient<IProductService, ProductService>();

        return services;
    }

}