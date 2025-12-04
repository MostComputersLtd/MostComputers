using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using MOSTComputers.UI.Web.Blazor.Client.Localization.Resources.Validation;

namespace MOSTComputers.UI.Web.Blazor.Client.Localization;

public sealed class ValidationMessagesLocalizer
{
    public ValidationMessagesLocalizer(
        IStringLocalizer<ValidationMessages> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    private readonly IStringLocalizer<ValidationMessages> _stringLocalizer;

    //  private readonly Dictionary<string, string> _errorDictionary = new()
    //  {
    //      { "EmailValidator", "'{PropertyName}' is not a valid email address." },
    //      { "GreaterThanOrEqualValidator", "'{PropertyName}' must be greater than or equal to '{ComparisonValue}'."},
    //      { "GreaterThanValidator", "'{PropertyName}' must be greater than '{ComparisonValue}'."},
    //      { "LengthValidator", "'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters."},
    //      { "MinimumLengthValidator", "The length of '{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters."},
    //      { "MaximumLengthValidator", "The length of '{PropertyName}' must be {MaxLength} characters or fewer. You entered {TotalLength} characters."},
    //      { "LessThanOrEqualValidator", "'{PropertyName}' must be less than or equal to '{ComparisonValue}'."},
    //      { "LessThanValidator", "'{PropertyName}' must be less than '{ComparisonValue}'."},
    //      { "NotEmptyValidator", "'{PropertyName}' must not be empty."},
    //      { "NotEqualValidator", "'{PropertyName}' must not be equal to '{ComparisonValue}'."},
    //      { "NotNullValidator", "'{PropertyName}' must not be empty."},
    //      { "PredicateValidator", "The specified condition was not met for '{PropertyName}'."},
    //      { "AsyncPredicateValidator", "The specified condition was not met for '{PropertyName}'."},
    //      { "RegularExpressionValidator", "'{PropertyName}' is not in the correct format."},
    //      { "EqualValidator", "'{PropertyName}' must be equal to '{ComparisonValue}'."},
    //      { "ExactLengthValidator", "'{PropertyName}' must be {MaxLength} characters in length. You entered {TotalLength} characters."},
    //      { "InclusiveBetweenValidator", "'{PropertyName}' must be between {From} and {To}. You entered {PropertyValue}."},
    //      { "ExclusiveBetweenValidator", "'{PropertyName}' must be between {From} and {To} (exclusive). You entered {PropertyValue}."},
    //      { "CreditCardValidator", "'{PropertyName}' is not a valid credit card number."},
    //      { "ScalePrecisionValidator", "'{PropertyName}' must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals. {Digits} digits and {ActualScale} decimals were found."},
    //      { "EmptyValidator", "'{PropertyName}' must be empty."},
    //      { "NullValidator", "'{PropertyName}' must be empty."},
    //      { "EnumValidator", "'{PropertyName}' has a range of values which does not include '{PropertyValue}'."},

    //      { "Length_Simple", "'{PropertyName}' must be between {MinLength} and {MaxLength} characters."},
    //      { "MinimumLength_Simple", "The length of '{PropertyName}' must be at least {MinLength} characters."},
    //      { "MaximumLength_Simple", "The length of '{PropertyName}' must be {MaxLength} characters or fewer."},
    //      { "ExactLength_Simple", "'{PropertyName}' must be {MaxLength} characters in length."},
    //      { "InclusiveBetween_Simple", "'{PropertyName}' must be between {From} and {To}."},

    //      { "NullOrGreaterThan", "Must be null or greater than {ComparisonValue}."},
    //      { "NotNullOrWhiteSpace", "Must be not null or whitespace."},
    //      { "NotEmptyOrWhiteSpace", "Must be not empty or whitespace."},
    //      { "NullOrNotEmpty", "Must be either be null or not empty."},

    //      { "ValidGTINCodeFormat", "Invalid code format."},
    //  };

    public string LocalizeMessage(ValidationFailure validationFailure)
    {
        if (string.IsNullOrWhiteSpace(validationFailure.ErrorCode))
        {
            return validationFailure.ErrorMessage;
        }

        LocalizedString localizedTemplate = _stringLocalizer.GetString(validationFailure.ErrorCode);

        string template = localizedTemplate.Value;

        if (localizedTemplate.ResourceNotFound)
        {
            template = validationFailure.ErrorMessage;
        }

        MessageFormatter formatter = new();

        if (!string.IsNullOrWhiteSpace(validationFailure.PropertyName))
        {
            formatter.AppendPropertyName(validationFailure.PropertyName);
        }

        if (validationFailure.AttemptedValue is not null)
        {
            formatter.AppendPropertyValue(validationFailure.AttemptedValue);
        }

        if (validationFailure.FormattedMessagePlaceholderValues is not null)
        {
            foreach (KeyValuePair<string, object> messagePlaceholderKvp in validationFailure.FormattedMessagePlaceholderValues)
            {
                formatter.AppendArgument(messagePlaceholderKvp.Key, messagePlaceholderKvp.Value);
            }
        }

        return formatter.BuildMessage(template);
    }
}