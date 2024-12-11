using FluentValidation;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;

namespace MOSTComputers.UI.Web.RealWorkTesting.Validation.Authentication;

public class LogInRequestValidator : AbstractValidator<LogInRequest>
{
    public LogInRequestValidator()
    {
        RuleFor(request => request.Username).MinimumLength(1).MaximumLength(20);

        RuleFor(request => request.Password)
            .NotNull().WithMessage("Password is required")
            .NotEmpty().WithMessage("Password is required");
    }
}
