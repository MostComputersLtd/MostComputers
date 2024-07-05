using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.LocalChangesHandling.Services.BackgroundServices;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;

namespace MOSTComputers.Services.LocalChangesHandling.Services;
internal sealed class LocalChangesCheckingImmediateExecutionService : ILocalChangesCheckingImmediateExecutionService
{
    private readonly LocalChangesCheckingBackgroundService _localChangesCheckingBackgroundService;

    public LocalChangesCheckingImmediateExecutionService(LocalChangesCheckingBackgroundService localChangesCheckingBackgroundService)
    {
        _localChangesCheckingBackgroundService = localChangesCheckingBackgroundService;
    }

    public void ExecuteImmediately()
    {
        _localChangesCheckingBackgroundService.ExecuteImmediately();
    }
}