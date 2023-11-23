using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductProperty;

internal sealed class ProductPropertyByCharacteristicNameCreateRequestValidator : AbstractValidator<ProductPropertyByCharacteristicNameCreateRequest>
{
    public ProductPropertyByCharacteristicNameCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProductCharacteristicName).Must(IsNotNullEmptyOrWhiteSpace);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.Value).Must(IsNotEmptyOrWhiteSpace).MaximumLength(200);
    }
}