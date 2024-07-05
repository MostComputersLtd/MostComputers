using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.UI.Web.Models.Authentication;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.Authentication;
public interface IAuthenticationService
{
    Task<OneOf<SignInResult, ValidationResult>> LoginAsync(LogInRequest logInRequest);
    Task<OneOf<Success, ValidationResult, IEnumerable<IdentityError>>> SignInAsync(SignInRequest signInRequest);
    Task LogoutAsync(LogInRequest loginRequest);
}