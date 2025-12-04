using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.ProductImageFileManagement.Services;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.TransactionalFileManagement.Services.Contracts;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MOSTComputers.Services.ProductImageFileManagement.Configuration;
public static class ConfigureServices
{
    public static IServiceCollection AddProductImageFileManager(this IServiceCollection services, string imageDirectoryFullPath)
    {
        services.Configure<DecoderOptions>(config =>
        {
            //config.Configuration.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder());
        });

        services.AddScoped<IProductImageFileManagementService, ProductImageFileManagementService>(serviceProvider =>
        {
            return new(imageDirectoryFullPath, serviceProvider.GetRequiredService<ITransactionalFileManager>());
        });

        return services;
    }
}