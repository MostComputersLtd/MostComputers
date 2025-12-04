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

        RuleFor(x => x.NewFileData!.FileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength)
            .When(x => x.NewFileData is not null);

        RuleFor(x => x.NewFileData!.Data).NotNull().NotEmpty()
            .When(x => x.NewFileData is not null);

        RuleFor(x => x.Description).NotEmptyOrWhiteSpace().MaximumLength(DescriptionMaxLength);
        RuleFor(x => x.RelatedProductsString).NotEmptyOrWhiteSpace().MaximumLength(RelatedProductsStringMaxLength);
        RuleFor(x => x.UpdateUserName).NotNullOrWhiteSpace().MaximumLength(LastUpdateUserNameMaxLength);
    }
}