using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;

using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.GroupPromotionContentConstraints;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;

internal sealed class GroupPromotionContentCreateRequestValidator : AbstractValidator<ServiceGroupPromotionContentCreateRequest>
{
    public GroupPromotionContentCreateRequestValidator()
    {
        RuleFor(x => x.Name).NotNull().MaximumLength(NameMaxLength);
        RuleFor(x => x.GroupId).NotNull();
        RuleFor(x => x.HtmlContent).NotNull().MaximumLength(HtmlContentMaxLength);
        RuleFor(x => x.DisplayOrder).NotNull();
        RuleFor(x => x.Disabled).NotNull();
        RuleFor(x => x.Restricted).NotNull();
        RuleFor(x => x.MemberOfDefaultGroup).NotNull();

        RuleForEach(x => x.PromotionImageCreateRequests).SetValidator(new ServiceGroupPromotionImageCreateRequestValidator())
            .When(x => x.PromotionImageCreateRequests is not null);

        RuleFor(x => x.PromotionImageCreateRequests!).HasDuplicateValues(AreRequestsDuplicatingIds)
            .When(x => x.PromotionImageCreateRequests is not null);
    }

    private static bool AreRequestsDuplicatingIds(ServiceGroupPromotionImageCreateRequest item1, ServiceGroupPromotionImageCreateRequest item2)
    {
        return item1.CustomFileNameWithoutExtension != null 
            && item2.CustomFileNameWithoutExtension != null
            && item1.CustomFileNameWithoutExtension == item2.CustomFileNameWithoutExtension;
    }
}