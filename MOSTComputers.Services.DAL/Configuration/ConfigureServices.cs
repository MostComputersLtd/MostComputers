using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.DAL.DAL;
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

                AddMappings(dapperDataAccess);

                return dapperDataAccess;
            });

        return services;
    }

    private static void AddMappings(DapperDataAccess relationalDataAccess)
    {
        relationalDataAccess.AddCustomEntityMap(new ProductEntityMap());
        relationalDataAccess.AddCustomEntityMap(new ManifacturerEntityMap());
        relationalDataAccess.AddCustomEntityMap(new CategoryEntityMap());
        relationalDataAccess.AddCustomEntityMap(new ProductCharacteristicEntityMap());
        relationalDataAccess.AddCustomEntityMap(new ProductPropertyEntityMap());
        relationalDataAccess.AddCustomEntityMap(new ProductImageEntityMap());
        relationalDataAccess.AddCustomEntityMap(new ProductFirstImageEntityMap());
        relationalDataAccess.AddCustomEntityMap(new ProductImageFileNameInfoEntityMap());
    }
}