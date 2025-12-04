using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.PromotionFileManagement.Services;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;

namespace MOSTComputers.Services.PromotionFileManagement.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddPromotionFileManager(this IServiceCollection services, string promotionFileDirectoryFullPath)
    {
        services.AddScoped<IPromotionFileManagementService, PromotionFileManagementService>(serviceProvider =>
        {
            return new(promotionFileDirectoryFullPath, serviceProvider.GetRequiredService<ITransactionalFileManager>());
        });

        return services;
    }

    public static IServiceCollection AddGroupPromotionFileManager(this IServiceCollection services, string promotionFileDirectoryFullPath)
    {
        services.AddScoped<IGroupPromotionFileManagementService, GroupPromotionFileManagementService>(serviceProvider =>
        {
            return new(promotionFileDirectoryFullPath, serviceProvider.GetRequiredService<ITransactionalFileManager>());
        });

        return services;
    }
}