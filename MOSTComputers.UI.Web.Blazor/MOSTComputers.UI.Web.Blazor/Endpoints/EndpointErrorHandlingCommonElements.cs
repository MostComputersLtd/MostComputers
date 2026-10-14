using System.Globalization;
using System.Text.Json.Serialization;
using FluentValidation.Results;

namespace MOSTComputers.UI.Web.Blazor.Endpoints;

internal static class EndpointErrorHandlingCommonElements
{
    internal const string GenericErrorDetail = "An error has occured";

    private static readonly Dictionary<string, string[]> _errorAdditionalDataPerErrorCodeDictionary = new()
    {
        { "GreaterThanOrEqualValidator", ["ComparisonValue"] },
        { "GreaterThanValidator", ["ComparisonValue"] },
        { "LengthValidator", ["MinLength", "MaxLength", "TotalLength"] },
        { "MinimumLengthValidator", ["MinLength", "TotalLength"] },
        { "MaximumLengthValidator", ["MaxLength", "TotalLength"] },
        { "LessThanOrEqualValidator", ["ComparisonValue"] },
        { "LessThanValidator", ["ComparisonValue"] },
        { "NotEqualValidator", ["ComparisonValue"] },
        { "EqualValidator", ["ComparisonValue"] },
        { "ExactLengthValidator", ["MaxLength", "TotalLength"] },
        { "InclusiveBetweenValidator", ["From", "To"] },
        { "ExclusiveBetweenValidator", ["From", "To"] },
        { "ScalePrecisionValidator", ["ExpectedPrecision", "ExpectedScale", "Digits", "ActualScale"] },

        { "Length_Simple", ["MinLength", "MaxLength"] },
        { "MinimumLength_Simple", ["MinLength"] },
        { "MaximumLength_Simple", ["MaxLength"] },
        { "ExactLength_Simple", ["MaxLength"] },
        { "InclusiveBetween_Simple", ["From", "To"] },

        { "NullOrGreaterThan", ["ComparisonValue"] },
    };

    private sealed class ValidationResultProblemValidationError
    {
        [JsonPropertyName("errorCode")]
        public required string ErrorCode { get; init; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Data { get; init; }
    }

    private static object? FormatPlaceholderValue(object? value)
    {
        return value switch
        {
            DateOnly date => date.ToString(CultureInfo.CurrentCulture),
            DateTime date => date.ToString(CultureInfo.CurrentCulture),
            DateTimeOffset date => date.ToString(CultureInfo.CurrentCulture),
            TimeOnly time => time.ToString(CultureInfo.CurrentCulture),
            _ => value
        };
    }

    internal static IResult ValidationResultProblem(
        ValidationResult result,
        int? statusCode = StatusCodes.Status400BadRequest)
    {
        Dictionary<string, List<ValidationResultProblemValidationError>> errors = [];

        foreach (ValidationFailure validationFailure in result.Errors)
        {
            string? errorCode = validationFailure.ErrorCode;

            Dictionary<string, object>? additionalData = null;

            if (errorCode != null
                && validationFailure.FormattedMessagePlaceholderValues?.Count > 0)
            {
                bool hasAdditionalData = _errorAdditionalDataPerErrorCodeDictionary.TryGetValue(
                    errorCode, out string[]? additionalDataKeys); 

                if (hasAdditionalData)
                {
                    List<string> additionalDataKeysCopy = new(additionalDataKeys!);

                    foreach (KeyValuePair<string, object> placeholderKvp in validationFailure.FormattedMessagePlaceholderValues)
                    {
                        int keyIndex = additionalDataKeysCopy.IndexOf(placeholderKvp.Key);

                        if (keyIndex < 0) continue;

                        additionalData ??= new();

                        object? formattedValue = FormatPlaceholderValue(placeholderKvp.Value);

                        additionalData.Add(placeholderKvp.Key, formattedValue!);

                        additionalDataKeysCopy.RemoveAt(keyIndex);
                    }
                }
            }

            ValidationResultProblemValidationError validationError = new()
            {
                ErrorCode = errorCode ?? string.Empty,
                Data = additionalData,
            };

            bool existsInResults = errors.TryGetValue(validationFailure.PropertyName,
                out List<ValidationResultProblemValidationError>? failures);

            if (existsInResults)
            {
                failures!.Add(validationError);

                continue;
            }

            errors.Add(validationFailure.PropertyName, [validationError]);
        }

        return Results.Problem(
            statusCode: statusCode,
            title: "Validation failed",
            type: "/problems/validation",
            extensions: new Dictionary<string, object?>
            {
                ["validationErrors"] = errors
            });
    }
}
