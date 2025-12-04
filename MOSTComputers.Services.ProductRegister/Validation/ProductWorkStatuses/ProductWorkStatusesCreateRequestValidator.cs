using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductWorkStatuses;
internal class ProductWorkStatusesCreateRequestValidator : AbstractValidator<ServiceProductWorkStatusesCreateRequest>
{
    public ProductWorkStatusesCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).NotNull().GreaterThan(0);
        RuleFor(x => x.CreateUserName).NotNullOrWhiteSpace();
    }
}