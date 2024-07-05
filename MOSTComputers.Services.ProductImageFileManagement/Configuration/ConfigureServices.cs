using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductImageFileManagement.Services;

namespace MOSTComputers.Services.ProductImageFileManagement.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddProductImageFileManagement(this IServiceCollection services, string imageDirectoryFullPath)
    {
        services.AddTransient<IProductImageFileManagementService, ProductImageFileManagementService>(_ =>
        {
            return new ProductImageFileManagementService(imageDirectoryFullPath);
        });

        return services;
    }
}