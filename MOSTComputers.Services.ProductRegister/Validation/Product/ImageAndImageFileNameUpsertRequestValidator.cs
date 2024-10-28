using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal sealed class ImageAndImageFileNameUpsertRequestValidator : AbstractValidator<ImageAndImageFileNameUpsertRequest>
{
    public ImageAndImageFileNameUpsertRequestValidator()
    {
        RuleFor(x => x.ImageContentType).Must(IsNotNullEmptyOrWhiteSpace)
            .Must(IsContentTypeValidAndSupported);

        RuleFor(x => x.ImageData).Must(IsNullOrNotEmpty);

        RuleFor(x => x.ProductImageUpsertRequest)
            .SetValidator(new ProductImageUpsertRequestValidator()!)
            .When(x => x.ProductImageUpsertRequest is not null);

        RuleFor(x => x.ProductImageFileNameInfoUpsertRequest)
            .SetValidator(new ProductImageFileNameInfoUpsertRequestValidator()!)
            .When(x => x.ProductImageFileNameInfoUpsertRequest is not null);

        RuleFor(x => x).Must(x =>
        {
            string? fileNameWithoutExtension = x.ProductImageFileNameInfoUpsertRequest?.CustomFileNameWithoutExtension;

            return FileNameLengthIsLessThanMaxLength(fileNameWithoutExtension, x.ImageContentType, 50);
        });
    }
}

internal sealed class ProductImageUpsertRequestValidator : AbstractValidator<ProductImageUpsertRequest>
{
    public ProductImageUpsertRequestValidator()
    {
        RuleFor(x => x.OriginalImageId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.HtmlData).Must(IsNotEmptyOrWhiteSpace);
    }
}

internal sealed class ProductImageFileNameInfoUpsertRequestValidator : AbstractValidator<ProductImageFileNameInfoUpsertRequest>
{
    public ProductImageFileNameInfoUpsertRequestValidator()
    {
        RuleFor(x => x.OriginalImageNumber).Must(NullOrGreaterThanZero);
        RuleFor(x => x.CustomFileNameWithoutExtension).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.NewDisplayOrder).Must(NullOrGreaterThanZero);
    }
}