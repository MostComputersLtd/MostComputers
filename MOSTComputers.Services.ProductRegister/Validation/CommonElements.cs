using FluentValidation;
using FluentValidation.Validators;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Utils.Files;
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

    public static IRuleBuilderOptions<T, ICollection<TItem?>> MaximumLength<T, TItem>(
        this IRuleBuilder<T, ICollection<TItem?>> ruleBuilder, int maximumLength)
    {
        return ruleBuilder.Must((obj, property, validationContext) =>
        {
            bool isValid = property.Count <= maximumLength;

            if (isValid) return true;

            validationContext.MessageFormatter.AppendArgument("MaximumLength", maximumLength);
            validationContext.MessageFormatter.AppendArgument("AttemptedLength", property.Count);

            return false;
        })
            .WithMessage($"Value must not have length greater than {maximumLength}")
            .WithErrorCode("MaximumLengthCollection");
    }

    public static IRuleBuilderOptions<T, TItem> FileNameMaxLengthValidation<T, TItem>(
       this IRuleBuilder<T, TItem> ruleBuilder, int maxLength, Func<T, TItem, string> fullFileNameFunc)
    {
        return ruleBuilder.SetValidator(
            new MaxCombinedLengthValidator<T, TItem>((obj, property) =>
            {
                return fullFileNameFunc(obj, property).Length;
            }, maxLength));
    }

    public class MaxCombinedLengthValidator<T, TItem> : PropertyValidator<T, TItem>
    {
        private readonly Func<T, TItem, int> _lengthFunc;
        private readonly int _max;

        public MaxCombinedLengthValidator(Func<T, TItem, int> lengthFunc, int max)
        {
            _lengthFunc = lengthFunc;
            _max = max;
        }

        public override bool IsValid(ValidationContext<T> context, TItem value)
        {
            int totalLength = _lengthFunc(context.InstanceToValidate, value);

            context.MessageFormatter
                .AppendArgument("MaxLength", _max)
                .AppendArgument("TotalLength", totalLength);

            return totalLength <= _max;
        }

        public override string Name => "MaximumLengthValidator";

        protected override string GetDefaultMessageTemplate(string errorCode)
            => "The length of '{PropertyName}' must be {MaxLength} characters or fewer. You entered {TotalLength} characters.";
    }

    public static IRuleBuilderOptions<T, IEnumerable<TItem>> HasDuplicateValues<T, TItem>(
        this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder,
        Func<TItem, TItem, bool> equalityComparison)
    {
        return ruleBuilder.Must(property =>
        {
            List<TItem> existingItems = new();

            foreach (TItem item in property)
            {
                foreach (TItem existingItem in existingItems)
                {
                    if (equalityComparison(item, existingItem))
                    {
                        return false;
                    }

                    existingItems.Add(item);
                }
            }

            return true;
        })
            .WithMessage($"Collection must not contain duplicate values")
            .WithErrorCode("ContainsDuplicateValues");
    }

    public static IRuleBuilderOptions<T, string?> IsImageContentType<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(ContentTypeUtils.IsImageContentType)
            .WithMessage($"Value must be a valid image content type")
            .WithErrorCode("IsImageContentType");
    }

    public static IRuleBuilderOptions<T, string?> IsExtensionValidForContentType<T>(
        this IRuleBuilder<T, string?> ruleBuilder, Func<T, string> contentTypeFunc)
    {
        return ruleBuilder.Must((obj, property, validationContext) =>
        {
            string contentType = contentTypeFunc(obj);
            
            bool isValid = ExtensionIsValidForContentType(contentType, property);

            if (!isValid)
            {
                validationContext.MessageFormatter
                    .AppendArgument("ContentType", contentType)
                    .AppendArgument("FileExtension", property);
            }

            return isValid;
        })
            .WithMessage($"Value must be a valid extension for given content type")
            .WithErrorCode("ValidExtensionForContentType");

    }

    public static IRuleBuilderOptions<T, string?> IsExtensionValidForContentType<T>(
        this IRuleBuilder<T, string?> ruleBuilder, string contentType)
    {
        return ruleBuilder.Must((obj, property, validationContext) =>
        {
            bool isValid = ExtensionIsValidForContentType(contentType, property);

            if (!isValid)
            {
                validationContext.MessageFormatter
                    .AppendArgument("ContentType", contentType)
                    .AppendArgument("FileExtension", property);
            }

            return isValid;
        })
            .WithMessage($"Value must be a valid extension for given content type")
            .WithErrorCode("ValidExtensionForContentType");
    }

    private static bool ExtensionIsValidForContentType(string contentType, string? property)
    {
        if (property == null) return false;

        List<string> allowedExtensions = ContentTypeUtils.GetPossibleExtensionsFromContentType(contentType);

        bool isValid = allowedExtensions.Contains(property);

        return isValid;
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