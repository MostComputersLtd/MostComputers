using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductImageFileManagement.Services;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using SixLabors.ImageSharp.Formats;

namespace MOSTComputers.Services.ProductImageFileManagement.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddProductImageFileManager(this IServiceCollection services, string imageDirectoryFullPath)
    {
        services.AddScoped<IProductImageFileManagementService, ProductImageFileManagementService>(serviceProvider =>
        {
            return new(imageDirectoryFullPath, serviceProvider.GetRequiredService<ITransactionalFileManager>());
        });

        return services;
    }
}
