using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage;

internal sealed class ProductFirstImageCreateRequestValidator : AbstractValidator<ServiceProductFirstImageCreateRequest>
{
    public ProductFirstImageCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.XML).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x).Must(x => (x.ImageData is not null) == (x.ImageFileExtension is not null));
        RuleFor(x => x.ImageData).Must(IsNullOrNotEmpty);
        RuleFor(x => x.ImageFileExtension).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}