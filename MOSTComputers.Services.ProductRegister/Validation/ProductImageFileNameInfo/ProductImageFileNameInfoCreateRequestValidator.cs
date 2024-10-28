using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFileNameInfo;

internal sealed class ProductImageFileNameInfoCreateRequestValidator : AbstractValidator<ServiceProductImageFileNameInfoCreateRequest>
{
    public ProductImageFileNameInfoCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
    }
}