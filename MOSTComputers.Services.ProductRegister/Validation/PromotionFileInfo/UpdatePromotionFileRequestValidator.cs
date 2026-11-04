using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.PromotionFileInfoConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.PromotionFileInfo;
internal sealed class UpdatePromotionFileRequestValidator : AbstractValidator<UpdatePromotionFileRequest>
{
    public UpdatePromotionFileRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmptyOrWhiteSpace().MaximumLength(NameMaxLength);

        RuleFor(x => x.ValidTo)
            .GreaterThan(x => x.ValidFrom)
            .When(x => x.ValidFrom != null && x.ValidTo != null);

        When(x => x.NewFileData != null, () =>
        {
            RuleFor(x => x.NewFileData!.FileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength);
            RuleFor(x => x.NewFileData!.ContentType).NotNullOrWhiteSpace().MaximumLength(ContentTypeMaxLength);
            RuleFor(x => x.NewFileData!.Data).NotNull().NotEmpty();
        });

        RuleFor(x => x.Description).NotEmptyOrWhiteSpace().MaximumLength(DescriptionMaxLength);
        RuleFor(x => x.RelatedProductsString).NotEmptyOrWhiteSpace().MaximumLength(RelatedProductsStringMaxLength);
        RuleFor(x => x.UpdateUserName).NotNullOrWhiteSpace().MaximumLength(LastUpdateUserNameMaxLength);
    }
}
