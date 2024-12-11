using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Services;
using MOSTComputers.UI.Web.Models.Authentication;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.Authentication;

internal class AuthenticationService(
    SignInManager<IdentityUser> signInManager,
    IIdentityService<IdentityUser, IdentityRole> identityService,
    IValidator<SignInRequest> signInRequestValidator,
    IValidator<LogInRequest> logInRequestValidator) : IAuthenticationService
{
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly IIdentityService<IdentityUser, IdentityRole> _identityService = identityService;
    private readonly IValidator<SignInRequest> _signInRequestValidator = signInRequestValidator;
    private readonly IValidator<LogInRequest> _logInRequestValidator = logInRequestValidator;

    public async Task<OneOf<Success, ValidationResult, IEnumerable<IdentityError>>> SignInAsync(SignInRequest signInRequest)
    {
        ValidationResult validationResult = _signInRequestValidator.Validate(signInRequest);

        if (!validationResult.IsValid) return validationResult;

        IdentityUser user = new()
        {
            UserName = signInRequest.Username
        };

        IdentityResult addUserResult = await _identityService.TryAddUserAsync(user, signInRequest.Password);

        if (!addUserResult.Succeeded)
        {
            return OneOf<Success, ValidationResult, IEnumerable<IdentityError>>.FromT2(addUserResult.Errors);
        }

        await _signInManager.SignInAsync(user, false);

        return new Success();
    }

    public async Task<OneOf<SignInResult, ValidationResult>> LoginAsync(LogInRequest loginRequest)
    {
        ValidationResult validationResult = _logInRequestValidator.Validate(loginRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password, false, false);
    }

    public async Task LogoutAsync(LogInRequest loginRequest)
    {
        await _signInManager.SignOutAsync();
    }
}