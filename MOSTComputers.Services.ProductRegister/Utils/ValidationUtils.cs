using FluentValidation;
using FluentValidation.Results;

namespace MOSTComputers.Services.ProductRegister.Utils;
internal static class ValidationUtils
{
    private static readonly ValidationResult _successResult = new();

    internal static ValidationResult CreateValidationResultFromErrors(params ValidationFailure[] errors)
    {
        ValidationResult result = new(errors);

        return result;
    }

    internal static ValidationResult ValidateDefault<T>(IValidator<T>? validator, T model)
    {
        if (validator is null) return _successResult;

        return validator.Validate(model);
    }

    internal static ValidationResult ValidateTwoValidatorsDefault<T>(T model, IValidator<T>? validator = null, IValidator<T>? otherValidator = null)
    {
        ValidationResult validationResult = ValidateDefault(validator, model);

        if (!validationResult.IsValid) return validationResult;

        ValidationResult validationResultInternal = ValidateDefault(otherValidator, model);

        if (!validationResultInternal.IsValid) return validationResultInternal;

        return _successResult;
    }

}