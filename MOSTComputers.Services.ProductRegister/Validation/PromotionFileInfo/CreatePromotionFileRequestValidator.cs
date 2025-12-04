using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.PromotionFileInfoConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.PromotionFileInfo;
internal sealed class CreatePromotionFileRequestValidator : AbstractValidator<CreatePromotionFileRequest>
{
    public CreatePromotionFileRequestValidator()
    {
        RuleFor(x => x.Name).NotEmptyOrWhiteSpace().MaximumLength(NameMaxLength);
        RuleFor(x => x.FileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength);
        RuleFor(x => x.FileData).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotEmptyOrWhiteSpace().MaximumLength(DescriptionMaxLength);
        RuleFor(x => x.RelatedProductsString).NotEmptyOrWhiteSpace().MaximumLength(RelatedProductsStringMaxLength);
        RuleFor(x => x.CreateUserName).NotNullOrWhiteSpace().MaximumLength(CreateUserNameMaxLength);
    }
}