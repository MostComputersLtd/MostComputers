using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;

internal class ProductWorkStatusesCreateManyWithSameDataRequestValidator : AbstractValidator<ServiceProductWorkStatusesCreateManyWithSameDataRequest>
{
    public ProductWorkStatusesCreateManyWithSameDataRequestValidator()
    {
        RuleFor(x => x.ProductIds).NotEmpty();
        RuleFor(x => x.ProductIds).ForEach(x => x.NotNull().GreaterThan(0));
        RuleFor(x => x.CreateUserName).NotNullOrWhiteSpace();
    }
}