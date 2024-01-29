using FluentValidation;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImageFileNameInfo;

internal sealed class ProductImageFileNameInfoUpdateRequestValidator : AbstractValidator<ServiceProductImageFileNameInfoUpdateRequest>
{
    public ProductImageFileNameInfoUpdateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.FileName).Must(IsNotEmptyOrWhiteSpace).MaximumLength(50);
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.NewDisplayOrder).Must(NullOrGreaterThanZero);
    }
}