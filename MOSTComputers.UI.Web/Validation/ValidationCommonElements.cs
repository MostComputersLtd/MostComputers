using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MOSTComputers.UI.Web.Validation;

public static class ValidationCommonElements
{
    public static IActionResult GetResultFromValidationResult(this PageModel pageModel, ValidationResult validationResult)
    {
        if (validationResult.IsValid) return new OkResult();

        foreach (ValidationFailure? error in validationResult.Errors)
        {
            pageModel.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return new BadRequestObjectResult(pageModel.ModelState);
    }
}