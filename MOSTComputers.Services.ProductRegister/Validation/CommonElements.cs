using FluentValidation;
using FluentValidation.Results;

namespace MOSTComputers.Services.ProductRegister.Validation;

internal static class CommonElements
{
    private static readonly ValidationResult _successResult = new();

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

    public static bool NullOrGreaterThanZero(int? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(decimal? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(float? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(short? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero(long? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(int? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(decimal? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(short? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(long? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero(float? num)
    {
        return num == null || num >= 0;
    }

    public static bool IsNotNullEmptyOrWhiteSpace(string? str)
    {
        if (string.IsNullOrEmpty(str)) return false;

        foreach (char character in str)
        {
            if (!char.IsWhiteSpace(character)) return true;
        }

        return false;
    }

    public static bool IsNotEmptyOrWhiteSpace(string? str)
    {
        if (str is null) return true;

        if (str == string.Empty) return false;

        foreach (char character in str)
        {
            if (!char.IsWhiteSpace(character)) return true;
        }

        return false;
    }

    public static bool IsNotWhiteSpace(string? str)
    {
        if (str is null) return true;

        if (str == string.Empty) return true;

        foreach (char character in str)
        {
            if (char.IsWhiteSpace(character)) return false;
        }

        return true;
    }

    public static bool IsEmpty(string str)
    {
        return !(str == string.Empty);
    }
}