using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.PromotionProductFileInfoConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.PromotionProductFileInfo;
internal sealed class ServicePromotionProductFileInfoCreateRequestValidator : AbstractValidator<ServicePromotionProductFileCreateRequest>
{
    public ServicePromotionProductFileInfoCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.PromotionFileInfoId).GreaterThan(0);
        RuleFor(x => x.CreateUserName).NotNullOrWhiteSpace().MaximumLength(CreateUserNameMaxLength);
    }
}