using FluentValidation;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal class LocalImageAndImageFileNameRelationValidator
    : AbstractValidator<Utils.ProductImageFileNameUtils.ImageAndImageFileNameRelation>
{
    public LocalImageAndImageFileNameRelationValidator()
    {
        RuleFor(x => x.ProductImage)
            .SetValidator(new ImageInImageAndImageFileNameRelationValidatorValidator()!)
            .When(image => image is not null);

        RuleFor(x => x.ProductImageFileNameInfo)
            .SetValidator(new ImageFileNameInfoInImageAndImageFileNameRelationValidatorValidator()!)
            .When(imageFileNameInfo => imageFileNameInfo is not null);
    }

    private class ImageInImageAndImageFileNameRelationValidatorValidator
        : AbstractValidator<MOSTComputers.Models.Product.Models.ProductImage>
    {
        public ImageInImageAndImageFileNameRelationValidatorValidator()
        {
            RuleFor(x => x.HtmlData).Must(IsNotEmptyOrWhiteSpace);
            RuleFor(x => x).Must(x => x.ImageData is not null == x.ImageContentType is not null);
            RuleFor(x => x.ImageData).Must(IsNullOrNotEmpty);
            RuleFor(x => x.ImageContentType).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        }
    }

    private class ImageFileNameInfoInImageAndImageFileNameRelationValidatorValidator
        : AbstractValidator<MOSTComputers.Models.Product.Models.ProductImageFileNameInfo>
    {
        public ImageFileNameInfoInImageAndImageFileNameRelationValidatorValidator()
        {
            RuleFor(x => x.FileName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
            RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
        }
    }
}