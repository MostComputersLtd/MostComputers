using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.ProductWorkStatuses;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal class ProductWorkStatusesCreateRequestValidator : AbstractValidator<ProductWorkStatusesCreateRequest>
{
    public ProductWorkStatusesCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).NotNull().GreaterThan(0);
    }
}