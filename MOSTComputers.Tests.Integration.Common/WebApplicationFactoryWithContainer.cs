using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using System.Data.Common;

namespace MOSTComputers.Tests.Integration.Common;

public abstract class WebApplicationFactoryWithContainer<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    protected WebApplicationFactoryWithContainer()
    {
        get_client = CreateClient();
        _dbConnection = new SqlConnection
        {
            ConnectionString = get_connectionString
        };
    }

    public HttpClient get_client { get; init; }

    protected abstract string get_connectionString { get; }
    private readonly DbConnection _dbConnection;
    protected Respawner respawner = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

    }

    public virtual async Task ResetDatabaseAsync()
    {
        await respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _dbConnection.OpenAsync();

        respawner = await Respawner.CreateAsync(_dbConnection, new()
        {
            DbAdapter = DbAdapter.SqlServer
        });
    }
}