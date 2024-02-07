using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.LocalChangesHandling.Services.BackgroundServices;

public sealed class LocalChangesCheckingBackgroundService : BackgroundService
{
    private const string _tableChangeNameOfProductsTable = "MOSTPRices";

    public LocalChangesCheckingBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMinutes(5));
    private readonly IServiceScopeFactory _scopeFactory;

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken)
            && !stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();

            IProductChangesService productChangesService = scope.ServiceProvider.GetRequiredService<IProductChangesService>();
            ILocalChangesService localChangesService = scope.ServiceProvider.GetRequiredService<ILocalChangesService>();

            IEnumerable<LocalChangeData> localChanges = localChangesService.GetAll();

            if (!localChanges.Any()) break;

            foreach (LocalChangeData localChange in localChanges)
            {
                if (localChange.TableName == _tableChangeNameOfProductsTable)
                {
                    productChangesService.HandleAnyOperation(localChange);
                }
            }
        }
    }
}