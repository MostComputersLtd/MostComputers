using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductImageFileDataConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFile;
internal sealed class ProductImageFileCreateRequestValidator : AbstractValidator<ProductImageFileCreateRequest>
{
    public ProductImageFileCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);

        RuleFor(x => x.ImageId).NullOrGreaterThan(0);
        RuleFor(x => x.CreateUserName).NotNullOrWhiteSpace();

        RuleFor(x => x.FileData!.FileName).NotNullOrWhiteSpace().MaximumLength(FileNameMaxLength)
            .When(x => x.FileData is not null);

        RuleFor(x => x.FileData!.Data).NotNull().NotEmpty()
           .When(x => x.FileData is not null);

        RuleFor(x => x.CustomDisplayOrder).GreaterThan(0)
            .When(x => x.CustomDisplayOrder is not null);
    }
}