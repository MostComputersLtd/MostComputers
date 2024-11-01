using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.RealWorkTesting.Authentication;
public interface IAuthenticationService
{
    Task<OneOf<SignInResult, ValidationResult>> LoginAsync(LogInRequest logInRequest);
    Task<OneOf<Success, ValidationResult, IEnumerable<IdentityError>>> SignInAsync(SignInRequest signInRequest);
    Task LogoutAsync();
}