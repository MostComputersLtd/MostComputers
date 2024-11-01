using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Services;
using OneOf.Types;
using OneOf;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using FluentValidation.Results;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Authentication;

internal sealed class PasswordsTableOnlyAuthenticationService(
    SignInManager<PasswordsTableOnlyUser> signInManager,
    IPasswordsTableOnlyIdentityService identityService,
    IValidator<SignInRequest> signInRequestValidator,
    IValidator<LogInRequest> logInRequestValidator) : IAuthenticationService
{
    private readonly SignInManager<PasswordsTableOnlyUser> _signInManager = signInManager;
    private readonly IPasswordsTableOnlyIdentityService _identityService = identityService;
    private readonly IValidator<SignInRequest> _signInRequestValidator = signInRequestValidator;
    private readonly IValidator<LogInRequest> _logInRequestValidator = logInRequestValidator;

    public async Task<OneOf<Success, ValidationResult, IEnumerable<IdentityError>>> SignInAsync(SignInRequest signInRequest)
    {
        ValidationResult validationResult = _signInRequestValidator.Validate(signInRequest);

        if (!validationResult.IsValid) return validationResult;

        PasswordsTableOnlyUser user = new()
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

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}