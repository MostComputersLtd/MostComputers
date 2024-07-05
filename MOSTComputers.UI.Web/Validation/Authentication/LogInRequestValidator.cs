using FluentValidation;
using MOSTComputers.UI.Web.Models.Authentication;

namespace MOSTComputers.UI.Web.Validation.Authentication;

public class LogInRequestValidator : AbstractValidator<LogInRequest>
{
    public LogInRequestValidator()
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
    }
}
