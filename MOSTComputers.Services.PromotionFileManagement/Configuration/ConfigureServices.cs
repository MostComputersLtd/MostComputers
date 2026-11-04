using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.PromotionFileManagement.Services;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;

namespace MOSTComputers.Services.PromotionFileManagement.Configuration;
public static class ConfigureServices
{
    public const string PromotionFileManagerKey = "MOSTComputers.Services.PromotionFileManagement.Promotion.LocalFileManagementServiceKey";
    public const string GroupPromotionFileManagerKey = "MOSTComputers.Services.PromotionFileManagement.GroupPromotion.LocalFileManagementServiceKey";

    public static IServiceCollection AddPromotionFileManager(this IServiceCollection services, string promotionFileDirectoryFullPath)
    {
        services.AddKeyedScoped<ILocalFileManagementService, LocalFileManagementService>(PromotionFileManagerKey, (serviceProvider, _) =>
        {
            return new(promotionFileDirectoryFullPath, serviceProvider.GetRequiredService<ITransactionalFileManager>());
        });

        return services;
    }

    public static IServiceCollection AddGroupPromotionFileManager(this IServiceCollection services, string groupPromotionFileDirectoryFullPath)
    {
        services.AddKeyedScoped<ILocalFileManagementService, LocalFileManagementService>(GroupPromotionFileManagerKey, (serviceProvider, _) =>
        {
            return new(groupPromotionFileDirectoryFullPath, serviceProvider.GetRequiredService<ITransactionalFileManager>());
        });

        return services;
    }
}
