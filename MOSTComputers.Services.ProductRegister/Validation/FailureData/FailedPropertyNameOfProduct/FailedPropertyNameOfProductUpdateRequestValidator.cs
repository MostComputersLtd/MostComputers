using FluentValidation;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;

internal sealed class FailedPropertyNameOfProductUpdateRequestValidator : AbstractValidator<FailedPropertyNameOfProductUpdateRequest>
{
    public FailedPropertyNameOfProductUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.OldPropertyName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.NewPropertyName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
    }
}