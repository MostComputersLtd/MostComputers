using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.UI.Web.RealWorkTesting.Authentication;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;
using OneOf;
using static MOSTComputers.UI.Web.RealWorkTesting.Validation.ValidationCommonElements;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.FilePathUtils;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Accounts;

public sealed class LoginModel : PageModel
{
    private readonly IAuthenticationService _authenticationService;

    public LoginModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string? ReturnUrl { get; set; } = null;

#pragma warning disable IDE0060 // Remove unused parameter
    // This is used to implicitly load the returnUrl data in the ReturnUrl prop
    public void OnGet(string? returnUrl = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public async Task<IActionResult> OnPostLogInAsync()
    {
        LogInRequest logInRequest = new()
        {
            Username = Username,
            Password = Password,
        };

        OneOf<Microsoft.AspNetCore.Identity.SignInResult, ValidationResult> logInResult
            = await _authenticationService.LoginAsync(logInRequest);

        return logInResult.Match<IActionResult>(
            signInResult =>
            {
                if (signInResult.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(ReturnUrl))
                    {
                        return LocalRedirect(AddSlashAtTheStart(ReturnUrl)!);
                    }

                    return new OkResult();
                }

                ModelState.AddModelError("", "Bad credentials");

                return RedirectToPage("", new { ReturnUrl = RemoveSlashAtTheStart(ReturnUrl) });
            },
            validationResult =>
            {
                AddValidationErrorsToPageModelState(this, validationResult);

                return RedirectToPage("", new { ReturnUrl = RemoveSlashAtTheStart(ReturnUrl) });
            });
    }

    public JsonResult OnGetRedirectToSignInPage([FromQuery] string? returnUrl = null)
    {
        return new JsonResult(new { redirectUrl = $"/Accounts/SignIn?ReturnUrl={returnUrl}" });
    }
}