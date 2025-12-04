using Microsoft.AspNetCore.Mvc.Testing;
using Respawn;

namespace MOSTComputers.Tests.Integration.Common;
public abstract class SingleIntegrationTestBaseForWebProjects<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    protected SingleIntegrationTestBaseForWebProjects(string connectionString, WebApplicationFactory<TEntryPoint>? webApplicationFactory = null, RespawnerOptions? respawnerOptions = null)
    {
        _connectionString = connectionString;

        webApplicationFactory ??= new();

        get_client = webApplicationFactory.CreateClient();

        _respawnerOptions = respawnerOptions ?? new()
        {
            DbAdapter = DbAdapter.SqlServer
        };
    }

    protected HttpClient get_client { get; init; }
    protected Respawner respawner = default!;
    private readonly RespawnerOptions _respawnerOptions;
    private readonly string _connectionString;

    [Fact]
    public async Task RunTestWrapperAsync()
    {
        RunTest();

        await ResetDatabaseAsync();
    }

    protected async Task ResetDatabaseAsync()
    {
        await respawner.ResetAsync(_connectionString);
    }

    protected abstract void RunTest();

    public async Task InitializeAsync()
    {
        respawner = await Respawner.CreateAsync(_connectionString, _respawnerOptions);
    }

    public async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }
}