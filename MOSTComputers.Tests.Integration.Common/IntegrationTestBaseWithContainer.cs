namespace MOSTComputers.Tests.Integration.Common;

public class IntegrationTestBaseWithContainer<TEntryPoint, TWebApplicationFactoryWithContainer> : IAsyncLifetime
    where TEntryPoint : class
    where TWebApplicationFactoryWithContainer : WebApplicationFactoryWithContainer<TEntryPoint>
{
    public IntegrationTestBaseWithContainer(TWebApplicationFactoryWithContainer webApplicationFactory)
    {
        client = webApplicationFactory.get_client;
        _resetDatabaseAction = webApplicationFactory.ResetDatabaseAsync;
    }

    protected readonly HttpClient client;
    protected readonly Func<Task> _resetDatabaseAction;

    protected async Task ResetDatabaseAsync() => await _resetDatabaseAction();

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }
}