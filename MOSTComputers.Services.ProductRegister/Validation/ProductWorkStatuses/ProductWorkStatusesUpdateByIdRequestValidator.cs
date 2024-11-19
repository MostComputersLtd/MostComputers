using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.ProductWorkStatuses;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal sealed class ProductWorkStatusesUpdateByIdRequestValidator : AbstractValidator<ProductWorkStatusesUpdateByIdRequest>
{
    public ProductWorkStatusesUpdateByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotNull().GreaterThan(0);
    }
}