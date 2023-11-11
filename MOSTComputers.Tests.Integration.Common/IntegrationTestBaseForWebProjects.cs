using Microsoft.AspNetCore.Mvc.Testing;
using Respawn;

namespace MOSTComputers.Tests.Integration.Common;

public class IntegrationTestBaseForWebProjects<TProgramClass> : IAsyncLifetime
    where TProgramClass : class
{
    public IntegrationTestBaseForWebProjects(string connString, RespawnerOptions? respawnerOptions = null)
    {
        WebApplicationFactory<TProgramClass> webAppFactory = new();
        //.WithWebHostBuilder(builder =>
        //{
        //	builder.ConfigureServices(services =>
        //	{
        //		services.RemoveAll<RecipeContext>();
        //		services.AddDbContext<RecipeContext>(options => options.UseInMemoryDatabase("Whisknflour.Services.RecipesRegister.Tests.Integration"));
        //	});
        //}
        //);

        _connString = connString;
        _respawnerOptions = respawnerOptions;
        _client = webAppFactory.CreateClient();
    }

    protected readonly HttpClient _client;

    protected readonly string _connString;

    protected Respawner _respawner = default!;

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

    public virtual async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    protected virtual async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connString);
    }
}