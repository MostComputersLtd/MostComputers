using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models.Customers;

namespace MOSTComputers.Services.Authentication.Contracts;
public interface ICustomAuthenticationService
{
    Task<SignInResult> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure);
    Task RefreshSignInAsync();
    Task<CheckPasswordResult> CheckIfCustomerCredentialsExistAsync(string username, string password);
}
