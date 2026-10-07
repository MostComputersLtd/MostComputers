using OpenIddict.EntityFrameworkCore.Models;

namespace MOSTComputers.Services.Identity.Models;

public sealed class ApiApplication
    : OpenIddictEntityFrameworkCoreApplication<
        Guid,
        ApiAuthorization,
        ApiToken>
{
    public int UserId { get; set; }
}

public sealed class ApiAuthorization
    : OpenIddictEntityFrameworkCoreAuthorization<
        Guid,
        ApiApplication,
        ApiToken>
{
}

public sealed class ApiToken
    : OpenIddictEntityFrameworkCoreToken<
        Guid,
        ApiApplication,
        ApiAuthorization>
{
}

public sealed class ApiScope
    : OpenIddictEntityFrameworkCoreScope<Guid>
{
}
