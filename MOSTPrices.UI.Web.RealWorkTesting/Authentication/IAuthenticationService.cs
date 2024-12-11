using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.RealWorkTesting.Authentication;
public interface IAuthenticationService<TUser>
    where TUser : IdentityUser
{
    Task<OneOf<SignInResult, ValidationResult>> LoginAsync(LogInRequest logInRequest);
    Task<OneOf<TUser, ValidationResult, IEnumerable<IdentityError>>> CreateAccountAsync(SignInRequest signInRequest);
    Task<OneOf<TUser, ValidationResult, IEnumerable<IdentityError>>> SignInAsync(SignInRequest signInRequest);
    Task LogoutAsync();
    Task<bool> DeleteAccountAsync(string userId);
    Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword);
    Task<OneOf<IdentityResult, NotFound>> TryRemoveUserFromRoleAsync(string userId, UserRoles userRole);
    Task<OneOf<IdentityResult, NotFound>> TryAddUserToRoleAsync(string userId, UserRoles userRole);
}