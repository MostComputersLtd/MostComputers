using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Requests.Product;
using System.Numerics;

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

    public static bool NullOrGreaterThanZero(byte? num)
    {
        if (num == null) return true;

        return num > 0;
    }

    public static bool NullOrGreaterThanZero<T>(T? num)
        where T : INumber<T>
    {
        if (num == null) return true;
        
        return num.CompareTo(T.Zero) > 0;
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

    public static bool NullOrGreaterThanOrEqualToZero(byte? num)
    {
        return num == null || num >= 0;
    }

    public static bool NullOrGreaterThanOrEqualToZero<T>(T? num)
        where T : INumberBase<T>, IComparable<T>
    {
        return num == null || num.CompareTo(T.Zero) > 0;
    }

    public static bool NullOrGreaterThanOrEqualTo<T>(T? num, T comparisonNum)
        where T : struct, INumberBase<T>, IComparable<T>
    {
        return num == null || num.Value.CompareTo(comparisonNum) > 0;
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

    public static bool IsNullOrNotEmpty(byte[]? x)
    {
        if (x is null) return true;

        return x.Length != 0;
    }

    internal static bool DoesNotHavePropertiesWithDuplicateCharacteristics(List<CurrentProductPropertyCreateRequest>? propertyCreateRequests)
    {
        if (propertyCreateRequests is null
            || propertyCreateRequests.Count <= 0) return true;

        List<int> productCharacteristicIds = new();

        foreach (CurrentProductPropertyCreateRequest propertyCreateRequest in propertyCreateRequests)
        {
            int? productCharacteristicId = propertyCreateRequest.ProductCharacteristicId;

            if (productCharacteristicId is null) continue;

            if (productCharacteristicIds.Contains(productCharacteristicId.Value))
            {
                return false;
            }

            productCharacteristicIds.Add(productCharacteristicId.Value);
        }

        return true;
    }

    internal static bool DoesNotHavePropertiesWithDuplicateCharacteristics(List<CurrentProductPropertyUpdateRequest>? propertyCreateRequests)
    {
        if (propertyCreateRequests is null
            || propertyCreateRequests.Count <= 0) return true;

        List<int> productCharacteristicIds = new();

        foreach (CurrentProductPropertyUpdateRequest propertyUpdateRequest in propertyCreateRequests)
        {
            int? productCharacteristicId = propertyUpdateRequest.ProductCharacteristicId;

            if (productCharacteristicId is null) continue;

            if (productCharacteristicIds.Contains(productCharacteristicId.Value))
            {
                return false;
            }

            productCharacteristicIds.Add(productCharacteristicId.Value);
        }

        return true;
    }

    public static List<int> RemoveValuesSmallerThanNumber(IEnumerable<int> intList, int value)
    {
        List<int> output = new();

        foreach (int item in intList)
        {
            if (item <= value) continue;

            output.Add(item);
        }

        return output;
    }

    public static List<int> RemoveValuesSmallerThanOne(IEnumerable<int> intList)
    {
        return RemoveValuesSmallerThanNumber(intList, 0);
    }
}