using Respawn;

namespace MOSTComputers.Tests.Integration.Common.DependancyInjection;

public class IntegrationTestBaseForNonWebProjectsWithDBReset : IAsyncLifetime
{
    public IntegrationTestBaseForNonWebProjectsWithDBReset(string connString, RespawnerOptions? respawnerOptions = null)
    {
        _connString = connString;
        _respawnerOptions = respawnerOptions;
    }

    protected Respawner _respawner = default!;
    protected string _connString;
    private readonly RespawnerOptions? _respawnerOptions;

    public virtual async Task InitializeAsync()
    {
        _respawner = await Respawner.CreateAsync(
            _connString,
            _respawnerOptions ?? new()
            {
                DbAdapter = DbAdapter.SqlServer
            });
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connString);
    }
}