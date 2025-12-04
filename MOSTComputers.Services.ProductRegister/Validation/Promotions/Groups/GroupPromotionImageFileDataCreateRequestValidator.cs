using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.GroupPromotionImageFileDataConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;
internal sealed class GroupPromotionImageFileDataCreateRequestValidator : AbstractValidator<GroupPromotionImageFileDataCreateRequest>
{
    public GroupPromotionImageFileDataCreateRequestValidator()
    {
        RuleFor(x => x.ImageId).GreaterThan(0);
        RuleFor(x => x.PromotionId).NullOrGreaterThan(0);
        RuleFor(x => x.FileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength);
    }
}