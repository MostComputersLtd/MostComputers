using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.SearchStringOrigin.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddSearchStringOriginService(this IServiceCollection services)
    {
        services.AddTransient<ISearchStringOriginService, SearchStringOriginService>();

        return services;
    }
}