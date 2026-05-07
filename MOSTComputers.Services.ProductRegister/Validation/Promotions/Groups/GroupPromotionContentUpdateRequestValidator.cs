using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;

using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.GroupPromotionContentConstraints;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;

internal sealed class GroupPromotionContentUpdateRequestValidator : AbstractValidator<ServiceGroupPromotionContentUpdateRequest>
{
    public GroupPromotionContentUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotNull().MaximumLength(NameMaxLength);
        RuleFor(x => x.GroupId).NotNull();
        RuleFor(x => x.HtmlContent).NotNull().MaximumLength(HtmlContentMaxLength);
        RuleFor(x => x.DisplayOrder).NotNull();
        RuleFor(x => x.Disabled).NotNull();
        RuleFor(x => x.Restricted).NotNull();
        RuleFor(x => x.MemberOfDefaultGroup).NotNull();

        RuleForEach(x => x.ImageRequests).SetValidator(new ServiceGroupPromotionImageUpsertRequestValidator())
            .When(x => x.ImageRequests is not null);

        RuleFor(x => x.ImageRequests!).HasDuplicateValues(AreRequestsDuplicatingIds)
            .When(x => x.ImageRequests is not null);
    }

    private static bool AreRequestsDuplicatingIds(ServiceGroupPromotionImageUpsertRequest item1, ServiceGroupPromotionImageUpsertRequest item2)
    {
        bool areImageIdsEqual = item1.ExistingImageId == item2.ExistingImageId;

        bool areImageFileNamesEqual = item1.CustomFileNameWithoutExtension != null
            && item2.CustomFileNameWithoutExtension != null
            && item1.CustomFileNameWithoutExtension == item2.CustomFileNameWithoutExtension;

        return areImageIdsEqual || areImageFileNamesEqual;
    }
}