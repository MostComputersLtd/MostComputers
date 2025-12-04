using FluentValidation.Results;
using OneOf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.UI.Web.Models.Authentication;
using MOSTComputers.UI.Web.Services.Authentication;

using static MOSTComputers.UI.Web.Utils.FilePathUtils;
using static MOSTComputers.UI.Web.Utils.PageCommonElements;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.Pages.Accounts;
public sealed class LoginModel : PageModel
{
    private readonly IAuthenticationService<PasswordsTableOnlyUser> _authenticationService;

    public LoginModel(IAuthenticationService<PasswordsTableOnlyUser> authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; } = null;

#pragma warning disable IDE0060 // Remove unused parameter
    // This is used to implicitly load the returnUrl data in the ReturnUrl prop
    public void OnGet(string? returnUrl = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public async Task<IActionResult> OnPostLogInAsync([FromBody] LoginDTO loginDTO)
    {
        LogInRequest logInRequest = new()
        {
            Username = loginDTO.Username,
            Password = loginDTO.Password,
        };

        string? returnUrl = null;

        if (loginDTO.ReturnUrl is not null)
        {
            returnUrl = GetOnlyPagePartOfUrl(loginDTO.ReturnUrl);
        }
        else if (ReturnUrl is not null)
        {
            returnUrl = GetOnlyPagePartOfUrl(ReturnUrl);
        }

        OneOf<Microsoft.AspNetCore.Identity.SignInResult, ValidationResult> logInResult
            = await _authenticationService.LoginAsync(logInRequest);

        return logInResult.Match<IActionResult>(
            signInResult =>
            {
                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return new JsonResult(new { redirectUrl = AddSlashAtTheStart(returnUrl)! });
                    }

                    return new JsonResult(new { redirectUrl = "/" });
                }

                ModelState.AddModelError("", "Bad credentials");

                return BadRequest(ModelState);
            },
            validationResult =>
            {
                AddValidationErrorsToPageModelState(this, validationResult);

                return BadRequest(ModelState);
            });
    }

    public JsonResult OnGetRedirectToSignInPage([FromQuery] string? returnUrl = null)
    {
        return new JsonResult(new { redirectUrl = $"/Accounts/SignIn?ReturnUrl={returnUrl}" });
    }
}

public sealed class LoginDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; } = null;
}