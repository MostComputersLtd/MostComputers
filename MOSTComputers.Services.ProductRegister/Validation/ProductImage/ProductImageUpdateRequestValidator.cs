using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage;

internal sealed class ProductImageUpdateRequestValidator : AbstractValidator<ProductImageUpdateRequest>
{
    public ProductImageUpdateRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.XML).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.ImageData);
        RuleFor(x => x.ImageFileExtension).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
    }
}