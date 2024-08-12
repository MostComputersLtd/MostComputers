using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage;

internal sealed class ProductImageCreateRequestValidator : AbstractValidator<ServiceProductImageCreateRequest>
{
    public ProductImageCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.HtmlData).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x).Must(x => (x.ImageData is not null) == (x.ImageContentType is not null));
        RuleFor(x => x.ImageData).Must(IsNullOrNotEmpty);
        RuleFor(x => x.ImageContentType).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}