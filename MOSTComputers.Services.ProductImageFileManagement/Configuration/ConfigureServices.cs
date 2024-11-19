using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductImageFileManagement.Services;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;

namespace MOSTComputers.Services.ProductImageFileManagement.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddProductImageFileManagement(this IServiceCollection services, string imageDirectoryFullPath)
    {
        services.AddSingleton<IEnlistmentManager, EnlistmentManager>();

        services.AddScoped<ITransactionalFileManager, TransactionalFileManager>(serviceProvider =>
        {
            return new(serviceProvider.GetRequiredService<IEnlistmentManager>());
        });

        services.AddScoped<IProductImageFileManagementService, ProductImageFileManagementService>(serviceProvider =>
        {
            return new(imageDirectoryFullPath, serviceProvider.GetRequiredService<ITransactionalFileManager>());
        });

        return services;
    }
}