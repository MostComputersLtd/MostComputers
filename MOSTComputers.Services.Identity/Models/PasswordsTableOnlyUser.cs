using Microsoft.AspNetCore.Identity;

namespace MOSTComputers.Services.Identity.Models;
public sealed class PasswordsTableOnlyUser : IdentityUser
{
    public string? PersonRealName { get; set; }
}