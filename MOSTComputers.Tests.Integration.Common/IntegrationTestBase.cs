using Microsoft.AspNetCore.Mvc.Testing;
using Respawn;

namespace MOSTComputers.Tests.Integration.Common;

public class IntegrationTestBase<TProgramClass> : IAsyncLifetime
    where TProgramClass : class
{
    protected readonly string _connString;

    public IntegrationTestBase(string connString)
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

        _client = webAppFactory.CreateClient();
    }

    protected readonly HttpClient _client;

    protected Respawner _respawner = default!;

    public virtual async Task InitializeAsync()
    {
        _respawner = await Respawner.CreateAsync(_connString, new()
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