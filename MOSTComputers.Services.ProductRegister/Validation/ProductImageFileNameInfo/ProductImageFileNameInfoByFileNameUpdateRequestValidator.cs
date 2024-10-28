using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFileNameInfo;
internal class ProductImageFileNameInfoByFileNameUpdateRequestValidator
    : AbstractValidator<ServiceProductImageFileNameInfoByFileNameUpdateRequest>
{
    public ProductImageFileNameInfoByFileNameUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.NewDisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.NewFileName).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}