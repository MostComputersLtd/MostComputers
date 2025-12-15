using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductImageFileDataConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFile;

internal sealed class ProductImageFileUpdateRequestValidator : AbstractValidator<ProductImageFileChangeRequest>
{
    public ProductImageFileUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.UpdateUserName).NotNullOrWhiteSpace();

        RuleFor(x => x.NewFileData.FileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength);

        RuleFor(x => x.NewFileData.Data).NotNull().NotEmpty();
    }
}