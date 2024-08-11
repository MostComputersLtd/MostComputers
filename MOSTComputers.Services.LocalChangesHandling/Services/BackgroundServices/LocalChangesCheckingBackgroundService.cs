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

    private CancellationTokenSource _immediateExecutionTokenSource = new();

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            CancellationTokenSource timerLinkedTokenSource
                = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _immediateExecutionTokenSource.Token);

            try
            {
                await _periodicTimer.WaitForNextTickAsync(timerLinkedTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            using IServiceScope scope = _scopeFactory.CreateScope();

            IProductChangesService productChangesService = scope.ServiceProvider.GetRequiredService<IProductChangesService>();
            ILocalChangesService localChangesService = scope.ServiceProvider.GetRequiredService<ILocalChangesService>();

            IEnumerable<LocalChangeData> localChanges = localChangesService.GetAll();

            if (!localChanges.Any())
            {
                if (_immediateExecutionTokenSource.Token.IsCancellationRequested)
                {
                    _immediateExecutionTokenSource.Dispose();
                    _immediateExecutionTokenSource = new();
                }

                continue;
            }

            foreach (LocalChangeData localChange in localChanges)
            {
                if (localChange.TableName == _tableChangeNameOfProductsTable)
                {
                    productChangesService.HandleAnyOperation(localChange);
                }
            }

            if (_immediateExecutionTokenSource.Token.IsCancellationRequested)
            {
                _immediateExecutionTokenSource.Dispose();
                _immediateExecutionTokenSource = new();
            }
        }
    }

    internal void ExecuteImmediately()
    {
        _immediateExecutionTokenSource.Cancel();
    }
}