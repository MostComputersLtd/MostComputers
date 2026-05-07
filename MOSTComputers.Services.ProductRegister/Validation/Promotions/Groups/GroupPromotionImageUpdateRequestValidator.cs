using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;

internal sealed class GroupPromotionImageUpdateRequestValidator : AbstractValidator<GroupPromotionImageUpdateRequest>
{
    public GroupPromotionImageUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.PromotionId).NotNull().GreaterThan(0);
        RuleFor(x => x.Image).NotEmpty();
        RuleFor(x => x.ContentType).NotNullOrWhiteSpace();
    }
}