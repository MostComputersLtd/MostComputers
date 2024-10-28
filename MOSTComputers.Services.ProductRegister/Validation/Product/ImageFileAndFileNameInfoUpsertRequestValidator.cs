using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Product;

internal sealed class ImageFileAndFileNameInfoUpsertRequestValidator : AbstractValidator<ImageFileAndFileNameInfoUpsertRequest>
{
    public ImageFileAndFileNameInfoUpsertRequestValidator()
    {
        RuleFor(x => x.ImageContentType).Must(IsNotNullEmptyOrWhiteSpace)
            .Must(IsContentTypeValidAndSupported);
        RuleFor(x => x.ImageData).NotNull().NotEmpty();
        RuleFor(x => x.OldFileName).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.RelatedImageId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.CustomFileNameWithoutExtension).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);

        RuleFor(x => x).Must(x => FileNameLengthIsLessThanMaxLength(x.CustomFileNameWithoutExtension, x.ImageContentType, 50));
    }
}