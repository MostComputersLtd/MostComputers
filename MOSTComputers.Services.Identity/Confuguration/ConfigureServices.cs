using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Services.Identity.DAL;
using MOSTComputers.Services.Identity.DAL.Contracts;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Identity.Services;
using MOSTComputers.Services.Identity.Services.Cached;
using OpenIddict.EntityFrameworkCore.Models;

namespace MOSTComputers.Services.Identity.Confuguration;
public static class ConfigureServices
{
    public const string IdentityServiceKey = "MOSTComputers.Services.Identity.IdentityService";

    public static IdentityBuilder AddCustomIdentityWithPasswordsTableOnly(this IServiceCollection services, string authenticationDBConnString)
    {
        //services.AddDbContextFactory<PasswordsTableOnlyAuthenticationDBContext>(options =>
        //{
        //    options.UseSqlServer(authenticationDBConnString);
        //});

        services.AddDbContext<PasswordsTableOnlyAuthenticationDBContext>(options =>
        {
            options.UseSqlServer(authenticationDBConnString);

            options.UseOpenIddict<
                ApiApplication,
                ApiAuthorization,
                ApiScope,
                ApiToken,
                string>();
        });

        IdentityBuilder identityBuilder = services
            .AddIdentityCore<PasswordsTableOnlyUser>()
            .AddRoles<PasswordsTableOnlyRole>()
            .AddEntityFrameworkStores<PasswordsTableOnlyAuthenticationDBContext>();

        services.AddKeyedScoped<IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole>, PasswordsTableOnlyIdentityService>(IdentityServiceKey);

        services.AddScoped<IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole>, CachedPasswordsTableOnlyIdentityService>();

        return identityBuilder;
    }

    public static OpenIddictBuilder AddOpenIddictToDatabase(this IServiceCollection services)
    {
        return services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<PasswordsTableOnlyAuthenticationDBContext>()
                    .ReplaceDefaultEntities<
                        ApiApplication,
                        ApiAuthorization,
                        ApiScope,
                        ApiToken,
                        string>();
            });
    }

    public static IServiceCollection AddCustomerUsersRepository(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<ICustomersViewLoginDataRepository, CustomersViewLoginDataRepository>(
            _ => new CustomersViewLoginDataRepository(connectionString));

        return services;
    }
}
