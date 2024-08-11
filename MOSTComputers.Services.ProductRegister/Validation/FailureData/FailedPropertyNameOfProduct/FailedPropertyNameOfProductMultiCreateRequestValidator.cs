using FluentValidation;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;

internal sealed class FailedPropertyNameOfProductMultiCreateRequestValidator : AbstractValidator<FailedPropertyNameOfProductMultiCreateRequest>
{
    public FailedPropertyNameOfProductMultiCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleForEach(x => x.PropertyNames).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
    }
}