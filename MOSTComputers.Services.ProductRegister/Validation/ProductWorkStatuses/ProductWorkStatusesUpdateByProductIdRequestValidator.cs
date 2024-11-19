using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.ProductWorkStatuses;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal sealed class ProductWorkStatusesUpdateByProductIdRequestValidator : AbstractValidator<ProductWorkStatusesUpdateByProductIdRequest>
{
    public ProductWorkStatusesUpdateByProductIdRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}