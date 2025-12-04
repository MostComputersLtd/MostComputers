using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage.FileRelated;

internal sealed class ProductImageWithFileForProductUpsertRequestValidator : AbstractValidator<ProductImageWithFileForProductUpsertRequest>
{
    public ProductImageWithFileForProductUpsertRequestValidator()
    {
        RuleFor(x => x.ExistingImageId).NullOrGreaterThan(0);
        RuleFor(x => x.FileExtension).NotNullOrWhiteSpace();
        RuleFor(x => x.ImageData).NotNull().NotEmpty();
        RuleFor(x => x.HtmlData).NotEmptyOrWhiteSpace();

        RuleFor(x => x.FileUpsertRequest!).SetValidator<FileForImageForProductUpsertRequestValidator>(_ => new())
            .When(x => x.FileUpsertRequest is not null);
    }
}