using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductCharacteristic;

internal sealed class ProductCharacteristicCreateRequestValidator : AbstractValidator<ProductCharacteristicCreateRequest>
{
    public ProductCharacteristicCreateRequestValidator()
    {
        RuleFor(x => x.CategoryId).Must(NullOrGreaterThanZero);
        RuleFor(x => x.Name).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.Meaning).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanOrEqualToZero);
        RuleFor(x => x.KWPrCh).NotNull();
    }
}