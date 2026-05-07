using FluentValidation;

using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.PromotionGroupConstraints;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;

internal sealed class PromotionGroupCreateRequestValidator : AbstractValidator<ServicePromotionGroupCreateRequest>
{
    public PromotionGroupCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmptyOrWhiteSpace().MaximumLength(NameMaxLength);
        RuleFor(x => x.Header).NotNull().MaximumLength(HeaderMaxLength);

        RuleFor(x => x.Logo!.Image).NotEmpty()
            .When(x => x.Logo is not null);

        RuleFor(x => x.Logo!.ContentType).NotEmptyOrWhiteSpace().MaximumLength(LogoContentTypeMaxLength)
            .When(x => x.Logo is not null);

        RuleFor(x => x.DisplayOrder).NotNull();
    }
}