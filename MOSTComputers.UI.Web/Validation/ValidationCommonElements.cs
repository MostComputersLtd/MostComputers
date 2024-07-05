using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MOSTComputers.UI.Web.Validation;

public static class ValidationCommonElements
{
    public static IActionResult GetBadRequestResultFromValidationResult(this PageModel pageModel, ValidationResult validationResult)
    {
        if (validationResult.IsValid) return new OkResult();

        AddValidationErrorsToPageModelState(pageModel, validationResult);

        return new BadRequestObjectResult(pageModel.ModelState);
    }

    public static IActionResult GetBadRequestResultFromValidationResult(ValidationResult validationResult)
    {
        if (validationResult.IsValid) return new OkResult();

        SerializableError serializableError = [];

        foreach (ValidationFailure? error in validationResult.Errors)
        {
            serializableError.TryGetValue(error.PropertyName, out object? errorsListObj);

            if (errorsListObj is List<string> errorList)
            {
                errorList.Add(error.ErrorMessage);

                continue;
            }

            serializableError.Add(error.PropertyName, new List<string>
            {
                error.ErrorMessage
            });
        }

        return new BadRequestObjectResult(serializableError);
    }

    public static IActionResult GetPageResultWithErrorsInModelStateFromValidationResult(this PageModel pageModel, ValidationResult validationResult)
    {
        if (validationResult.IsValid) return new OkResult();

        AddValidationErrorsToPageModelState(pageModel, validationResult);

        return pageModel.Page();
    }

    internal static void AddValidationErrorsToPageModelState(PageModel pageModel, ValidationResult validationResult)
    {
        foreach (ValidationFailure? error in validationResult.Errors)
        {
            pageModel.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}