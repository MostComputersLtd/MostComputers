using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ToDoLocalChange;
internal class ServiceToDoLocalChangeCreateRequestValidator : AbstractValidator<ServiceToDoLocalChangeCreateRequest>
{
    public ServiceToDoLocalChangeCreateRequestValidator()
    {
        RuleFor(x => x.TableName).Must(IsNotNullEmptyOrWhiteSpace);
        RuleFor(x => x.TableElementId).GreaterThan(0);
    }
}