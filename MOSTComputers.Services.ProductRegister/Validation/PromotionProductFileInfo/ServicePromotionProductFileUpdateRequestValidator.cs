using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.PromotionProductFileInfoConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.PromotionProductFileInfo;
internal sealed class ServicePromotionProductFileUpdateRequestValidator : AbstractValidator<ServicePromotionProductFileUpdateRequest>
{
    public ServicePromotionProductFileUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.NewPromotionFileInfoId).GreaterThan(0);
        RuleFor(x => x.UpdateUserName).NotNullOrWhiteSpace().MaximumLength(LastUpdateUserNameMaxLength);
        RuleFor(x => x.ValidTo).GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom is not null && x.ValidTo is not null);
    }
}