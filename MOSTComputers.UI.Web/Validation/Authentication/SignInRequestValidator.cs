using FluentValidation;
using MOSTComputers.UI.Web.Models.Authentication;

namespace MOSTComputers.UI.Web.Validation.Authentication;
public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(request => request.Username).MinimumLength(1).MaximumLength(20);

        RuleFor(request => request.Password)
            .NotNull().WithMessage("Password is required")
            .NotEmpty().WithMessage("Password is required");

        RuleFor(request => request.ConfirmPassword)
            .NotNull().WithMessage("Must match the password")
            .NotEmpty().WithMessage("Must match the password")
            .Equal(request => request.Password).WithMessage("Must match the password");

        RuleFor(request => request.Roles).Must(
            roleNames =>
            {
                if (roleNames is null || roleNames.Count <= 0) return true;

                List<string> uniqueRoles = new();

                foreach (UserRoles item in roleNames)
                {
                    if (item.RoleName is null) return false;

                    if (uniqueRoles.Contains(item.RoleName)) return false;

                    uniqueRoles.Add(item.RoleName);
                }

                return true;
            });
    }
}
