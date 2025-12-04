using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductImageFileNameInfoConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFile;

internal sealed class ProductImageFileRenameRequestValidator : AbstractValidator<ProductImageFileRenameRequest>
{
    public ProductImageFileRenameRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.UpdateUserName).NotNullOrWhiteSpace();

        RuleFor(x => x.NewFileNameWithoutExtension).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength);
    }
}