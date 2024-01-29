using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
public interface IProductChangesService
{
    OneOf<Success, UnexpectedFailureResult> HandleInsert(LocalChangeData localChangeData);
    OneOf<Success, UnexpectedFailureResult> HandleUpdate(LocalChangeData localChangeData);
    OneOf<Success, UnexpectedFailureResult> HandleDelete(LocalChangeData localChangeData);
}