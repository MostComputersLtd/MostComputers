using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;

internal sealed class ProductWorkStatusesUpsertRequestValidator : AbstractValidator<ServiceProductWorkStatusesUpsertRequest>
{
    public ProductWorkStatusesUpsertRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.UpsertUserName).NotNullOrWhiteSpace();
    }
}