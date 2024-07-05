using Microsoft.AspNetCore.Identity;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.Identity.Services;

internal sealed class IdentityService(UserManager<IdentityUser> userManager) : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<IdentityResult> TryAddUserAsync(IdentityUser user, string password)
    {
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return result;
    }
}