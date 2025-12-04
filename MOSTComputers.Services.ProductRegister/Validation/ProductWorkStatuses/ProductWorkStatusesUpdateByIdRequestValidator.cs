using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal sealed class ProductWorkStatusesUpdateByIdRequestValidator : AbstractValidator<ServiceProductWorkStatusesUpdateByIdRequest>
{
    public ProductWorkStatusesUpdateByIdRequestValidator()
    {
        RuleFor(x => x.Id).NotNull().GreaterThan(0);
        RuleFor(x => x.LastUpdateUserName).NotNullOrWhiteSpace();
    }
}