using OpenIddict.EntityFrameworkCore.Models;

namespace MOSTComputers.Services.Identity.Models;

public sealed class ApiApplication
    : OpenIddictEntityFrameworkCoreApplication<
        string,
        ApiAuthorization,
        ApiToken>
{
    public int UserId { get; set; }
}

public sealed class ApiAuthorization
    : OpenIddictEntityFrameworkCoreAuthorization<
        string,
        ApiApplication,
        ApiToken>
{
}

public sealed class ApiToken
    : OpenIddictEntityFrameworkCoreToken<
        string,
        ApiApplication,
        ApiAuthorization>
{
}

public sealed class ApiScope
    : OpenIddictEntityFrameworkCoreScope<string>
{
}
