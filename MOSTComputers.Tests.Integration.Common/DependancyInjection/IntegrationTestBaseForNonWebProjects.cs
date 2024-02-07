using Microsoft.Extensions.DependencyInjection;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Tests.Integration.Common.DependancyInjection;

public class IntegrationTestBaseForNonWebProjects : IAsyncLifetime
{
    public IntegrationTestBaseForNonWebProjects(string connString, RespawnerOptions? respawnerOptions = null)
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