using FluentValidation;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.FailureData.FailedPropertyNameOfProduct;

internal sealed class FailedPropertyNameOfProductCreateRequestValidator : AbstractValidator<FailedPropertyNameOfProductCreateRequest>
{
    public FailedPropertyNameOfProductCreateRequestValidator()
    {
        RuleFor(x => (int)x.ProductId).NotEqual(0);
        RuleFor(x => x.PropertyName).Must(IsNotNullEmptyOrWhiteSpace).MaximumLength(50);
    }
}