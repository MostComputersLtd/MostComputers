using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Identity.DAL;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Identity.Services;
using MOSTComputers.Services.Identity.Services.Cached;

namespace MOSTComputers.Services.Identity.Confuguration;
public static class Configuration
{
    public const string IdentityServiceKey = "MOSTComputers.Services.Identity.IdentityService";

    public static IdentityBuilder AddCustomIdentityWithPasswordsTableOnly(this IServiceCollection services, string authenticationDBConnString)
    {
        services.AddDbContext<PasswordsTableOnlyAuthenticationDBContext>(options =>
        {
            options.UseSqlServer(authenticationDBConnString);
        });

        IdentityBuilder identityBuilder = services.AddIdentityCore<PasswordsTableOnlyUser>()
            .AddRoles<PasswordsTableOnlyRole>()
            .AddEntityFrameworkStores<PasswordsTableOnlyAuthenticationDBContext>();

        services.AddKeyedScoped<IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole>, PasswordsTableOnlyIdentityService>(IdentityServiceKey);

        services.AddScoped<IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole>, CachedPasswordsTableOnlyIdentityService>();

        return identityBuilder;
    }
}