using FluentValidation;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;

namespace MOSTComputers.UI.Web.RealWorkTesting.Validation.Authentication;

public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(request => request.Username).MinimumLength(3).MaximumLength(20);

        RuleFor(request => request.Password)
            .MinimumLength(8).WithMessage("Password must have atleast 8 characters")
            .Must(username =>
            {
                ReadOnlySpan<char> usernameSpan = username.AsSpan();

                List<char> uniqueChars = new();

                foreach (char item in usernameSpan)
                {
                    if (!uniqueChars.Contains(item))
                    {
                        uniqueChars.Add(item);
                    }
                }

                if (uniqueChars.Count < 5) return false;

                return true;
            })
            .WithMessage("Password must have atleast 5 different characters");

        RuleFor(request => request.ConfirmPassword)
            .NotNull().WithMessage("Must match the password")
            .NotEmpty().WithMessage("Must match the password")
            .Equal(request => request.Password).WithMessage("Must match the password");
    }
}
