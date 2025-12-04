using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MOSTComputers.Services.Identity.Models;

namespace MOSTComputers.UI.Web.Blazor.Components.Account;
// Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
internal sealed class IdentityNoOpEmailSender : IEmailSender<PasswordsTableOnlyUser>
{
    private readonly IEmailSender _emailSender = new NoOpEmailSender();

    public Task SendConfirmationLinkAsync(PasswordsTableOnlyUser user, string email, string confirmationLink) =>
        _emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    public Task SendPasswordResetLinkAsync(PasswordsTableOnlyUser user, string email, string resetLink) =>
        _emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public Task SendPasswordResetCodeAsync(PasswordsTableOnlyUser user, string email, string resetCode) =>
        _emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
