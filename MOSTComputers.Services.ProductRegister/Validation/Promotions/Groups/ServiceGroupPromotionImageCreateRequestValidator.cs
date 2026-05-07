using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Utils.Files.FileExtensionUtils;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;

internal sealed class ServiceGroupPromotionImageCreateRequestValidator : AbstractValidator<ServiceGroupPromotionImageCreateRequest>
{
    public ServiceGroupPromotionImageCreateRequestValidator()
    {
        RuleFor(x => x.ContentType)
            .NotEmpty()
            .MaximumLength(GroupPromotionImageConstraints.ContentTypeMaxLength)
            .IsImageContentType();

        RuleFor(x => x.FileExtension)
            .NotEmpty()
            .MaximumLength(GroupPromotionImageFileDataConstraints.FileNameMaxLength)
            .IsExtensionValidForContentType(x => x.ContentType);

        RuleFor(x => x.CustomFileNameWithoutExtension)
            .NotEmptyOrWhiteSpace()
            .MaximumLength(GroupPromotionImageFileDataConstraints.FileNameMaxLength);

        RuleFor(x => x.CustomFileNameWithoutExtension).FileNameMaxLengthValidation(
            GroupPromotionImageFileDataConstraints.FileNameMaxLength,
            (obj, property) =>
            {
                string fileExtensionWithDot = GetExtensionWithDotFromExtensionOrFileName(obj.FileExtension)!;

                return obj.CustomFileNameWithoutExtension! + fileExtensionWithDot;
            })
            .When(x => x.CustomFileNameWithoutExtension != null);

        RuleFor(x => x.Image).NotEmpty();
    }
}
