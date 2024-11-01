using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Identity.DAL;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Identity.Services;

namespace MOSTComputers.Services.Identity.Confuguration;

public static class Configuration
{
    public static IdentityBuilder AddCustomIdentity(this IServiceCollection services, string authenticationDBConnString)
    {
        services.AddDbContext<DefaultAuthenticationDBContext>(options =>
        {
            options.UseSqlServer(authenticationDBConnString);
        });

        IdentityBuilder identityBuilder = services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 5;
        })
        .AddEntityFrameworkStores<DefaultAuthenticationDBContext>();

        services.AddScoped<IIdentityService, IdentityService>();

        return identityBuilder;
    }

    public static IdentityBuilder AddCustomIdentityWithPasswordsTableOnly(this IServiceCollection services, string authenticationDBConnString)
    {
        services.AddDbContext<PasswordsTableOnlyAuthenticationDBContext>(options =>
        {
            options.UseSqlServer(authenticationDBConnString);
        });

        IdentityBuilder identityBuilder = services.AddIdentityCore<PasswordsTableOnlyUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 5;
        })
            .AddUserStore<PasswordsTableOnlyUserStore>()
            .AddEntityFrameworkStores<PasswordsTableOnlyAuthenticationDBContext>();

        services.AddScoped<IPasswordsTableOnlyIdentityService, PasswordsTableOnlyIdentityService>();

        return identityBuilder;
    }
}