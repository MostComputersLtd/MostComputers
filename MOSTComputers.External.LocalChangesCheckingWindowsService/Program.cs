using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using static MOSTComputers.Services.LocalChangesHandling.Configuration.ConfigureServices;

const string _windowsServiceName = "MOSTComputers.External.LocalChangesHandlingService";

const string _appsettingsFileName = "appsettings.json";
//const string _connectionStringNameInConfigFile = "MostDBNew";

ConfigurationBuilder configurationBuilder = new();

configurationBuilder
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(_appsettingsFileName, optional: false, reloadOnChange: true);

IConfiguration configuration = configurationBuilder.Build();

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(config =>
    {
        config.ServiceName = _windowsServiceName;
    })
    .ConfigureServices(services =>
    {
        services.AddLocalChangesHandlingBackgroundService();
    })
    .Build();

host.Run();