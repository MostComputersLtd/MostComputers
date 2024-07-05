using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Identity.DAL;
using MOSTComputers.Services.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.Identity.Confuguration;

public static class Configuration
{
    public static IdentityBuilder AddCustomIdentity(this IServiceCollection services, string authenticationDBConnString)
    {
        services.AddDbContext<AuthenticationDBContext>(options =>
        {
            options.UseSqlServer(authenticationDBConnString);
        });

        IdentityBuilder identityBuilder = services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 5;
        })
        .AddEntityFrameworkStores<AuthenticationDBContext>();

        services.AddScoped<IIdentityService, IdentityService>();

        return identityBuilder;
    }
}