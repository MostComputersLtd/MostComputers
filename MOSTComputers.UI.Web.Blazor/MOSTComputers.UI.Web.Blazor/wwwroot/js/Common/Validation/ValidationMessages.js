export function getValidationMessageOrDefault(validationError, messagesMap) {

    let message = messagesMap.get(validationError.errorCode);

    if (message === undefined) {

        message = defaultValidationMessages.get(validationError.errorCode);

        if (message === undefined) {

            return defaultMessage;
        }
    }

    if (validationError.data) {

        for (const [key, value] of Object.entries(validationError.data)) {

            message = addDataToMessage(message, key, value);
        }
    } 

    return message;
}

export function getValidationMessage(validationError, messagesMap, defaultMessage = "Invalid value.") {

    let message = messagesMap.get(validationError.errorCode);

    if (message === undefined) {

        return defaultMessage;
    }

    if (validationError.data) {

        for (const [key, value] of Object.entries(validationError.data)) {

            message = addDataToMessage(message, key, value);
        }
    } 

    return message;
}

export function getDefaultValidationMessage(validationError) {

    return getValidationMessage(validationError, defaultValidationMessages, defaultMessage);
}

function addDataToMessage(message, placeholderName, value) {

    return message.replaceAll(`{${placeholderName}}`, value.toString());
}

const defaultMessage = "Invalid value.";

const defaultValidationMessages = new Map([
    ["EmailValidator", "Must be a valid email address."],
    ["GreaterThanOrEqualValidator", "Must be greater than or equal to {ComparisonValue}."],
    ["GreaterThanValidator", "Must be greater than {ComparisonValue}."],
    ["LengthValidator", "Must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters."],
    ["MinimumLengthValidator", "Must be at least {MinLength} characters. You entered {TotalLength} characters."],
    ["MaximumLengthValidator", "Must be {MaxLength} characters or fewer. You entered {TotalLength} characters."],
    ["LessThanOrEqualValidator", "Must be less than or equal to {ComparisonValue}."],
    ["LessThanValidator", "Must be less than {ComparisonValue}."],
    ["NotEmptyValidator", "Must not be empty."],
    ["NotEqualValidator", "Must not be equal to {ComparisonValue}."],
    ["NotNullValidator", "Must not be empty."],
    ["PredicateValidator", "The specified condition was not met."],
    ["AsyncPredicateValidator", "The specified condition was not met."],
    ["RegularExpressionValidator", "Is not in the correct format."],
    ["EqualValidator", "Must be equal to {ComparisonValue}."],
    ["ExactLengthValidator", "Must be {MaxLength} characters in length. You entered {TotalLength} characters."],
    ["InclusiveBetweenValidator", "Must be between {From} and {To}."],
    ["ExclusiveBetweenValidator", "Must be between {From} and {To} (exclusive)."],
    ["CreditCardValidator", "Must be a valid credit card number."],
    ["ScalePrecisionValidator", "Must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals. {Digits} digits and {ActualScale} decimals were found."],
    ["EmptyValidator", "Must be empty."],
    ["NullValidator", "Must be empty."],
    ["EnumValidator", "Must be in supported range."],

    ["Length_Simple", "Must be between {MinLength} and {MaxLength} characters."],
    ["MinimumLength_Simple", "Must be at least {MinLength} characters."],
    ["MaximumLength_Simple", "Must be {MaxLength} characters or fewer."],
    ["ExactLength_Simple", "Must be {MaxLength} characters in length."],
    ["InclusiveBetween_Simple", "Must be between {From} and {To}."],

    ["NullOrGreaterThan", "Must be null or greater than {ComparisonValue}."],
    ["NotNullOrWhiteSpace", "Must not be null or whitespace."],
    ["NotEmptyOrWhiteSpace", "Must not be empty or whitespace."],
    ["NullOrNotEmpty", "Must be null or not empty."],

    ["ValidGTINCodeFormat", "Invalid code format."]
]);
