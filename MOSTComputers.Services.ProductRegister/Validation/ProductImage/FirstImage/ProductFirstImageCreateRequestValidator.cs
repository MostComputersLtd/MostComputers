using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage.FirstImage;
internal sealed class ProductFirstImageCreateRequestValidator : AbstractValidator<ServiceProductFirstImageCreateRequest>
{
    public ProductFirstImageCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.HtmlData).NotEmptyOrWhiteSpace();

        RuleFor(x => x).Must(x => x.ImageData is not null == x.ImageContentType is not null);

        RuleFor(x => x.ImageData).NullOrNotEmpty();
        RuleFor(x => x.ImageContentType).NotEmptyOrWhiteSpace().MaximumLength(50);
    }
}