using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
public interface IProductChangesService
{
    OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleInsert(LocalChangeData localChangeData);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleUpdate(LocalChangeData localChangeData);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleDelete(LocalChangeData localChangeData);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> HandleAnyOperation(LocalChangeData localChangeData);
}