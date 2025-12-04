using FluentValidation.Results;
using OneOf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.UI.Web.Models.Authentication;
using MOSTComputers.UI.Web.Services.Authentication;

using static MOSTComputers.UI.Web.Utils.FilePathUtils;
using static MOSTComputers.UI.Web.Utils.PageCommonElements;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.Pages.Accounts;
[Authorize(Roles = "Admin")]
public sealed class SignInModel : PageModel
{
    private readonly IAuthenticationService<PasswordsTableOnlyUser> _authenticationService;

    public SignInModel(IAuthenticationService<PasswordsTableOnlyUser> authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public SignInPageUsageOptionsEnum UsageEnum { get; set; } = SignInPageUsageOptionsEnum.SignIn;

    [BindProperty(SupportsGet = true)]
    public int Usage
    {
        get => (int)UsageEnum;
        set => UsageEnum = (SignInPageUsageOptionsEnum)value;
    }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; } = null;

#pragma warning disable IDE0060 // Remove unused parameter
    // This is used to implicitly load the returnUrl data in the ReturnUrl prop
    public IActionResult OnGet(int usage, string? returnUrl = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        ModelState.Clear();

        return Page();
    }

    public async Task<IActionResult> OnPostSignInAsync([FromBody] SignInDTO signInDTO)
    {
        SignInRequest signInRequest = new()
        {
            Username = signInDTO.Username,
            Password = signInDTO.Password,
            ConfirmPassword = signInDTO.ConfirmPassword ?? string.Empty
        };

        if (signInDTO.RoleValues is not null)
        {
            foreach (int roleValue in signInDTO.RoleValues)
            {
                UserRoles? roleWithValue = UserRoles.GetRoleWithValue(roleValue);

                if (roleWithValue is null) return BadRequest();

                signInRequest.Roles ??= new();

                signInRequest.Roles.Add(roleWithValue);
            }
        }

        string? returnUrl = null;

        if (signInDTO.ReturnUrl is not null)
        {
            returnUrl = GetOnlyPagePartOfUrl(signInDTO.ReturnUrl);
        }
        else if (ReturnUrl is not null)
        {
            returnUrl = GetOnlyPagePartOfUrl(ReturnUrl);
        }

        if (signInDTO.Usage == (int)SignInPageUsageOptionsEnum.CreateAccount)
        {
            return await CreateAccountAsync(signInRequest, returnUrl);
        }

        return await SignInAsync(signInRequest, returnUrl);
    }

    private async Task<IActionResult> CreateAccountAsync(SignInRequest signInRequest, string? returnUrl)
    {
        OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>> createAccountResult
            = await _authenticationService.CreateAccountAsync(signInRequest);

        return createAccountResult.Match<IActionResult>(
            user =>
            {
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return new JsonResult(new { redirectUrl = AddSlashAtTheStart(returnUrl)! });
                }

                return BadRequest();
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            errors =>
            {
                AddIdentityErrorsToModelState(this, errors);

                return BadRequest(ModelState);
            });
    }

    private async Task<IActionResult> SignInAsync(SignInRequest signInRequest, string? returnUrl)
    {
        OneOf<PasswordsTableOnlyUser, ValidationResult, IEnumerable<IdentityError>> signInResult
            = await _authenticationService.SignInAsync(signInRequest);

        return signInResult.Match<IActionResult>(
            user =>
            {
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return new JsonResult(new { redirectUrl = AddSlashAtTheStart(returnUrl)! });
                }

                return new JsonResult(new { redirectUrl = string.Empty });
            },
            validationResult =>
            {
                AddValidationErrorsToPageModelState(this, validationResult);

                return new JsonResult(new { redirectUrl = string.Empty });
            },
            errors =>
            {
                return new JsonResult(new { redirectUrl = string.Empty });
            });
    }

    public JsonResult OnGetRedirectToLogInPage([FromQuery] string? returnUrl = null)
    {
        return new JsonResult(new { redirectUrl = $"/Accounts/Login?ReturnUrl={returnUrl}" });
    }
}

public sealed class SignInDTO
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? ConfirmPassword { get; set; } = string.Empty;
    public int Usage { get; set; } = 0;

    public string? ReturnUrl { get; set; } = null;
    public IEnumerable<int>? RoleValues { get; set; } = null;
}