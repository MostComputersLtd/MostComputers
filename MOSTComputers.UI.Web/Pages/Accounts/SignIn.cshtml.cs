using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.UI.Web.Authentication;
using MOSTComputers.UI.Web.Models.Authentication;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;
using static MOSTComputers.UI.Web.StaticUtilities.FilePathUtils;

namespace MOSTComputers.UI.Web.Pages.Accounts;

public sealed class SignInModel(IAuthenticationService authenticationService) : PageModel
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    //[ValidateNever]
    public string? ConfirmPassword { get; set; } = string.Empty;

    [BindProperty]
    public string? ReturnUrl { get; set; } = null;

#pragma warning disable IDE0060 // Remove unused parameter
    // This is used to implicitly load the returnUrl data in the ReturnUrl prop
    public void OnGet(string? returnUrl = null)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public async Task<IActionResult> OnPostSignInAsync()
    {
        SignInRequest signInRequest = new()
        {
            Username = Username,
            Password = Password,
            ConfirmPassword = ConfirmPassword ?? string.Empty
        };

        OneOf<Success, ValidationResult, IEnumerable<IdentityError>> signInResult = await _authenticationService.SignInAsync(signInRequest);

        return signInResult.Match<IActionResult>(
            success =>
            {
                if (!string.IsNullOrWhiteSpace(ReturnUrl))
                {
                    return LocalRedirect(AddSlashAtTheStart(ReturnUrl)!);
                }

                return RedirectToPage("", new { ReturnUrl = RemoveSlashAtTheStart(ReturnUrl) });
            },
            validationResult =>
            {
                AddValidationErrorsToPageModelState(this, validationResult);

                return RedirectToPage("", new { ReturnUrl = RemoveSlashAtTheStart(ReturnUrl) });
            },
            errors =>
            {
                return RedirectToPage("", new { ReturnUrl = RemoveSlashAtTheStart(ReturnUrl) });
            });
    }
}