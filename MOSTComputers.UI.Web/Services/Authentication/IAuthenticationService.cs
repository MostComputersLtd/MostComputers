using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.UI.Web.Models.Authentication;

namespace MOSTComputers.UI.Web.Services.Authentication;
public interface IAuthenticationService<TUser>
    where TUser : IdentityUser
{
    Task<OneOf<SignInResult, ValidationResult>> LoginAsync(LogInRequest logInRequest);
    Task<OneOf<TUser, ValidationResult, IEnumerable<IdentityError>>> SignInAsync(SignInRequest signInRequest);
    Task<OneOf<TUser, ValidationResult, IEnumerable<IdentityError>>> CreateAccountAsync(SignInRequest signInRequest);
    Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword);
    Task<OneOf<IdentityResult, NotFound>> TryAddUserToRoleAsync(string userId, UserRoles userRole);
    Task<OneOf<IdentityResult, NotFound>> TryRemoveUserFromRoleAsync(string userId, UserRoles userRole);
    Task LogoutAsync();
    Task<bool> DeleteAccountAsync(string userId);
}