using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;
using MOSTComputers.Services.DataAccess.Documents.DataAccess;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;

namespace MOSTComputers.Services.DataAccess.Documents.Configuration;
public static class ConfigureServices
{
    internal const string DocumentsDataAccessServiceKey = "MOSTComputers.Services.DataAccess.Documents.DocumentsDB";

    public static IServiceCollection AddDocumentRepositories(this IServiceCollection services, string replicationDBConnectionString)
    {
        services.AddKeyedScoped<IConnectionStringProvider, ConnectionStringProvider>(DocumentsDataAccessServiceKey,
            (_, _) => new ConnectionStringProvider(replicationDBConnectionString));

        AddDapperMappings();

        services.TryAddScoped<IInvoiceItemRepository, InvoiceItemRepository>();
        services.TryAddScoped<IInvoiceRepository, InvoiceRepository>();

        services.TryAddScoped<IWarrantyCardItemRepository, WarrantyCardItemRepository>();
        services.TryAddScoped<IWarrantyCardRepository, WarrantyCardRepository>();

        services.TryAddScoped<IFirmDataRepository, FirmDataRepository>();

        return services;
    }

    private static void AddDapperMappings()
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new InvoiceItemDAOEntityMap());
            config.AddMap(new InvoiceDAOEntityMap());
            config.AddMap(new InvoiceCustomerDataEntityMap());

            config.AddMap(new WarrantyCardItemDAOEntityMap());
            config.AddMap(new WarrantyCardDAOEntityMap());
            config.AddMap(new WarrantyCardCustomerDataEntityMap());

            config.AddMap(new FirmDataEntityMap());
        });
    }
}