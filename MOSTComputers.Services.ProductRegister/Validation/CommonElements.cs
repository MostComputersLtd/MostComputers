using FluentValidation;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using System.Numerics;

namespace MOSTComputers.Services.ProductRegister.Validation;
internal static class CommonElements
{
    public static IRuleBuilderOptions<T, TNumber?> NullOrGreaterThan<T, TNumber>(
        this IRuleBuilder<T, TNumber?> ruleBuilder, TNumber minValue)
        where TNumber: struct, INumber<TNumber>
    {
        return ruleBuilder.Must((obj, property, validationContext) =>
        {
            bool valid = property == null || property > minValue;

            if (valid) return true;

            validationContext.MessageFormatter.AppendArgument("ComparisonValue", minValue);

            return false;
        })
            .WithMessage($"Value must be null or greater than {minValue}")
            .WithErrorCode("NullOrGreaterThan");
    }

    public static IRuleBuilderOptions<T, string?> NotNullOrWhiteSpace<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Value must be not null or whitespace")
            .WithErrorCode("NotNullOrWhiteSpace");
    }

    public static IRuleBuilderOptions<T, string?> NotEmptyOrWhiteSpace<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(x =>
        {
            if (x is null) return true;

            if (x == string.Empty) return false;

            foreach (char character in x)
            {
                if (!char.IsWhiteSpace(character)) return true;
            }

            return false;
        })
            .WithMessage("Value must be not empty or whitespace")
            .WithErrorCode("NotEmptyOrWhiteSpace");
    }

    public static IRuleBuilderOptions<T, TItem[]?> NullOrNotEmpty<T, TItem>(
        this IRuleBuilder<T, TItem[]?> ruleBuilder)
    {
        return ruleBuilder.Must(x => x is null || x.Length > 0)
            .WithMessage("Value must be either be null or not empty")
            .WithErrorCode("NullOrNotEmpty");
    }

    public static IRuleBuilderOptions<T, ICollection<TItem>?> NullOrNotEmpty<T, TItem>(
        this IRuleBuilder<T, ICollection<TItem>?> ruleBuilder)
    {
        return ruleBuilder.Must(x => x is null || x.Count > 0)
            .WithMessage("Value must be either be null or not empty")
            .WithErrorCode("NullOrNotEmpty");
    }

    public static IRuleBuilderOptions<T, IEnumerable<TItem>?> NullOrNotEmpty<T, TItem>(
        this IRuleBuilder<T, IEnumerable<TItem>?> ruleBuilder)
    {
        return ruleBuilder.Must(x => x is null || x.Any())
            .WithMessage("Value must be either be null or not empty")
            .WithErrorCode("NullOrNotEmpty");
    }

    public static IRuleBuilderOptions<T, string> ValidGTINCodeForType<T>(
        this IRuleBuilder<T, string> ruleBuilder, Func<T, ProductGTINCodeType> getCodeTypeFunc)
    {
        return ruleBuilder.Must((obj, property, validationContext) =>
        {
            ProductGTINCodeType codeType = getCodeTypeFunc(obj);

            bool valid = codeType.IsValid(property);

            if (valid) return true;

            validationContext.MessageFormatter.AppendArgument("CodeTypeValue", codeType.Value);

            return false;
        })
            .WithMessage("Invalid code format.")
            .WithErrorCode("ValidGTINCodeFormat");
    }

    internal static List<TValue> DoesNotHaveNotNullDuplicates<T, TValue>(IEnumerable<T> datas, Func<T, TValue?> getValueFunc)
    {
        List<TValue> knownValues = new();
        List<TValue> repeatedValues = new();

        foreach (T data in datas)
        {
            TValue? value = getValueFunc(data);

            if (value is null)
            {
                continue;
            }

            if (knownValues.Contains(value))
            {
                if (repeatedValues.Contains(value))
                {
                    continue;
                }

                repeatedValues.Add(value);

                continue;
            }

            knownValues.Add(value);
        }

        return repeatedValues;
    }
}