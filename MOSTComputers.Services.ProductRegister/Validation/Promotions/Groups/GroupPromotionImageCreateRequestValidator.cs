using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;

internal sealed class GroupPromotionImageCreateRequestValidator : AbstractValidator<GroupPromotionImageCreateRequest>
{
    public GroupPromotionImageCreateRequestValidator()
    {
        RuleFor(x => x.PromotionId).GreaterThan(0);
        RuleFor(x => x.Image).NotEmpty();
        RuleFor(x => x.ContentType).NotNullOrWhiteSpace();
    }
}