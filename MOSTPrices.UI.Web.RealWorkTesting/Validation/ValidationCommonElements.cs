using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MOSTComputers.UI.Web.RealWorkTesting.Validation;

public static class ValidationCommonElements
{
    public static IStatusCodeActionResult GetBadRequestResultFromValidationResult(this PageModel pageModel, ValidationResult validationResult)
    {
        if (validationResult.IsValid) return new OkResult();

        AddValidationErrorsToPageModelState(pageModel, validationResult);

        return new BadRequestObjectResult(pageModel.ModelState);
    }

    public static IStatusCodeActionResult GetBadRequestResultFromValidationResult(ValidationResult validationResult)
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

    internal static IStatusCodeActionResult GetActionResultFromIdentityResult(this PageModel pageModel, IdentityResult identityResult)
    {
        if (identityResult.Succeeded) return new OkResult();

        AddIdentityErrorsToModelState(pageModel, identityResult.Errors);

        return new BadRequestObjectResult(pageModel.ModelState);
    }

    internal static void AddIdentityErrorsToModelState(PageModel pageModel, IEnumerable<IdentityError> identityErrors)
    {
        foreach (IdentityError error in identityErrors)
        {
            pageModel.ModelState.AddModelError(error.Code, error.Description);
        }
    }
}