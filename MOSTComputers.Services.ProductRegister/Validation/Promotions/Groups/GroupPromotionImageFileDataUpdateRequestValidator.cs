using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.GroupPromotionImageFileDataConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;

internal sealed class GroupPromotionImageFileDataUpdateRequestValidator : AbstractValidator<GroupPromotionImageFileDataUpdateRequest>
{
    public GroupPromotionImageFileDataUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.NewFileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength);
    }
}