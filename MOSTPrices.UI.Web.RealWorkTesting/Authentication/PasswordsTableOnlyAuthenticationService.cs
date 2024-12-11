using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Identity.Services;
using OneOf.Types;
using OneOf;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using FluentValidation.Results;
using MOSTComputers.Services.Identity.Models;
using System.Security.Claims;

namespace MOSTComputers.UI.Web.RealWorkTesting.Authentication;

internal sealed class PasswordsTableOnlyAuthenticationService(
    SignInManager<PasswordsTableOnlyUser> signInManager,
    IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> identityService,
    IValidator<SignInRequest> signInRequestValidator,
    IValidator<LogInRequest> logInRequestValidator) : IAuthenticationService<PasswordsTableOnlyUser>
{
    private readonly SignInManager<PasswordsTableOnlyUser> _signInManager = signInManager;
    private readonly IIdentityService<PasswordsTableOnlyUser, PasswordsTableOnlyRole> _identityService = identityService;
    private readonly IValidator<SignInRequest> _signInRequestValidator = signInRequestValidator;
    private readonly IValidator<LogInRequest> _logInRequestValidator = logInRequestValidator;

    public async Task<OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>>> CreateAccountAsync(SignInRequest signInRequest)
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
            return OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>>.FromT2(addUserResult.Errors);
        }

        if (signInRequest.Roles?.Count > 0)
        {
            IdentityResult addToRolesResult = await _identityService.TryAddUserToRolesAsync(user, signInRequest.Roles.Select(x => x.RoleName));

            if (!addToRolesResult.Succeeded)
            {
                return OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>>.FromT2(addToRolesResult.Errors);
            }
        }

        return user;
    }

    public async Task<OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>>> SignInAsync(SignInRequest signInRequest)
    {
        OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>> createAccountResult = await CreateAccountAsync(signInRequest);

        if (createAccountResult.IsT0)
        {
            PasswordsTableOnlyUser user = createAccountResult.AsT0;

            await _signInManager.SignInAsync(user, false);

            return user;
        }

        return createAccountResult.Match(
            user => user,
            validationResult => validationResult,
            identityErrors => OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>>.FromT2(identityErrors));
    }

    public async Task<OneOf<IdentityResult, NotFound>> TryAddUserToRoleAsync(string userId, UserRoles userRole)
    {
        PasswordsTableOnlyUser? user = await _identityService.GetUserAsync(userId);

        if (user is null) return new NotFound();

        IdentityResult addToRolesResult = await _identityService.TryAddUserToRoleAsync(user, userRole.RoleName);

        return addToRolesResult;
    }

    public async Task<OneOf<SignInResult, ValidationResult>> LoginAsync(LogInRequest loginRequest)
    {
        ValidationResult validationResult = _logInRequestValidator.Validate(loginRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password, false, false);
    }

    public async Task<OneOf<IdentityResult, NotFound>> ChangePasswordForUserAsync(string userId, string newPassword)
    {
        return await _identityService.ChangePasswordForUserAsync(userId, newPassword);
    }

    public async Task<OneOf<IdentityResult, NotFound>> TryRemoveUserFromRoleAsync(string userId, UserRoles userRole)
    {
        PasswordsTableOnlyUser? user = await _identityService.GetUserAsync(userId);

        if (user is null) return new NotFound();

        IdentityResult addToRolesResult = await _identityService.TryRemoveUserFromRoleAsync(user, userRole.RoleName);

        return addToRolesResult;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> DeleteAccountAsync(string userId)
    {
        bool deleteUserResult = await _identityService.TryDeleteUserAsync(userId);

        if (!deleteUserResult) return false;

        string? currentUserId = _signInManager.Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == userId)
        {
            await _signInManager.SignOutAsync();
        }

        return true;
    }
}