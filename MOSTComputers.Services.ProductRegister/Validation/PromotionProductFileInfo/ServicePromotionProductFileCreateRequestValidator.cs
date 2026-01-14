using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.PromotionProductFileInfoConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.PromotionProductFileInfo;
internal sealed class ServicePromotionProductFileCreateRequestValidator : AbstractValidator<ServicePromotionProductFileCreateRequest>
{
    public ServicePromotionProductFileCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.PromotionFileInfoId).GreaterThan(0);
        RuleFor(x => x.CreateUserName).NotNullOrWhiteSpace().MaximumLength(CreateUserNameMaxLength);
        RuleFor(x => x.ValidTo).GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom is not null && x.ValidTo is not null);
    }
}