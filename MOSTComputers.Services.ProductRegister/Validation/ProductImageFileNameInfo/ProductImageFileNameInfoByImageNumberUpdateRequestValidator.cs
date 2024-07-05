using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFileNameInfo;

internal sealed class ProductImageFileNameInfoByImageNumberUpdateRequestValidator
    : AbstractValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>
{
    public ProductImageFileNameInfoByImageNumberUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileName).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.NewDisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.ImageNumber).GreaterThan(0);
    }
}