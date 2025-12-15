using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductImageConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage;
internal sealed class ProductImageUpdateRequestValidator : AbstractValidator<ServiceProductImageUpdateRequest>
{
    public ProductImageUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.HtmlData).NotEmptyOrWhiteSpace();

        RuleFor(x => x).Must(x => x.ImageData is not null == x.ImageContentType is not null);

        RuleFor(x => x.ImageData).NullOrNotEmpty();
        RuleFor(x => x.ImageContentType).NotEmptyOrWhiteSpace().MaximumLength(ContentTypeMaxLength);
    }
}