using Microsoft.AspNetCore.Identity;

namespace MOSTComputers.UI.Web.Blazor.Components.Account;
internal interface ICustomAuthenticationService
{
    Task<SignInResult> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure);
    Task RefreshSignInAsync();
}